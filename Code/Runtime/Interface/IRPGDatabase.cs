using System;
using System.Collections.Generic;

namespace WolfRPG.Core
{
	public interface IRPGDatabase
	{
		Dictionary<string, IRPGObject> Objects { get; set; }
		
		bool AddObjectInstance(IRPGObject rpgObject);
		IRPGObject GetObjectInstance(string guid);
	}
}