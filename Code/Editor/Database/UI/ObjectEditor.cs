using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace WolfRPG.Core
{
    public class ObjectEditor
    {
        public Action OnBeforeSelectedObjectUpdated { get; set; }
        public Action OnSelectedObjectUpdated { get; set; }
        public IRPGObject SelectedObject { get; private set; }

        private IRPGDatabase _database;
        private TextField _nameField;
        private TextField _guidField;
        private Toggle _includedInSavedGameToggle;
        private DropdownField _category;
        private TemplateContainer _container;
        private GroupBox _bottomBox; // box that's located below the "add component" button. Gets cleared when object is deselected
        private GroupBox _objectList;
        private bool _isAddingComponent;

        public VisualElement CreateUI(IRPGDatabase database)
        {
            _database = database;
            
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Packages/nl.eestudio.wolfrpg.core/Code/Editor/Database/UI/ObjectEditor.uxml");
            _container = visualTree.Instantiate();
            _container.style.display = DisplayStyle.None;
            _nameField = _container.Query<TextField>("NameField").First();
            _guidField = _container.Query<TextField>("GuidField").First();
            _includedInSavedGameToggle = _container.Query<Toggle>("IncludedInSavedGame").First();
            _objectList = _container.Query<GroupBox>("ObjectList").First();
            _category = _container.Query<DropdownField>("Category").First();
            _category.choices = database.Categories;

            var newComponentButton = _container.Query<Button>("NewComponentButton").First();
            newComponentButton.clicked += OnNewComponentButtonPressed;

            _bottomBox = new();
            _container.Add(_bottomBox);

            return _container;
        }
        
        public void SelectObject(IRPGObject rpgObject)
        {
            Clear();
            ClearCallbacks();
            
            SelectedObject = rpgObject;

            _container.style.display = DisplayStyle.Flex;
            _nameField.value = SelectedObject.Name;
            
            _nameField.RegisterValueChangedCallback(OnNameFieldChanged);
            _category.RegisterValueChangedCallback(OnCategoryChanged);

            _guidField.value = SelectedObject.Guid;
            _guidField.RegisterCallback<ClickEvent>( OnGuidFieldClicked);

            _includedInSavedGameToggle.value = SelectedObject.IncludedInSavedGame;
            _includedInSavedGameToggle.RegisterValueChangedCallback(OnIncludedOnSaveGameToggleChanged);
            _category.index = rpgObject.Category;
            
            BuildComponentsList();
        }
        
        public void DeselectObject()
        {
            SelectedObject = null;
            _container.style.display = DisplayStyle.None;
            Clear();
        }

        #region Callbacks
        private void OnNameFieldChanged(ChangeEvent<string> changeEvent)
        {
            if (changeEvent.newValue == SelectedObject.Name) return;

            // Because object name == file name, duplicates are not allowed
            var hasObject = _database.GetObjectByName(changeEvent.newValue);
            if (hasObject != null && hasObject != SelectedObject)
            {
                DatabaseEditor.DisplayMessage("Name already exists");
                return;
            }

            OnBeforeSelectedObjectUpdated?.Invoke();
            SelectedObject.Name = changeEvent.newValue;
            OnSelectedObjectUpdated?.Invoke();
        }
        
        private void OnCategoryChanged(ChangeEvent<string> changeEvent)
        {
            var category = _database.Categories.IndexOf(changeEvent.newValue);
            if (category == SelectedObject.Category) return;
            
            OnBeforeSelectedObjectUpdated?.Invoke();
            SelectedObject.Category = category;
            OnSelectedObjectUpdated?.Invoke();
        }

        private void OnGuidFieldClicked(ClickEvent evt)
        {
            GUIUtility.systemCopyBuffer = SelectedObject.Guid;
            DatabaseEditor.DisplayMessage("Copied to clipboard");
        }

        private void OnIncludedOnSaveGameToggleChanged(ChangeEvent<bool> changeEvent)
        {
            if (changeEvent.newValue == SelectedObject.IncludedInSavedGame) return;

            OnBeforeSelectedObjectUpdated?.Invoke();
            SelectedObject.IncludedInSavedGame = changeEvent.newValue;
            OnSelectedObjectUpdated?.Invoke();
        }
        #endregion

        private void ClearCallbacks()
        {
            _nameField.UnregisterValueChangedCallback(OnNameFieldChanged);
            _category.UnregisterValueChangedCallback(OnCategoryChanged);
            _guidField.UnregisterCallback<ClickEvent>(OnGuidFieldClicked);
            _includedInSavedGameToggle.UnregisterValueChangedCallback(OnIncludedOnSaveGameToggleChanged); 
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
                var deleteButton = new Button(() => RemoveComponent(component));
                deleteButton.name = "DeleteButton";
                deleteButton.text = "x";
                _objectList.Add(deleteButton);
                
                var componentEditor = new ComponentEditor(component);
                _objectList.Add(componentEditor);
                componentEditor.OnBeforeComponentUpdated += OnBeforeSelectedObjectUpdated;
                componentEditor.OnComponentUpdated += OnSelectedObjectUpdated;
            }
        }

        private void RemoveComponent(IRPGComponent component)
        {
            OnBeforeSelectedObjectUpdated?.Invoke();
            SelectedObject.RemoveComponent(component);
            OnSelectedObjectUpdated?.Invoke();
            
            BuildComponentsList();
        }

        private void AddNewComponent(Type type)
        {
            OnBeforeSelectedObjectUpdated?.Invoke();
            var newComponent = (IRPGComponent)Activator.CreateInstance(type);
            SelectedObject.AddComponent(newComponent);
            
            OnSelectedObjectUpdated.Invoke();
            BuildComponentsList();
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