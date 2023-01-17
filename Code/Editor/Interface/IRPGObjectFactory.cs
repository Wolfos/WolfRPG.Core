﻿namespace WolfRPG.Core
{
	public interface IRPGObjectFactory
	{
		IRPGObject CreateNewObject(string name, int category);
		void SaveObject(IRPGObject rpgObject);
	}
}