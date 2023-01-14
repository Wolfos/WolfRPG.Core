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
        public Action OnSelectedObjectUpdated { get; set; }
        public IRPGObject SelectedObject { get; private set; }
        
        private TextField _nameField;
        private TemplateContainer _container;
        private GroupBox _bottomBox; // box that's located below the "add component" button. Gets cleared when object is deselected
        private GroupBox _objectList;
        private bool _isAddingComponent;

        public ObjectEditor()
        {
            OnSelectedObjectUpdated += BuildComponentsList;
        }
        
        public VisualElement CreateUI()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Packages/nl.eestudio.wolfrpg.core/Code/Editor/UI/ObjectEditor.uxml");
            _container = visualTree.Instantiate();
            _container.style.display = DisplayStyle.None;
            _nameField = _container.Query<TextField>("NameField").First();
            _objectList = _container.Query<GroupBox>("ObjectList").First();
            
            var newComponentButton = _container.Query<Button>("NewComponentButton").First();
            newComponentButton.clicked += OnNewComponentButtonPressed;

            _bottomBox = new();
            _container.Add(_bottomBox);

            return _container;
        }
        
        public void SelectObject(IRPGObject rpgObject)
        {
            Clear();
            
            SelectedObject = rpgObject;

            _container.style.display = DisplayStyle.Flex;
            _nameField.value = SelectedObject.Name;
            
            _nameField.RegisterValueChangedCallback(changeEvent =>
            {
                SelectedObject.Name = changeEvent.newValue;
                OnSelectedObjectUpdated?.Invoke();
            });
            
            BuildComponentsList();
        }

        private void Clear()
        {
            _isAddingComponent = false;

            var toRemove = new List<VisualElement>();
            toRemove.AddRange(_bottomBox.Children());
            toRemove.AddRange(_objectList.Children());

            foreach (var element in toRemove)
            {
                element.RemoveFromHierarchy();
            }
            
        }
        
        private void BuildComponentsList()
        {
            Clear();
            
            var components = SelectedObject.GetAllComponents();
            foreach (var component in components)
            {
                var foldout = new Foldout
                {
                    text = component.GetType().Name
                };
                _objectList.Add(foldout);
            }
        }

        public void Deselect()
        {
            SelectedObject = null;
            _container.style.display = DisplayStyle.None;
            Clear();
        }

        private void AddNewComponent(Type type)
        {
            var newComponent = (IRPGComponent)Activator.CreateInstance(type);
            SelectedObject.AddComponent(newComponent);
            
            OnSelectedObjectUpdated.Invoke();
        }

        private void OnNewComponentButtonPressed()
        {
            if (_isAddingComponent) return;

            _isAddingComponent = true;
            
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
            _bottomBox.Add(popupField);

            var button = new Button();
            button.clicked += () =>
            {
                var type = typeList[popupField.index];
                AddNewComponent(type);
                _isAddingComponent = false;

                button?.RemoveFromHierarchy();
                popupField?.RemoveFromHierarchy();
            };
            
            button.text = "Add";
            _bottomBox.Add(button);
        }
    }
}