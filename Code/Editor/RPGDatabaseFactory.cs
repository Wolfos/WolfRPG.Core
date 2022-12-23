using System;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace WolfRPG.Core
{
	public class RPGDatabaseFactory: IRPGDatabaseFactory
	{
		public static string DefaultRelativeDatabasePath = "WolfRPG/DefaultDatabase.json";
		public string DefaultDatabasePath = $"{Application.dataPath}/{DefaultRelativeDatabasePath}";

		public IRPGDatabase CreateNewDatabase(out Nullable<GUID> guid)
		{
			if (File.Exists(DefaultDatabasePath))
			{
				Debug.LogWarning($"File {DefaultDatabasePath} already exists");
				guid = null;
				return null;
			}

			if (AssetDatabase.IsValidFolder("Assets/WolfRPG") == false)
			{
				AssetDatabase.CreateFolder("Assets", "WolfRPG");
			}

			var newDatabase = new RPGDatabase();

			var json = JsonConvert.SerializeObject(newDatabase, Formatting.Indented);
			File.WriteAllText(DefaultDatabasePath, json);

			AssetDatabase.Refresh();

			guid = AssetDatabase.GUIDFromAssetPath($"Assets/{DefaultRelativeDatabasePath}");
			return newDatabase;
		}

		public IRPGDatabase GetDefaultDatabase(out TextAsset asset)
		{
			if (File.Exists(DefaultDatabasePath) == false)
			{
				Debug.LogWarning($"File {DefaultDatabasePath} doesn't exist");
				asset = null;
				return null;
			}

			asset = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/{DefaultRelativeDatabasePath}");
			return JsonConvert.DeserializeObject<RPGDatabase>(asset.text);
		}
	}
}