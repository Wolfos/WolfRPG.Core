using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Experimental.GraphView;
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

        private ScrollView _objectList;
        private ObjectField _databaseAssetField;
        private TextAsset _databaseAsset;
        private Button _newObjectButton;
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

            var root = rootVisualElement;
            
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
            root.Add(uxml);
            uxml.styleSheets.Add(styleSheet);
            
            if (AddressableAssetSettingsDefaultObject.SettingsExists)
            {
                _addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            }
            else
            {
                DisplayWarning("Addressables has not been setup for this project. Please create the addressables settings from the Addressables Groups window and reopen the WolfRPG editor");
                root.Query<GroupBox>("Editor").First().SetEnabled(false);
                return;
            }
            
            var newAssetButton = root.Query<Button>("NewAssetButton").First();
            newAssetButton.clicked += OnCreateNewAssetButtonClicked;
            
            _newObjectButton = root.Query<Button>("NewObject").First();
            _newObjectButton.clicked += OnCreateNewObjectClicked;

            _databaseAssetField = root.Query<ObjectField>("DatabaseAssetField").First();
            _objectList = root.Query<ScrollView>("ObjectList").First();

            _objectEditorContainer = root.Query<GroupBox>("ObjectEditor").First();

            _tabContainer = root.Query<GroupBox>("Tabs").First();
            
            // TODO: This is a placeholder to test loading
            _objectEditorContainer.Add(_objectEditor.CreateUI());

            _objectEditor.OnSelectedObjectUpdated += OnSelectedObjectUpdated;

            _database = _databaseFactory.GetDefaultDatabase(out _databaseAsset);
            if (_database != null)
            {
                _databaseAssetField.SetValueWithoutNotify(_databaseAsset);
                newAssetButton.SetEnabled(false);
                var newTabButton = root.Query<Button>("NewTabButton").First();
                newTabButton.clicked += AddTab;
                PopulateObjectList();
                CreateTabs();
            }
            else
            {
                _newObjectButton.SetEnabled(false);
            }
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
            _newObjectButton.SetEnabled(true);
        }

        private void OnCreateNewObjectClicked()
        {
            var database = (RPGDatabase) _database;
            
            var name = $"New Object {database.Objects.Count + 1}";
            
            // TODO: Apply category before first serialization
            var newObject = _objectFactory.CreateNewObject(name);
            newObject.Category = _currentTab;
            
            database.AddObjectInstance(newObject);

            var label = new Label(newObject.Name);
            _objectButtons.Add(label);
            var index = database.Objects.Count - 1;
            label.RegisterCallback<ClickEvent>(_ => OnObjectSelected(index, newObject));
            _objectList.Add(label);
            
            _databaseFactory.SaveDefaultDatabase();
        }

        private void OnSelectedObjectUpdated()
        {
            _objectButtons[_selectedObjectId].text = _objectEditor.SelectedObject.Name;
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
    }
}