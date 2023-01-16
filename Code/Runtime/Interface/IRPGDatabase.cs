using System;
using System.Collections.Generic;

namespace WolfRPG.Core
{
	public interface IRPGDatabase
	{
		Dictionary<string, IRPGObject> Objects { get; set; }
		List<string> Categories { get; set; }
		
		bool AddObjectInstance(IRPGObject rpgObject);
		IRPGObject GetObjectInstance(string guid);
	}
}