using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace WolfRPG.Core.Localization
{
	public static class CreateLocalizationFile
	{
		public static string DefaultRelativePath = "WolfRPG/Localization.csv";
		public static string DefaultPath = $"{Application.dataPath}/{DefaultRelativePath}";
		
		
		
		[MenuItem("WolfRPG/Create Localization File")]
		public static void CreateFile()
		{
			if (File.Exists(DefaultPath))
			{
				Debug.LogWarning($"File {DefaultPath} already exists");
				return;
			}
			
			if (AddressableAssetSettingsDefaultObject.SettingsExists == false)
			{
				Debug.LogError("Addressables has not been setup for this project. Please create the addressables settings from the Addressables Groups window and retry");
				return;
			}
			
			const string text = "Identifier;English;Dutch\nExampleID;Example;Voorbeeld";
			File.WriteAllText(DefaultPath, text);
			AssetDatabase.Refresh();
			var guid = AssetDatabase.GUIDFromAssetPath($"Assets/{DefaultRelativePath}");

			// Add to addressables
			var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
			var assetGroup = addressableSettings.DefaultGroup;
			
			var entry = addressableSettings.CreateOrMoveEntry(guid.ToString(), assetGroup);
			addressableSettings.AddLabel(LocalizationFile.Label);
			entry.labels.Add(LocalizationFile.Label);
		}
	}
}