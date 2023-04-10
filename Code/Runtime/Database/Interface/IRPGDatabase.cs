using System.Collections.Generic;

namespace WolfRPG.Core
{
	public interface IRPGDatabase
	{
		Dictionary<string, IRPGObject> Objects { get; set; }
		List<string> Categories { get; set; }
		int NumObjectsAdded { get; set; }
		
		bool AddObjectInstance(IRPGObject rpgObject);
		void RemoveObjectInstance(IRPGObject rpgObject);
		IRPGObject GetObjectInstance(string guid);
		IRPGObject GetObjectByName(string name);
		string GetSaveData();
		void ApplySaveData(string json);
	}
}