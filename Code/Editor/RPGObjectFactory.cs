using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace WolfRPG.Core
{
	public class RPGObjectFactory: IRPGObjectFactory
	{
		public static string DefaultRelativePath = "WolfRPG/Objects";
		public string DefaultPath = $"{Application.dataPath}/{DefaultRelativePath}";
		
		public IRPGObject CreateNewObject(string name)
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
				Name = name
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
			var assetGroup = addressableSettings.FindGroup("WolfRPG");
			if (assetGroup == null)
			{
				assetGroup = addressableSettings.CreateGroup("WolfRPG", false, false, false,
					new() {addressableSettings.DefaultGroup.Schemas[0]});
			}

			var entry = addressableSettings.CreateOrMoveEntry(guid.ToString(), assetGroup);
			addressableSettings.AddLabel(RPGObject.Label);
			entry.labels.Add(RPGObject.Label);

			return newObject;
		}
	}
}