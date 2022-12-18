using System;
using System.Collections.Generic;

namespace WolfRPG.Core
{
	public class RPGDatabase: IRPGDatabase
	{
		public static IRPGDatabase DefaultDatabase { get; set; }
		private readonly Dictionary<Guid, IRPGObject> _objects = new();


		public static bool AddObject(IRPGObject rpgObject)
		{
			return DefaultDatabase.AddObjectInstance(rpgObject);
		}
		
		public static IRPGObject GetObject(Guid guid)
		{
			return DefaultDatabase.GetObjectInstance(guid);
		}

		public bool AddObjectInstance(IRPGObject rpgObject)
		{
			if (_objects.ContainsKey(rpgObject.Guid))
			{
				return false;
			}
			
			_objects.Add(rpgObject.Guid, rpgObject);
			
			return true;
		}

		public IRPGObject GetObjectInstance(Guid guid)
		{
			if (_objects.ContainsKey(guid))
			{
				return _objects[guid];
			}

			return null;
		}
	}
}