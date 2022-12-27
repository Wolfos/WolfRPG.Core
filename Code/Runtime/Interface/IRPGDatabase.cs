using System;

namespace WolfRPG.Core
{
	public interface IRPGDatabase
	{
		bool AddObjectInstance(IRPGObject rpgObject);
		IRPGObject GetObjectInstance(string guid);
	}
}