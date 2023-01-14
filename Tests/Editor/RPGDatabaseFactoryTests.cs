using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace WolfRPG.Core.Tests.Runtime
{
	public class RPGDatabaseFactoryTests
	{
		// [Test]
		// public void CreateNewDatabase_ReturnsNotNull()
		// {
		// 	var target = new RPGDatabaseFactory();
		// 	var previousPath = RPGDatabaseFactory.DefaultRelativeDatabasePath;
		// 	RPGDatabaseFactory.DefaultRelativeDatabasePath = "TestDatabase.json";
		// 	
		// 	var actual = target.CreateNewDatabase(out _);
		//
		// 	Assert.IsNotNull(actual);
		// 	AssetDatabase.DeleteAsset($"Assets/{RPGDatabaseFactory.DefaultRelativeDatabasePath}");
		//
		// 	RPGDatabaseFactory.DefaultRelativeDatabasePath = previousPath;
		// }
		
		// [Test]
		// public void CreateNewDatabase_SetsGUID()
		// {
		// 	var target = new RPGDatabaseFactory();
		// 	var previousPath = RPGDatabaseFactory.DefaultRelativeDatabasePath;
		// 	RPGDatabaseFactory.DefaultRelativeDatabasePath = "TestDatabase.json";
		// 	
		// 	_ = target.CreateNewDatabase(out var actual);
		//
		// 	Assert.IsNotNull(actual);
		// 	AssetDatabase.DeleteAsset($"Assets/{RPGDatabaseFactory.DefaultRelativeDatabasePath}");
		// 	RPGDatabaseFactory.DefaultRelativeDatabasePath = previousPath;
		// }
	}
}