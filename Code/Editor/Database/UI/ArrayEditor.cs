using System;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace WolfRPG.Core
{
	public class ArrayEditor: VisualElement
	{
		private GroupBox _propertyEditors;
		public ArrayEditor(object component, PropertyInfo property, Type propertyType, ComponentEditor componentEditor)
		{
			// Don't show if property has JsonIgnore or HideInInspector attribute
			if (property.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length > 0
			    || property.GetCustomAttributes(typeof(HideInInspector), true).Length > 0)
			{
				return;
			}
			
			var foldout = new Foldout();
			foldout.AddToClassList("ArrayFoldout");
			foldout.text = property.Name;
			foldout.SetValueWithoutNotify(false); // Close the foldout by default

			var array = (Array) property.GetValue(component) ?? Array.CreateInstance(propertyType.GetElementType(), 0);

			var buttonGroup = new GroupBox();
			buttonGroup.AddToClassList("ArrayButtons");
			
			foldout.Add(buttonGroup);

			var length = new Label(array.Length.ToString());
			length.style.unityTextAlign = new(TextAnchor.MiddleCenter);

			// Add new element
			var addButton = new Button(() =>
			{
				componentEditor.OnBeforeComponentUpdated?.Invoke();
				Resize(ref array, array.Length + 1);
				property.SetValue(component, array);
				componentEditor.OnComponentUpdated?.Invoke();

				length.text = array.Length.ToString();

				RefreshPropertyEditors(propertyType, componentEditor, array);

			});
			addButton.text = "+";
			addButton.AddToClassList("ArrayButton");
			
			// Remove last element
			var removeButton = new Button(() =>
			{
				componentEditor.OnBeforeComponentUpdated?.Invoke();
				Resize(ref array, array.Length - 1);
				property.SetValue(component, array);
				componentEditor.OnComponentUpdated?.Invoke();
				
				length.text = array.Length.ToString();
				
				RefreshPropertyEditors(propertyType, componentEditor, array);
			});
			removeButton.text = "-";
			removeButton.AddToClassList("ArrayButton");
			
			buttonGroup.Add(removeButton);
			buttonGroup.Add(length);
			buttonGroup.Add(addButton);


			_propertyEditors = new();
			_propertyEditors.AddToClassList("ArrayElementContainer");
			foldout.Add(_propertyEditors);

			RefreshPropertyEditors(propertyType, componentEditor, array);
			
			Add(foldout);
		}

		private void RefreshPropertyEditors(Type propertyType, ComponentEditor componentEditor, Array array)
		{
			_propertyEditors.Clear();
			
			for (var i = 0; i < array.Length; i++)
			{
				var index = i;
				var propertyEditor =
					new PropertyEditor(propertyType.GetElementType(), array, index, componentEditor);
				propertyEditor.AddToClassList("ArrayElement");
				if (propertyEditor.Hidden == false)
				{
					_propertyEditors.Add(propertyEditor);
				}
			}
		}

		private static void Resize(ref Array array, int newSize) {        
			var elementType = array.GetType().GetElementType();
			var newArray = Array.CreateInstance(elementType, newSize);
			Array.Copy(array, newArray, Math.Min(array.Length, newArray.Length));
			array = newArray;
		}
	}
}