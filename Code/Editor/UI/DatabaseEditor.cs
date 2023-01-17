using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace WolfRPG.Core
{
    public class DatabaseEditor : EditorWindow
    {
        private IRPGDatabaseFactory _databaseFactory;
        private IRPGObjectFactory _objectFactory;
        private IRPGDatabase _database;
        private ObjectEditor _objectEditor = new();
        
        private AddressableAssetSettings _addressableSettings;
        private AddressableAssetGroup _assetGroup;

        private VisualElement _root;
        private ScrollView _objectList;
        private ObjectField _databaseAssetField;
        private TextAsset _databaseAsset;
        private Button _newObjectButton;
        private Button _newAssetButton;
        private Button _saveButton;
        private GroupBox _objectEditorContainer;
        private List<Label> _objectButtons = new();
        private GroupBox _tabContainer;
        private List<Label> _tabs = new();

        private int _selectedObjectId = -1;
        private int _currentTab;
        


        [MenuItem("WolfRPG/Database Editor")]
        public static void Open()
        {
            var window = GetWindow<DatabaseEditor>();
            window.titleContent = new("WolfRPG Database Editor");
        }

        public void CreateGUI()
        {
            _databaseFactory = new RPGDatabaseFactory();
            _objectFactory = new RPGObjectFactory();

            _root = rootVisualElement;
            
            // Workaround because you can't add stylesheets from a package path in the editor
           var styleSheet =
               AssetDatabase.LoadAssetAtPath<StyleSheet>(
                   "Packages/nl.eestudio.wolfrpg.core/Code/Editor/UI/WolfRPG.uss");

            // Import UXML
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Packages/nl.eestudio.wolfrpg.core/Code/Editor/UI/DatabaseEditor.uxml");
            VisualElement uxml = visualTree.Instantiate();
            uxml.style.flexGrow = 1;
            uxml.style.flexShrink = 1;
            _root.Add(uxml);
            uxml.styleSheets.Add(styleSheet);
            
            if (AddressableAssetSettingsDefaultObject.SettingsExists)
            {
                _addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            }
            else
            {
                DisplayWarning("Addressables has not been setup for this project. Please create the addressables settings from the Addressables Groups window and reopen the WolfRPG editor");
                _root.Query<GroupBox>("Editor").First().SetEnabled(false);
                return;
            }
            
            _newAssetButton = _root.Query<Button>("NewAssetButton").First();
            _newAssetButton.clicked += OnCreateNewAssetButtonClicked;
            
            _newObjectButton = _root.Query<Button>("NewObject").First();
            _newObjectButton.clicked += OnCreateNewObjectClicked;

            _databaseAssetField = _root.Query<ObjectField>("DatabaseAssetField").First();
            _objectList = _root.Query<ScrollView>("ObjectList").First();

            _objectEditorContainer = _root.Query<GroupBox>("ObjectEditor").First();

            _tabContainer = _root.Query<GroupBox>("Tabs").First();
            
            _objectEditorContainer.Add(_objectEditor.CreateUI());
            _objectEditor.OnSelectedObjectUpdated += OnSelectedObjectUpdated;

            _saveButton = _root.Query<Button>("SaveButton").First();
            _saveButton.clicked += () =>
            {
                _databaseFactory.SaveDatabase(_database, GetDatabasePath());
            };
            
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
        }

        private void Init()
        {
            //TODO: Also init when new database is selected
            _databaseAssetField.SetValueWithoutNotify(_databaseAsset);
            _newAssetButton.SetEnabled(false);
            _newObjectButton.SetEnabled(true);
            _saveButton.SetEnabled(true);
            var newTabButton = _root.Query<Button>("NewTabButton").First();
            newTabButton.clicked += AddTab;
            PopulateObjectList();
            CreateTabs();
        }

        private void PopulateObjectList()
        {
            ClearObjectList();
            
            int i = 0;
            foreach (var rpgObject in _database.Objects)
            {
                var newObject = rpgObject.Value;
                if(newObject.Category != _currentTab) continue;
                
                
                var label = new Label(newObject.Name);
                
                _objectButtons.Add(label);
                var i1 = i;
                label.RegisterCallback<ClickEvent>(_ => OnObjectSelected(i1, newObject));
                _objectList.Add(label);

                i++;
            }
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

        private void DisplayWarning(string text)
        {
            var warning = rootVisualElement.Query<Label>("Warning").First();
            warning.text = text;
            warning.style.display = DisplayStyle.Flex;
        }

        private void HideWarning()
        {
            var warning = rootVisualElement.Query<Label>("Warning").First();
            warning.style.display = DisplayStyle.None;
        }
        
        private void DisplayError(string text)
        {
            var error = rootVisualElement.Query<Label>("Error").First();
            error.text = text;
            error.style.display = DisplayStyle.Flex;
        }

        private void HideError()
        {
            var error = rootVisualElement.Query<Label>("Error").First();
            error.style.display = DisplayStyle.None;
        }

        private void OnCreateNewAssetButtonClicked()
        {
            _database = _databaseFactory.CreateNewDatabase(out _databaseAsset);

            if (_databaseAsset == null) return;
            
            _databaseAssetField.SetValueWithoutNotify(_databaseAsset);

            Init();
        }

        private void OnCreateNewObjectClicked()
        {
            var name = $"New Object {_database.Objects.Count + 1}";
            
            var newObject = _objectFactory.CreateNewObject(name, _currentTab);
            
            _database.AddObjectInstance(newObject);

            var label = new Label(newObject.Name);
            _objectButtons.Add(label);
            var index = _objectButtons.Count - 1;
            label.RegisterCallback<ClickEvent>(_ => OnObjectSelected(index, newObject));
            _objectList.Add(label);
            
            _databaseFactory.SaveDatabase(_database, GetDatabasePath());
        }

        private void OnSelectedObjectUpdated()
        {
            _objectButtons[_selectedObjectId].text = _objectEditor.SelectedObject.Name;
        }

        private void DeselectObject()
        {
            if (_selectedObjectId != -1)
            {
                _objectButtons[_selectedObjectId].RemoveFromClassList("Selected");
            }
            _selectedObjectId = -1;
            _objectEditor.Deselect();
        }
        
        private void OnObjectSelected(int objectIndex, IRPGObject rpgObject)
        {
            if (objectIndex == _selectedObjectId)
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