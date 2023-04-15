﻿using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using WolfRPG.Core.Localization;
using Object = UnityEngine.Object;

namespace WolfRPG.Core
{
	public class PropertyEditor: VisualElement
	{
		public PropertyEditor(PropertyInfo property, IRPGComponent component, ComponentEditor editor)
		{
			var propertyType = property.PropertyType;
			// int
			if (propertyType == typeof(int))
			{
				var field = new IntegerField();
				field.label = property.Name;
				field.value = (int)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) =>
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// bool
			else if (propertyType == typeof(bool))
			{
				var field = new Toggle();
				field.label = property.Name;
				field.value = (bool)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) =>
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// float
			else if (propertyType == typeof(float))
			{
				var field = new FloatField();
				field.label = property.Name;
				field.value = (float)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) => 
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// long
			else if (propertyType == typeof(long))
			{
				var field = new LongField();
				field.label = property.Name;
				field.value = (long)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) => 
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// String
			else if (propertyType == typeof(string))
			{
				var field = new TextField();
				field.label = property.Name;
				field.value = (string)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) => 
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Vector2
			else if (propertyType == typeof(Vector2))
			{
				var field = new Vector2Field();
				field.label = property.Name;
				field.value = (Vector2)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) => 
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Vector3
			else if (propertyType == typeof(Vector3))
			{
				var field = new Vector3Field();
				field.label = property.Name;
				field.value = (Vector3)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) => 
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Vector4
			else if (propertyType == typeof(Vector4))
			{
				var field = new Vector4Field();
				field.label = property.Name;
				field.value = (Vector4)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) => 
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Vector2Int
			else if (propertyType == typeof(Vector2Int))
			{
				var field = new Vector2IntField();
				field.label = property.Name;
				field.value = (Vector2Int)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) => 
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Vector3Int
			else if (propertyType == typeof(Vector3Int))
			{
				var field = new Vector3IntField();
				field.label = property.Name;
				field.value = (Vector3Int)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) => 
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Rect
			else if (propertyType == typeof(Rect))
			{
				var field = new RectField();
				field.label = property.Name;
				field.value = (Rect)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) => 
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Bounds
			else if (propertyType == typeof(Bounds))
			{
				var field = new BoundsField();
				field.label = property.Name;
				field.value = (Bounds)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) => 
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Color
			else if (propertyType == typeof(Color))
			{
				var field = new ColorField();
				field.label = property.Name;
				field.value = (Color)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) => 
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// AnimationCurve
			else if (propertyType == typeof(AnimationCurve))
			{
				var field = new CurveField();
				field.label = property.Name;
				field.value = (AnimationCurve)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) => 
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Gradient
			else if (propertyType == typeof(Gradient))
			{
				var field = new GradientField();
				field.label = property.Name;
				field.value = (Gradient)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) =>
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Enum
			else if (propertyType.IsEnum)
			{
				var field = new EnumField((Enum)property.GetValue(component));
				field.label = property.Name;
				field.RegisterValueChangedCallback((evt) => 
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Enum
			else if (propertyType == typeof(LayerMask))
			{
				var field = new LayerMaskField();
				field.label = property.Name;
				field.value = (LayerMask)property.GetValue(component);
				field.RegisterValueChangedCallback((evt) => 
				{
					property.SetValue(component, evt.newValue);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// AssetReference
			else if (propertyType == typeof(AssetReference))
			{
				// AssetReferenceAttribute is required to set the type
				var attributes = property.GetCustomAttributes(typeof(AssetReferenceAttribute), true);
				if (attributes.Any())
				{
					var attribute = (AssetReferenceAttribute)attributes.First();
					var field = new ObjectField();
					field.label = property.Name;
					var reference = (AssetReference)property.GetValue(component);
					if (reference != null && reference.Guid != String.Empty)
					{
						field.value = reference.GetAsset<Object>();
					}

					field.objectType = attribute.Type;
					field.RegisterValueChangedCallback((evt) =>
					{
						var obj = evt.newValue;
						if (obj == null) return;
						
						var path = AssetDatabase.GetAssetPath(obj);
						var guid = AssetDatabase.GUIDFromAssetPath(path).ToString();

						var settings = AddressableAssetSettingsDefaultObject.Settings;
						var entry = settings.FindAssetEntry(guid);

						if (entry == null)
						{
							DatabaseEditor.DisplayMessage("Asset needs to be addressable");
							field.SetValueWithoutNotify(null);
							return;
						}
						
						var reference = new AssetReference
						{
							Guid = guid
						};
						property.SetValue(component, reference);
						
						editor.OnComponentUpdated?.Invoke();
					});
					
					this.Add(field);
				}
			}
			// LocalizedString
			else if (propertyType == typeof(LocalizedString))
			{
				var field = new TextField();
				field.label = property.Name;
				var reference = (LocalizedString) property.GetValue(component);
				if (reference != null)
				{
					field.value = reference.Identifier;
				}

				field.RegisterValueChangedCallback((evt) =>
				{
					var newString = new LocalizedString
					{
						Identifier = evt.newValue
					};
					property.SetValue(component, newString);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
				
				if (string.IsNullOrEmpty(field.value) == false && LocalizationFile.HasIdentifier(field.value))
				{
					var localized = LocalizationFile.Get(field.value, LocalizationFile.DefaultLanguage);
					var label = new Label($"Text: {localized}");
					this.Add(label);
				}
				else
				{
					var label = new Label("Text: Not found");
					this.Add(label);
				}
			}
		}
		public PropertyEditor(Type propertyType, Array array, int index, ComponentEditor editor)
		{
			var value = array.GetValue(index);
			// int
			if (propertyType == typeof(int))
			{
				var field = new IntegerField();
				field.value = (int)value;
				field.RegisterValueChangedCallback((evt) =>
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// bool
			else if (propertyType == typeof(bool))
			{
				var field = new Toggle();
				field.value = (bool)value;
				field.RegisterValueChangedCallback((evt) =>
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// float
			else if (propertyType == typeof(float))
			{
				var field = new FloatField();
				field.value = (float)value;
				field.RegisterValueChangedCallback((evt) => 
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// long
			else if (propertyType == typeof(long))
			{
				var field = new LongField();
				field.value = (long)value;
				field.RegisterValueChangedCallback((evt) => 
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// String
			else if (propertyType == typeof(string))
			{
				var field = new TextField();
				field.value = (string)value;
				field.RegisterValueChangedCallback((evt) => 
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Vector2
			else if (propertyType == typeof(Vector2))
			{
				var field = new Vector2Field();
				field.value = (Vector2)value;
				field.RegisterValueChangedCallback((evt) => 
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Vector3
			else if (propertyType == typeof(Vector3))
			{
				var field = new Vector3Field();
				field.value = (Vector3)value;
				field.RegisterValueChangedCallback((evt) => 
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Vector4
			else if (propertyType == typeof(Vector4))
			{
				var field = new Vector4Field();
				field.value = (Vector4)value;
				field.RegisterValueChangedCallback((evt) => 
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Vector2Int
			else if (propertyType == typeof(Vector2Int))
			{
				var field = new Vector2IntField();
				field.value = (Vector2Int)value;
				field.RegisterValueChangedCallback((evt) => 
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Vector3Int
			else if (propertyType == typeof(Vector3Int))
			{
				var field = new Vector3IntField();
				field.value = (Vector3Int)value;
				field.RegisterValueChangedCallback((evt) => 
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Rect
			else if (propertyType == typeof(Rect))
			{
				var field = new RectField();
				field.value = (Rect)value;
				field.RegisterValueChangedCallback((evt) => 
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Bounds
			else if (propertyType == typeof(Bounds))
			{
				var field = new BoundsField();
				field.value = (Bounds)value;
				field.RegisterValueChangedCallback((evt) => 
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Color
			else if (propertyType == typeof(Color))
			{
				var field = new ColorField();
				field.value = (Color)value;
				field.RegisterValueChangedCallback((evt) => 
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// AnimationCurve
			else if (propertyType == typeof(AnimationCurve))
			{
				var field = new CurveField();
				field.value = (AnimationCurve)value;
				field.RegisterValueChangedCallback((evt) => 
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Gradient
			else if (propertyType == typeof(Gradient))
			{
				var field = new GradientField();
				field.value = (Gradient)value;
				field.RegisterValueChangedCallback((evt) =>
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Enum
			else if (propertyType.IsEnum)
			{
				var field = new EnumField((Enum)value);
				field.RegisterValueChangedCallback((evt) => 
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// Enum
			else if (propertyType == typeof(LayerMask))
			{
				var field = new LayerMaskField();
				field.value = (LayerMask)value;
				field.RegisterValueChangedCallback((evt) => 
				{
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
			}
			// AssetReference
			else if (propertyType == typeof(AssetReference))
			{
				// AssetReferenceAttribute is required to set the type
				// var attributes = property.GetCustomAttributes(typeof(AssetReferenceAttribute), true);
				// if (attributes.Any())
				// {
				// 	var attribute = (AssetReferenceAttribute)attributes.First();
				// 	var field = new ObjectField();
				// 	var reference = (AssetReference)value;
				// 	if (reference != null && reference.Guid != String.Empty)
				// 	{
				// 		field.value = reference.GetAsset<Object>();
				// 	}
				//
				// 	field.objectType = attribute.Type;
				// 	field.RegisterValueChangedCallback((evt) =>
				// 	{
				// 		var obj = evt.newValue;
				// 		if (obj == null) return;
				// 		
				// 		var path = AssetDatabase.GetAssetPath(obj);
				// 		var guid = AssetDatabase.GUIDFromAssetPath(path).ToString();
				//
				// 		var settings = AddressableAssetSettingsDefaultObject.Settings;
				// 		var entry = settings.FindAssetEntry(guid);
				//
				// 		if (entry == null)
				// 		{
				// 			DatabaseEditor.DisplayMessage("Asset needs to be addressable");
				// 			field.SetValueWithoutNotify(null);
				// 			return;
				// 		}
				// 		
				// 		var reference = new AssetReference
				// 		{
				// 			Guid = guid
				// 		};
				// 		value = evt.newValue;
				// 		
				// 		editor.OnComponentUpdated?.Invoke();
				// 	});
				// 	
				// 	this.Add(field);
				// }
			}
			// LocalizedString
			else if (propertyType == typeof(LocalizedString))
			{
				var field = new TextField();
				var reference = (LocalizedString) value;
				if (reference != null)
				{
					field.value = reference.Identifier;
				}

				field.RegisterValueChangedCallback((evt) =>
				{
					var newString = new LocalizedString
					{
						Identifier = evt.newValue
					};
					array.SetValue(evt.newValue, index);
					editor.OnComponentUpdated?.Invoke();
				});
				this.Add(field);
				
				if (string.IsNullOrEmpty(field.value) == false && LocalizationFile.HasIdentifier(field.value))
				{
					var localized = LocalizationFile.Get(field.value, LocalizationFile.DefaultLanguage);
					var label = new Label($"Text: {localized}");
					this.Add(label);
				}
				else
				{
					var label = new Label("Text: Not found");
					this.Add(label);
				}
			}
		}
	}
}