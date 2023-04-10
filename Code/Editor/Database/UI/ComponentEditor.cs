using System;
using System.Linq;
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
					var foldout = new Foldout();
					foldout.text = property.Name;
					
					var array = (Array) property.GetValue(component) ?? Array.CreateInstance(propertyType.GetElementType(), 0);
					
					var button = new Button(() =>
					{
						Resize(ref array, array.Length + 1);
						property.SetValue(component, array);
						OnComponentUpdated?.Invoke();

						var propertyEditor =
							new PropertyEditor(propertyType.GetElementType(), array, array.Length - 1, this);
						foldout.Add(propertyEditor);
					});
					button.text = "+";
					
					foldout.Add(button);

					for (var i = 0; i < array.Length; i++)
					{
						var index = i;
						var propertyEditor =
							new PropertyEditor(propertyType.GetElementType(), array, index, this);
						foldout.Add(propertyEditor);
					}

					
					
					Add(foldout);
				}
				else
				{
					var editor = new PropertyEditor(property, component, this);
					Add(editor);
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