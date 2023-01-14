using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace WolfRPG.Core
{
    public class ObjectEditor
    {
        public Action SelectedObjectUpdated { get; set; }
        public IRPGObject SelectedObject { get; private set; }
        
        private TextField _nameField;
        private TemplateContainer _container;
        private bool _addingComponent;
        
        public VisualElement CreateUI()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Packages/nl.eestudio.wolfrpg.core/Code/Editor/UI/ObjectEditor.uxml");
            _container = visualTree.Instantiate();
            _container.style.display = DisplayStyle.None;
            _nameField = _container.Query<TextField>("NameField").First();
            var newComponentButton = _container.Query<Button>("NewComponentButton").First();
            newComponentButton.clicked += OnNewComponentButtonPressed;

            return _container;
        }
        
        public void SelectObject(IRPGObject rpgObject)
        {
            SelectedObject = rpgObject;

            _container.style.display = DisplayStyle.Flex;
            _nameField.value = SelectedObject.Name;
            
            _nameField.RegisterValueChangedCallback(changeEvent =>
            {
                SelectedObject.Name = changeEvent.newValue;
                SelectedObjectUpdated?.Invoke();
            });
        }

        public void Deselect()
        {
            SelectedObject = null;
            _container.style.display = DisplayStyle.None;
        }

        private void AddNewComponent(Type type)
        {
            var newComponent = (IRPGComponent)Activator.CreateInstance(type);
            SelectedObject.AddComponent(newComponent);
        }

        private void OnNewComponentButtonPressed()
        {
            if (_addingComponent) return;

            _addingComponent = true;
            
            var componentList = new List<string>();
            // Use reflection to build a list of every class implementing IRPGComponent
            var types = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IRPGComponent).IsAssignableFrom(type));
            
            var typeList = new List<Type>();
            foreach (var type in types)
            {
                // Exclude components we already have
                if (SelectedObject.HasComponent(type)) continue;
                // Exclude IRPGComponent itself
                if(type == typeof(IRPGComponent)) continue;

                typeList.Add(type);
            }

            // Sort alphabetically
            typeList.OrderBy(t => t.ToString());
            
            foreach (var type in typeList)
            {
                componentList.Add(type.ToString());
            }
            
            var popupField = new PopupField<string>("", componentList, 0);
            _container.Add(popupField);

            var button = new Button();
            button.clicked += () =>
            {
                var type = typeList[popupField.index];
                AddNewComponent(type);
                _addingComponent = false;

                _container.Remove(button);
                _container.Remove(popupField);
            };
            
            button.text = "Add";
            _container.Add(button);
        }
    }
}