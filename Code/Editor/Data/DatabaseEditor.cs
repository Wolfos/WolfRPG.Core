using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace WolfRPG.Core
{
    public class DatabaseEditor : EditorWindow
    {
        private List<RPGObject> _objects = new(); // TODO: Temporary
        private ScrollView _objectList;

        [MenuItem("WolfRPG/Database Editor")]
        public static void Open()
        {
            var window = GetWindow<DatabaseEditor>();
            window.titleContent = new("WolfRPG Database Editor");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            
           var styleSheet =
               AssetDatabase.LoadAssetAtPath<StyleSheet>(
                   "Packages/nl.eestudio.wolfrpg.core/Code/Editor/Data/WolfRPG.uss");

            // Import UXML
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Packages/nl.eestudio.wolfrpg.core/Code/Editor/Data/DatabaseEditor.uxml");
            VisualElement uxml = visualTree.Instantiate();
            root.Add(uxml);
            uxml.styleSheets.Add(styleSheet);
            
            var newObjectButton = root.Query<Button>("NewObject").First();
            newObjectButton.clicked += OnCreateNewObjectClicked;

            _objectList = root.Query<ScrollView>("ObjectList").First();
            //VisualElement labelWithStyle = new Label("Hello World! With Style");
            //labelWithStyle.styleSheets.Add(styleSheet);
            //root.Add(labelWithStyle);
        }

        private void OnCreateNewObjectClicked()
        {
            var newObject = new RPGObject();
            newObject.Name = $"New Object {_objects.Count + 1}";
            _objects.Add(newObject);

            var label = new Label(newObject.Name);
            label.RegisterCallback<ClickEvent>(_ => OnObjectClicked(_objects.Count - 1));
            _objectList.Add(label);
        }

        private void OnObjectClicked(int objectIndex)
        {
            
        }
    }
}