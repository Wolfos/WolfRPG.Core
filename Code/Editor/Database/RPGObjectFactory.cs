using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WolfRPG.Core
{
	public class RPGObjectFactory: IRPGObjectFactory
	{
		public static string DefaultRelativePath = "WolfRPG/Objects";
		public string DefaultPath = $"{Application.dataPath}/{DefaultRelativePath}";
		
		public IRPGObject CreateNewObject(string name, int category)
		{
			// Create folders
			if (AssetDatabase.IsValidFolder("Assets/WolfRPG") == false)
			{
				AssetDatabase.CreateFolder("Assets", "WolfRPG");
			}
			
			if (AssetDatabase.IsValidFolder("Assets/WolfRPG/Objects") == false)
			{
				AssetDatabase.CreateFolder("Assets/WolfRPG", "Objects");
			}

			if (File.Exists($"{DefaultPath}/{name}.json"))
			{
				Debug.LogError($"Object with name {name} already exists");
				return null;
			}

			var newObject = new RPGObject
			{
				Name = name,
				Category = category
			};

			var json = JsonConvert.SerializeObject(newObject, Formatting.Indented);
			File.WriteAllText($"{DefaultPath}/{name}.json", json);
			
			AssetDatabase.Refresh();
			
			// Set GUID and re-serialize
			var guid = AssetDatabase.GUIDFromAssetPath($"Assets/{DefaultRelativePath}/{name}.json");
			newObject.Guid = guid.ToString();
			json = JsonConvert.SerializeObject(newObject, Formatting.Indented);
			File.WriteAllText($"{DefaultPath}/{name}.json", json);
			
			// Add to addressables
			var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
			var assetGroup = addressableSettings.DefaultGroup;
			
			// This was a nice idea, but new groups don't build by default. Might revisit this in the future.
			// if (assetGroup == null)
			// {
			// 	assetGroup = addressableSettings.CreateGroup("WolfRPG", false, false, false,
			// 		new() {addressableSettings.DefaultGroup.Schemas[0]});
			// }

			var entry = addressableSettings.CreateOrMoveEntry(guid.ToString(), assetGroup);
			addressableSettings.AddLabel(RPGObject.Label);
			entry.labels.Add(RPGObject.Label);

			return newObject;
		}

		public void SaveObject(IRPGObject rpgObject)
		{
			var path = $"Assets/{DefaultRelativePath}/{rpgObject.Name}.json";
			var oldPath = AssetDatabase.GUIDToAssetPath(rpgObject.Guid);
			path = path.Replace("\\", "/");
			oldPath = oldPath.Replace("\\", "/");
			if (path != oldPath)
			{
				AssetDatabase.MoveAsset(oldPath, path);
			}
			var json = JsonConvert.SerializeObject(rpgObject, Formatting.Indented, 
				Settings.JsonSerializerSettings);
			File.WriteAllText($"{DefaultPath}/{rpgObject.Name}.json", json);
			
			AssetDatabase.Refresh();
		}

		public void DeleteObject(IRPGObject rpgObject)
		{
			RPGDatabase.DefaultDatabase.RemoveObjectInstance(rpgObject);
			
			var path = AssetDatabase.GUIDToAssetPath(rpgObject.Guid);
			AssetDatabase.DeleteAsset(path);
		}
	}
}