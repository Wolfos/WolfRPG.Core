using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.UnityConverters.Math;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.UIElements;

namespace WolfRPG.Core
{
    public class DatabaseEditor : EditorWindow
    {
        private static DatabaseEditor _instance;
        
        private IRPGDatabaseFactory _databaseFactory;
        private IRPGObjectFactory _objectFactory;
        private IRPGDatabase _database;
        private readonly ObjectEditor _objectEditor = new();
        
        private double _messageStartTime;
        private float _messageDuration;

        private VisualElement _root;
        private ScrollView _objectList;
        private TextAsset _databaseAsset;
        private Button _newObjectButton;
        private Button _newAssetButton;
        private Button _saveButton;
        private Button _undoButton;
        private DropdownField _templateDropdown;
        private GroupBox _objectEditorContainer;
        private readonly List<GroupBox> _objectButtons = new();
        private GroupBox _tabContainer;
        private readonly List<Label> _tabs = new();

        private readonly List<IRPGObject> _dirtyObjects = new();

        private int _selectedObjectId = -1;
        private int _currentTab;

        private const int _undoBufferMaxSize = 1000;
        // Item1 is a copy of the IRPGObject at that time, Item2 is whether it was dirtied at that point
        private readonly List<Tuple<IRPGObject, bool>> _undoBuffer = new();

        private List<IWolfRPGTemplate> _templates;


        [MenuItem("WolfRPG/Database Editor")]
        public static void Open()
        {
            var window = GetWindow<DatabaseEditor>();
            window.titleContent = new("WolfRPG Database Editor");
        }

        public void CreateGUI()
        {
            _instance = this;
            
            _databaseFactory = new RPGDatabaseFactory();
            _objectFactory = new RPGObjectFactory();

            _root = rootVisualElement;
            
            // Workaround because you can't add stylesheets from a package path in the editor
           var styleSheet =
               AssetDatabase.LoadAssetAtPath<StyleSheet>(
                   "Packages/nl.eestudio.wolfrpg.core/Code/Editor/Database/UI/WolfRPG.uss");

            // Import UXML
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Packages/nl.eestudio.wolfrpg.core/Code/Editor/Database/UI/DatabaseEditor.uxml");
            VisualElement uxml = visualTree.Instantiate();
            uxml.style.flexGrow = 1;
            uxml.style.flexShrink = 1;
            _root.Add(uxml);
            uxml.styleSheets.Add(styleSheet);
            
            if (AddressableAssetSettingsDefaultObject.SettingsExists)
            {
                //_addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            }
            else
            {
                DisplayMessage("Addressables has not been setup for this project. Please create the addressables settings from the Addressables Groups window and reopen the WolfRPG editor", 0);
                _root.Query<GroupBox>("Editor").First().SetEnabled(false);
                return;
            }
            
            _newAssetButton = _root.Query<Button>("NewAssetButton").First();
            _newAssetButton.clicked += OnCreateNewAssetButtonClicked;
            
            _newObjectButton = _root.Query<Button>("NewObject").First();
            _newObjectButton.clicked += OnCreateNewObjectClicked;
            
            _objectList = _root.Query<ScrollView>("ObjectList").First();

            _objectEditorContainer = _root.Query<GroupBox>("ObjectEditor").First();

            _tabContainer = _root.Query<GroupBox>("Tabs").First();
            
            _templateDropdown = _root.Query<DropdownField>("Template").First();
            
            _objectEditor.OnBeforeSelectedObjectUpdated += OnBeforeSelectedObjectUpdated;
            _objectEditor.OnSelectedObjectUpdated += OnSelectedObjectUpdated;

            _saveButton = _root.Query<Button>("SaveButton").First();
            _saveButton.clicked += Save;
            
            _undoButton = _root.Query<Button>("UndoButton").First();
            _undoButton.clicked += Undo;

            _database = _databaseFactory.GetDefaultDatabase(out _databaseAsset);
            if (_database != null)
            {
                Init();
            }
            else
            {
                _newObjectButton.SetEnabled(false);
                _saveButton.SetEnabled(false);
            }
            
            _objectEditorContainer.Add(_objectEditor.CreateUI(_database));
            
            AddTemplates();
        }

        private void Init()
        {
            _newAssetButton.style.display = DisplayStyle.None;
            _newObjectButton.SetEnabled(true);
            _saveButton.SetEnabled(true);
            
            var newTabButton = _root.Query<Button>("NewTabButton").First();
            newTabButton.clicked += AddTab;
            
            PopulateObjectList();
            CreateTabs();
        }

        private void AddTemplates()
        {
            var templateList = new List<string>();
            // Use reflection to build a list of every class implementing IWolfRPGTemplate
            var types = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IWolfRPGTemplate).IsAssignableFrom(type));
            
