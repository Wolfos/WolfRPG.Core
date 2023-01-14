using System.Collections.Generic;
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


        //TODO: Are these actually needed?
        private int _selectedObjectId = -1;


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
            
            // TODO: This is a placeholder to test loading
            _objectEditorContainer.Add(_objectEditor.GetUI());

            _objectEditor.SelectedObjectUpdated += OnSelectedObjectUpdated;

            _database = _databaseFactory.GetDefaultDatabase(out _databaseAsset);
            if (_database != null)
            {
                _databaseAssetField.SetValueWithoutNotify(_databaseAsset);
                newAssetButton.SetEnabled(false);
                PopulateObjectList();
            }
            else
            {
                _newObjectButton.SetEnabled(false);
            }
        }

        private void PopulateObjectList()
        {
            int i = 0;
            foreach (var rpgObject in _database.Objects)
            {
                var newObject = rpgObject.Value as RPGObject;
                var label = new Label(newObject.Name);
                
                _objectButtons.Add(label);
                var i1 = i;
                label.RegisterCallback<ClickEvent>(_ => OnObjectSelected(i1, newObject));
                _objectList.Add(label);

                i++;
            }
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
            
            var newObject = (RPGObject)_objectFactory.CreateNewObject(name);
            
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
        
        private void OnObjectSelected(int objectIndex, RPGObject rpgObject)
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