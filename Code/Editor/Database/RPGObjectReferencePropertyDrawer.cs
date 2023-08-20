using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace WolfRPG.Core
{
	[CustomPropertyDrawer(typeof(RPGObjectReference))]
	public class RPGObjectReferencePropertyDrawer: PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			var guidRect = new Rect(position.x, position.y, position.width * 0.75f, position.height);
			var nameRect = new Rect(position.x + position.width * 0.75f + 2, position.y, position.width * 0.25f, position.height);
			var guidField = property.FindPropertyRelative("Guid");
			EditorGUI.PropertyField(guidRect, guidField, label);
			var guid = guidField.stringValue;
			string objectName = "";
			if (string.IsNullOrEmpty(guid) == false)
			{
				var rpgObject = RPGDatabase.GetObject(guid);
				if (rpgObject != null)
				{
					objectName = rpgObject.Name;
				}
				else
				{
					objectName = "Object not found";
				}
			}
			
			EditorGUI.LabelField(nameRect, objectName);
			EditorGUI.EndProperty();
		}
	}

	[CustomPropertyDrawer(typeof(ObjectReferenceAttribute))]
	public class ObjectReferenceAttributePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var refAttribute = (ObjectReferenceAttribute) attribute;
			var guidField = property.FindPropertyRelative("Guid");

			var database = RPGDatabase.DefaultDatabase;
			var names = new List<string>();
			var guids = new List<string>();
			names.Add("NONE");
			guids.Add(string.Empty);

			foreach (var obj in database.Objects)
			{
				if(obj.Value.Category != refAttribute.Category) continue;
				
				names.Add(obj.Value.Name);
				guids.Add(obj.Value.Guid);
			}

			int index = 0;
			if (guidField.stringValue != string.Empty)
			{
				index = guids.IndexOf(guidField.stringValue);
			}

			EditorGUI.LabelField(position, label);
			var newPosition = position;
			newPosition.x += position.width * 0.4f;
			newPosition.width = position.width * 0.6f;
			index = EditorGUI.Popup(newPosition, index, names.ToArray());
			guidField.stringValue = guids[index];
		}
	}
}