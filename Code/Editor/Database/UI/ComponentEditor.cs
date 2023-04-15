using System;
using UnityEngine.UIElements;

namespace WolfRPG.Core
{
	public class ComponentEditor: Foldout
	{
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
					Add(new ArrayEditor(component, property, propertyType, this));
				}
				else
				{
					var editor = new PropertyEditor(property, component, this);
					Add(editor);
				}
			}
		}
	}
}