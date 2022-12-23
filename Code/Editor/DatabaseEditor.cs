using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace WolfRPG.Core
{
    public class DatabaseEditor : EditorWindow
    {
        private ScrollView _objectList;
        private IRPGDatabaseFactory _databaseFactory;
        private IRPGDatabase _database;

        private ObjectField _databaseAssetField;
        private TextAsset _databaseAsset;

        [MenuItem("WolfRPG/Database Editor")]
        public static void Open()
        {
            var window = GetWindow<DatabaseEditor>();
            window.titleContent = new("WolfRPG Database Editor");
        }

        public void CreateGUI()
        {
            _databaseFactory = new RPGDatabaseFactory();
            
            VisualElement root = rootVisualElement;
            
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

        private void OnCreateNewAssetButtonClicked()
        {
            _database = _databaseFactory.CreateNewDatabase(out var guid);

            if (guid == null) return;
            
            var path = AssetDatabase.GUIDToAssetPath(guid.Value);
            _databaseAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            
            _databaseAssetField.SetValueWithoutNotify(_databaseAsset);
        }

        private void OnCreateNewObjectClicked()
        {
            var newObject = new RPGObject();
            newObject.Name = "New Object";//$"New Object {_objects.Count + 1}";
            _database.AddObjectInstance(newObject);

            var label = new Label(newObject.Name);
            //label.RegisterCallback<ClickEvent>(_ => OnObjectClicked(_objects.Count - 1));
            _objectList.Add(label);
        }

        private void OnObjectClicked(int objectIndex)
        {
            
        }
    }
}