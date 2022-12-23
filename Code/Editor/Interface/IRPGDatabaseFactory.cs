using System;
using UnityEditor;
using UnityEngine;

namespace WolfRPG.Core
{
	public interface IRPGDatabaseFactory
	{
		IRPGDatabase CreateNewDatabase(out Nullable<GUID> guid);
		IRPGDatabase GetDefaultDatabase(out TextAsset asset);
	}
}