﻿using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WolfRPG.Core
{
	public class RPGDatabaseFactory: IRPGDatabaseFactory
	{
		public static string DefaultRelativeDatabasePath = "WolfRPG/DefaultDatabase.json";
		public string DefaultDatabasePath = $"{Application.dataPath}/{DefaultRelativeDatabasePath}";

		public IRPGDatabase CreateNewDatabase(out TextAsset asset)
		{
			if (File.Exists(DefaultDatabasePath))
			{
				Debug.LogWarning($"File {DefaultDatabasePath} already exists");
				asset = null;
				return null;
			}

			if (AssetDatabase.IsValidFolder("Assets/WolfRPG") == false)
			{
				AssetDatabase.CreateFolder("Assets", "WolfRPG");
			}

			var newDatabase = new RPGDatabase();
			var dbAsset = new RPGDatabaseAsset();
			dbAsset.CreateFrom(newDatabase);

			var json = JsonConvert.SerializeObject(dbAsset, Formatting.Indented);
			File.WriteAllText(DefaultDatabasePath, json);

			AssetDatabase.Refresh();
			
			var guid = AssetDatabase.GUIDFromAssetPath($"Assets/{DefaultRelativeDatabasePath}");
			asset = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/{DefaultRelativeDatabasePath}");
			
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
			addressableSettings.AddLabel(RPGDatabaseAsset.LabelDefault);
			addressableSettings.AddLabel(RPGDatabaseAsset.Label);
			entry.labels.Add(RPGDatabaseAsset.LabelDefault);
			entry.labels.Add(RPGDatabaseAsset.Label);
			
			return newDatabase;
		}

		public IRPGDatabase GetDefaultDatabase(out TextAsset asset)
		{
			// First check if asset exists
			var operation = Addressables.LoadResourceLocationsAsync(RPGDatabaseAsset.LabelDefault);
			var x = operation.WaitForCompletion();
			if (x.Count == 0)
			{
				Debug.LogWarning($"No default database found");
				asset = null;
				return null;
			}

			var loadOperation = Addressables.LoadAssetAsync<TextAsset>(RPGDatabaseAsset.LabelDefault);
			asset = loadOperation.WaitForCompletion();
			Addressables.Release(loadOperation);
				
			var databaseAsset = JsonConvert.DeserializeObject<RPGDatabaseAsset>(asset.text);
			var database = databaseAsset.Get();
			RPGDatabase.DefaultDatabase = database;
				
			return database;
		}

		public void SaveDatabase(IRPGDatabase database, string path)
		{
			var databaseAsset = new RPGDatabaseAsset();
			databaseAsset.CreateFrom(database);

			var json = JsonConvert.SerializeObject(databaseAsset);
			File.WriteAllText(path, json);
			
			AssetDatabase.Refresh();
		}

		public void SaveDefaultDatabase()
		{
			var operation = Addressables.LoadAssetAsync<TextAsset>(RPGDatabaseAsset.LabelDefault);
			var asset = operation.WaitForCompletion();
			Addressables.Release(operation);
			
			var relativePath = AssetDatabase.GetAssetPath(asset);
			var absolutePath = $"{Path.GetDirectoryName(Application.dataPath)}/{relativePath}";

			SaveDatabase(RPGDatabase.DefaultDatabase, absolutePath);
		}
	}
}