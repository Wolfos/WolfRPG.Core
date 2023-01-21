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
}