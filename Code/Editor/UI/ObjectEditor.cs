using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace WolfRPG.Core
{
    public class ObjectEditor
    {
        public Action SelectedObjectUpdated { get; set; }
        public RPGObject SelectedObject { get; private set; }
        private TextField _nameField;
        private TemplateContainer _container;
        
        public void SelectObject(RPGObject rpgObject)
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
        
        public VisualElement GetUI()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Packages/nl.eestudio.wolfrpg.core/Code/Editor/UI/ObjectEditor.uxml");
            _container = visualTree.Instantiate();
            _container.style.display = DisplayStyle.None;
            _nameField = _container.Query<TextField>("NameField").First();

            return _container;
        }
    }
}