using System;
using System.Collections.Generic;

namespace WolfRPG.Core
{
	public class RPGDatabase: IRPGDatabase
	{
		public static IRPGDatabase DefaultDatabase { get; set; }
		public Dictionary<string, IRPGObject> Objects = new();


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