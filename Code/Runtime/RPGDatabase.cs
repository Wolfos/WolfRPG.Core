using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WolfRPG.Core
{
	public class RPGDatabase: IRPGDatabase
	{
		private static IRPGDatabase _defaultDatabase;

		public static IRPGDatabase DefaultDatabase
		{
			get
			{
				if (_defaultDatabase == null)
				{
					var operation = Addressables.LoadResourceLocationsAsync(RPGDatabaseAsset.LabelDefault);
					var x = operation.WaitForCompletion();
					if (x.Count == 0)
					{
						Debug.LogWarning($"No default database found");
						return null;
					}

					var loadOperation = Addressables.LoadAssetAsync<TextAsset>(RPGDatabaseAsset.LabelDefault);
					var asset = loadOperation.WaitForCompletion();
				
					var databaseAsset = JsonConvert.DeserializeObject<RPGDatabaseAsset>(asset.text);
					var database = databaseAsset.Get();
					_defaultDatabase = database;
				}

				return _defaultDatabase;
			}
			set
			{
				_defaultDatabase = value;
			}
		}

		public Dictionary<string, IRPGObject> Objects { get; set; } = new();
		public List<string> Categories { get; set; } = new()
		{
			"Default"
		};

		public static bool AddObject(IRPGObject rpgObject)
		{
			return DefaultDatabase.AddObjectInstance(rpgObject);
		}
		
		public static IRPGObject GetObject(string guid)
		{
			return DefaultDatabase.GetObjectInstance(guid);
		}

		public bool AddObjectInstance(IRPGObject rpgObject)
		{
			if (Objects.ContainsKey(rpgObject.Guid))
			{
				return false;
			}
			
			Objects.Add(rpgObject.Guid, rpgObject);
			
			return true;
		}

		public IRPGObject GetObjectInstance(string guid)
		{
			if (Objects.ContainsKey(guid))
			{
				return Objects[guid];
			}

			return null;
		}
	}
}