using UnityEditor;
using UnityEngine;

namespace WolfRPG.Core.Localization
{
	[CustomPropertyDrawer(typeof(LocalizedString))]
	public class LocalizedStringPropertyDrawer: PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			var identifierRect = new Rect(position.x, position.y, position.width * 0.75f, position.height);
			
			var identifierField = property.FindPropertyRelative("Identifier");
			EditorGUI.PropertyField(identifierRect, identifierField, label);
			var identifier = identifierField.stringValue;
			var localized = "";
			if (string.IsNullOrEmpty(identifier) == false && LocalizationFile.HasIdentifier(identifier))
			{
				localized = LocalizationFile.Get(identifier, LocalizationFile.DefaultLanguage);
			}
			
			var localizedRect = new Rect(position.x + position.width * 0.75f + 2, position.y, position.width * 0.25f, position.height);
			EditorGUI.LabelField(localizedRect, localized);
			EditorGUI.EndProperty();
		}
	}
}