            templateList.Add("None");

            _templates = new();
            foreach (var type in types)
            {
                // Exclude IWolfRPGTemplate itself
                if(type == typeof(IWolfRPGTemplate)) continue;
                var template = (IWolfRPGTemplate) Activator.CreateInstance(type);

                templateList.Add(template.Name);
                _templates.Add(template);
            }

            _templateDropdown.choices = templateList;
            _templateDropdown.index = 0;
        }

        private void PopulateObjectList()
        {
            ClearObjectList();

            int i = 0;
            foreach (var kvp in _database.Objects)
            {
                var rpgObject = kvp.Value;
                if(rpgObject.Category != _currentTab) continue;

                var objectName = rpgObject.Name;
                if (_dirtyObjects.Contains(rpgObject))
                {
                    objectName += "*";
                }
                var label = new Label(objectName);
                var groupbox = new GroupBox();
                groupbox.AddToClassList("Horizontal");
                var deleteButton = new Button(() =>
                {
                    DeleteObject(rpgObject);
                });
                deleteButton.name = "DeleteButton";
                deleteButton.text = "x";
                groupbox.Add(deleteButton);
                var i1 = i;

                _objectButtons.Add(groupbox);
                groupbox.RegisterCallback<ClickEvent>(_ => OnObjectSelected(i1, rpgObject));
                groupbox.Add(label);
                _objectList.Add(groupbox);
                

                if (i == _selectedObjectId)
                {
                    label.AddToClassList("Selected");
                }

                i++;
            }
        }

        private void DeleteObject(IRPGObject rpgObject)
        {
            if (_objectEditor.SelectedObject == rpgObject)
            {
                DeselectObject();
            }
            
            _objectFactory.DeleteObject(rpgObject);
            PopulateObjectList();
            // Save database only to remove the object reference, as the file was deleted
            _databaseFactory.SaveDatabase(_database, GetDatabasePath());
        }

        private void ClearObjectList()
        {
            _selectedObjectId = -1;
            
            var toDelete = new List<VisualElement>();
            foreach (var obj in _objectButtons)
            {
                toDelete.Add(obj);
            }

            foreach (var obj in toDelete)
            {
                obj.RemoveFromHierarchy();
            }
            _objectButtons.Clear();
        }

        private void CreateTabs()
        {
            var i = 0;
            foreach (var category in _database.Categories)
            {
                var tab = new Label(category);
                tab.AddToClassList("Tab");
                _tabContainer.Add(tab);
                if (i == 0)
                {
                    tab.SendToBack();
                }
                else
                {
                    tab.PlaceInFront(_tabs.Last());
                }

                _tabs.Add(tab);
                var i1 = i;
                tab.RegisterCallback<ClickEvent>(evt =>
                {
                    if (evt.clickCount == 1)
                    {
                        SelectTab(i1);
                    }
                    else if (evt.clickCount == 2)
                    {
                        RenameTab(i1);
                    }
                });
                
                i++;
            }
            
            _tabs[0].AddToClassList("SelectedTab");
        }

