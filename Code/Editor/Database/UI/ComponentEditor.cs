using System;
using UnityEngine.UIElements;

namespace WolfRPG.Core
{
	public class ComponentEditor: Foldout
	{
		public Action OnBeforeComponentUpdated { get; set; }
		public Action OnComponentUpdated { get; set; }
		
		public ComponentEditor(IRPGComponent component) : base()
		{
			var type = component.GetType();
			text = type.Name;

			foreach (var property in type.GetProperties())
			{
				var propertyType = property.PropertyType;
				if (propertyType.IsArray)
				{
					var arrayEditor = new ArrayEditor(component, property, propertyType, this);
					arrayEditor.AddToClassList("Array");
					Add(arrayEditor);
				}
				else
				{
					var propertyEditor = new PropertyEditor(property, component, this);
					if (propertyEditor.Hidden == false)
					{
						propertyEditor.AddToClassList("Property");
						Add(propertyEditor);
					}
				}
			}
		}
	}
}