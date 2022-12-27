using System;
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
		public List<KeyValuePair<string, Type>> ObjectReferences { get; set; }

		public IRPGDatabase Get()
		{
			var database = new RPGDatabase();
			foreach (var obj in ObjectReferences)
			{
				var operation = Addressables.LoadAssetAsync<TextAsset>(obj.Key);
				var asset = operation.WaitForCompletion();

				var rpgObject = JsonConvert.DeserializeObject(asset.text, obj.Value);
				database.AddObjectInstance(rpgObject as IRPGObject);
			}

			return database;
		}

		public void Create(RPGDatabase database)
		{
			foreach (var obj in database.Objects)
			{
				ObjectReferences.Add(new (obj.Key, ));
			}
		}
	}
}