        private void AddTab()
        {
            var tab = new Label("New Category");
            _tabContainer.Add(tab);
            tab.PlaceInFront(_tabs.Last());
            _tabs.Add(tab);
            var index = _tabs.Count - 1;
            tab.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.clickCount == 1)
                {
                    SelectTab(index);
                }
                else if (evt.clickCount == 2)
                {
                    RenameTab(index);
                }
            });
            
            _database.Categories.Add("New Category");
        }

        private void SelectTab(int index)
        {
            if (index == _currentTab) return;
            _tabs[_currentTab].RemoveFromClassList("SelectedTab");
            _currentTab = index;
            _tabs[_currentTab].AddToClassList("SelectedTab");
            
            DeselectObject();
            
            _selectedObjectId = -1;
            PopulateObjectList();
        }

        private void RenameTab(int index)
        {
            var tab = _tabs[index];
            var textField = new TextField();
            textField.SetValueWithoutNotify(tab.text);
            tab.parent.Add(textField);
            textField.PlaceBehind(tab);

            var button = new Button();
            button.text = "Apply";
            tab.parent.Add(button);
            button.PlaceInFront(textField);
            button.clicked += () =>
            {
                _database.Categories[index] = textField.value;
                tab.text = textField.value;
                tab.style.display = DisplayStyle.Flex;
                
                button.RemoveFromHierarchy();
                textField.RemoveFromHierarchy();
            };
            tab.style.display = DisplayStyle.None;
        }

        public static void DisplayMessage(string text, float duration = 5.0f)
        {
            var label = _instance.rootVisualElement.Query<Label>("Message").First();
            label.text = text;
            label.style.display = DisplayStyle.Flex;

            if (duration != 0)
            {
                _instance._messageDuration = duration;
                _instance._messageStartTime = EditorApplication.timeSinceStartup;
            }
            else
            {
                _instance._messageDuration = Mathf.Infinity;
            }
        }

        private void OnInspectorUpdate()
        {
            if (_messageStartTime < EditorApplication.timeSinceStartup - _messageDuration)
            {
                var label = rootVisualElement.Query<Label>("Message").First();
                label.text = "";
                label.style.display = DisplayStyle.None;
            }
        }
        

        private void OnCreateNewAssetButtonClicked()
        {
            _database = _databaseFactory.CreateNewDatabase(out _databaseAsset);

            if (_databaseAsset == null) return;

            Init();
        }

        private void OnCreateNewObjectClicked()
        {
            var name = $"New Object {_database.NumObjectsAdded + 1}";
            _database.NumObjectsAdded++;
            
            var newObject = _objectFactory.CreateNewObject(name, _currentTab);

            if (_templateDropdown.index != 0)
            {
                var template = _templates[_templateDropdown.index - 1];
                foreach (var component in template.GetComponents())
                {
                    newObject.AddComponent(component);
                }
            }
            
            _database.AddObjectInstance(newObject);
            _databaseFactory.SaveDatabase(_database, GetDatabasePath());
            
            PopulateObjectList();
        }

        private void OnBeforeSelectedObjectUpdated()
        {
            var selectedObject = _objectEditor.SelectedObject;
            // Serialize to JSON and back to create a copy for the undo buffer
            var json = JsonConvert.SerializeObject(selectedObject, Formatting.None, 
                Settings.JsonSerializerSettings);

            var deserialized = JsonConvert.DeserializeObject<RPGObject>(json, Settings.JsonSerializerSettings);
            var isDirty = _dirtyObjects.Contains(selectedObject);
            _undoBuffer.Add(new(deserialized, !isDirty));

            if (_undoBuffer.Count > _undoBufferMaxSize)
            {
                _undoBuffer.RemoveAt(0);
            }
        }

        private void OnSelectedObjectUpdated()
        {
            if (_dirtyObjects.Contains(_objectEditor.SelectedObject) == false)
            {
                _dirtyObjects.Add(_objectEditor.SelectedObject);
            }
            
            PopulateObjectList();
        }

        private void DeselectObject()
        {
            if (_selectedObjectId != -1)
            {
                _objectButtons[_selectedObjectId].RemoveFromClassList("Selected");
            }
            _selectedObjectId = -1;
            _objectEditor.DeselectObject();
        }

        private void Save()
        {
            _databaseFactory.SaveDatabase(_database, GetDatabasePath());
            foreach (var rpgObject in _dirtyObjects)
            {
                _objectFactory.SaveObject(rpgObject);
            }
            
            _dirtyObjects.Clear();
            PopulateObjectList();
        }

        private void Undo()
        {
            if (_undoBuffer.Count == 0) return;
            
            var undoState = _undoBuffer.Last();
            var rpgObject = undoState.Item1;
            var unDirty = undoState.Item2;
            // Object was deleted
            if (_database.GetObjectInstance(rpgObject.Guid) == null)
            {
                _undoBuffer.RemoveAt(_undoBuffer.Count - 1);
                Undo();
                return;
            }
            
            _database.SetObjectInstance(rpgObject);
            if (_objectEditor.SelectedObject.Guid == rpgObject.Guid)
            {
                _objectEditor.SelectObject(rpgObject);
            }

            _dirtyObjects.RemoveAll(x => x.Guid == rpgObject.Guid);
            if (unDirty == false)
            {
                _dirtyObjects.Add(rpgObject);
            }

            _undoBuffer.RemoveAt(_undoBuffer.Count - 1);
            
            PopulateObjectList();
        }
        
        private void OnObjectSelected(int objectIndex, IRPGObject rpgObject)
        {
            if (objectIndex == _selectedObjectId || objectIndex >= _objectButtons.Count)
            {
                return;
            }

            // Deselect previous
            if (_selectedObjectId != -1)
            {
                _objectButtons[_selectedObjectId].RemoveFromClassList("Selected");
            }
            
            _objectButtons[objectIndex].AddToClassList("Selected");

            _selectedObjectId = objectIndex;

            _objectEditor.SelectObject(rpgObject);
        }

        private string GetDatabasePath()
        {
            var relativePath = AssetDatabase.GetAssetPath(_databaseAsset);
            return $"{Path.GetDirectoryName(Application.dataPath)}/{relativePath}";
        }
    }
}