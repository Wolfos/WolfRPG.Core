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

		public IRPGDatabase Get()
		{
			var database = new RPGDatabase();
			foreach (var guid in ObjectReferences)
			{
				var operation = Addressables.LoadAssetAsync<TextAsset>(guid);
				var asset = operation.WaitForCompletion();

				var rpgObject = JsonConvert.DeserializeObject<RPGObject>(asset.text);
				database.AddObjectInstance(rpgObject);
			}

			return database;
		}

		public void CreateFrom(RPGDatabase database)
		{
			ObjectReferences = new();
			foreach (var obj in database.Objects)
			{
				ObjectReferences.Add(obj.Key);
			}
		}
	}
}