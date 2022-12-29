using System.Collections.Generic;
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
        private ScrollView _objectList;
        private IRPGDatabaseFactory _databaseFactory;
        private IRPGObjectFactory _objectFactory;
        private IRPGDatabase _database;

        private ObjectField _databaseAssetField;
        private TextAsset _databaseAsset;
        private AddressableAssetSettings _addressableSettings;
        private AddressableAssetGroup _assetGroup;

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
            
            // Workaround because you can't add stylesheets from a package path
           var styleSheet =
               AssetDatabase.LoadAssetAtPath<StyleSheet>(
                   "Packages/nl.eestudio.wolfrpg.core/Code/Editor/WolfRPG.uss");

            // Import UXML
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Packages/nl.eestudio.wolfrpg.core/Code/Editor/DatabaseEditor.uxml");
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
            
            var newObjectButton = root.Query<Button>("NewObject").First();
            newObjectButton.clicked += OnCreateNewObjectClicked;

            _databaseAssetField = root.Query<ObjectField>("DatabaseAssetField").First();
            _objectList = root.Query<ScrollView>("ObjectList").First();

            _database = _databaseFactory.GetDefaultDatabase(out _databaseAsset);
            if (_database != null)
            {
                _databaseAssetField.SetValueWithoutNotify(_databaseAsset);
                newAssetButton.SetEnabled(false);
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
        }

        private void OnCreateNewObjectClicked()
        {
            var database = (RPGDatabase) _database;
            
            var name = $"New Object {database.Objects.Count + 1}";
            
            var newObject = (RPGObject)_objectFactory.CreateNewObject(name);
            
            database.AddObjectInstance(newObject);

            var label = new Label(newObject.Name);
            label.RegisterCallback<ClickEvent>(_ => OnObjectClicked(database.Objects.Count - 1));
            _objectList.Add(label);
            
            _databaseFactory.SaveDefaultDatabase();
        }

        private void OnObjectClicked(int objectIndex)
        {
            
        }
    }
}