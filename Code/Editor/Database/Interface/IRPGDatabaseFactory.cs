﻿using UnityEngine;

namespace WolfRPG.Core
{
	public interface IRPGDatabaseFactory
	{
		IRPGDatabase CreateNewDatabase(out TextAsset asset);
		IRPGDatabase GetDefaultDatabase(out TextAsset asset);
		void SaveDatabase(IRPGDatabase database, string path);
		void SaveDefaultDatabase();
	}
}