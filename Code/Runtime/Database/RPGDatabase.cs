using System.Collections.Generic;
using System.Linq;
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
					Addressables.Release(loadOperation);
				
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

		/// <summary>
		/// All objects, sorted by GUID
		/// </summary>
		public Dictionary<string, IRPGObject> Objects { get; set; } = new();
		
		/// <summary>
		/// The categories, as used in the WolfRPG editor
		/// </summary>
		public List<string> Categories { get; set; } = new()
		{
			"Default"
		};

		public int NumObjectsAdded { get; set; }

		/// <summary>
		/// Add an object to the database
		/// </summary>
		public static bool AddObject(IRPGObject rpgObject)
		{
			return DefaultDatabase.AddObjectInstance(rpgObject);
		}
		
		/// <summary>
		/// Retrieve an object from the database by GUID
		/// </summary>
		/// <param name="guid">the object's unique ID</param>
		/// <returns>The object if found, otherwise null</returns>
		public static IRPGObject GetObject(string guid)
		{
			return DefaultDatabase.GetObjectInstance(guid);
		}

		/// <summary>
		/// Add an object to the database
		/// </summary>
		public bool AddObjectInstance(IRPGObject rpgObject)
		{
			if (Objects.ContainsKey(rpgObject.Guid))
			{
				return false;
			}
			
			Objects.Add(rpgObject.Guid, rpgObject);
			
			return true;
		}

		/// <summary>
		/// Remove an object from the database
		/// </summary>
		public void RemoveObjectInstance(IRPGObject rpgObject)
		{
			if (Objects.ContainsKey(rpgObject.Guid) == false)
			{
				return;
			}

			Objects.Remove(rpgObject.Guid);
		}

		/// <summary>
		/// Retrieve an object from the database by GUID
		/// </summary>
		/// <param name="guid">the object's unique ID</param>
		/// <returns>The object if found, otherwise null</returns>
		public IRPGObject GetObjectInstance(string guid)
		{
			if (Objects.ContainsKey(guid))
			{
				return Objects[guid];
			}

			return null;
		}

		public void SetObjectInstance(IRPGObject rpgObject)
		{
			if (Objects.ContainsKey(rpgObject.Guid))
			{
				Objects[rpgObject.Guid] = rpgObject;
			}
		}

		public IRPGObject GetObjectByName(string name)
		{
			foreach (var kvp in Objects)
			{
				if (kvp.Value.Name == name) return kvp.Value;
			}

			return null;
		}

		/// <summary>
		/// Retrieve save data in JSON format
		/// </summary>
		public string GetSaveData()
		{
			var savedGame = new RPGSavedGame();
			foreach (var rpgObject in Objects.Select(keyValuePair => keyValuePair.Value).Where(rpgObject => rpgObject.IncludedInSavedGame))
			{
				savedGame.RpgObjects.Add((RPGObject)rpgObject);
			}
			
			return JsonConvert.SerializeObject(savedGame, Formatting.None, 
				new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto
				});
		}

		/// <summary>
		/// Apply savegame data to this database
		/// </summary>
		public void ApplySaveData(string json)
		{
			var savedGame = JsonConvert.DeserializeObject<RPGSavedGame>(json, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
			
			if (savedGame == null)
			{
				Debug.LogError("Failed to load saved data");
				return;
			}

			foreach (var rpgObject in savedGame.RpgObjects)
			{
				var sameObject = GetObjectInstance(rpgObject.Guid);
				
				// If we don't already have the object
				if (sameObject == null)
				{
					// Add it
					AddObjectInstance(rpgObject);
				}
				else
				{
					// We already have it, so override it
					Objects[rpgObject.Guid] = rpgObject;
				}
			}
		}
	}
}