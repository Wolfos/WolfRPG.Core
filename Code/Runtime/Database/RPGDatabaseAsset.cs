﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WolfRPG.Core
{
	[Serializable]
	public class RPGDatabaseAsset
	{
		public const string LabelDefault = "WolfRPG Default Database";
		public const string Label = "WolfRPG Database";

		// List of GUIDs that point to the files for RPGObjects
		public List<string> ObjectReferences { get; set; }
		public List<string> Categories { get; set; }
		public int NumObjectsAdded { get; set; }

		public IRPGDatabase Get()
		{
			var database = new RPGDatabase();
			foreach (var guid in ObjectReferences)
			{
				var operation = Addressables.LoadAssetAsync<TextAsset>(guid);
				var asset = operation.WaitForCompletion();
				Addressables.Release(operation);

				try
				{
					var rpgObject = JsonConvert.DeserializeObject<RPGObject>(asset.text, Settings.JsonSerializerSettings);
					database.AddObjectInstance(rpgObject);
				}
				catch(Exception e)
				{
					Debug.LogError($"Could not load object {asset.name}, error: {e.Message}");
				}
			}

			database.Categories = Categories;
			database.NumObjectsAdded = NumObjectsAdded;
			return database;
		}

		public void CreateFrom(IRPGDatabase database)
		{
			ObjectReferences = new();
			foreach (var obj in database.Objects)
			{
				ObjectReferences.Add(obj.Key);
			}

			Categories = database.Categories;
			NumObjectsAdded = database.NumObjectsAdded;
		}
	}
}