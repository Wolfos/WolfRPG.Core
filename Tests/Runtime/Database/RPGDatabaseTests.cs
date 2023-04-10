using System;
using System.Linq;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace WolfRPG.Core.Tests.Runtime
{
	public class RPGDatabaseTests
	{
		
		[Test]
		public void AddObject_CallsDefaultDatabase()
		{
			var mockDB = new Mock<IRPGDatabase>();
			RPGDatabase.DefaultDatabase = mockDB.Object;
			var rpgObject = new RPGObject();

			mockDB.Setup(db => db.AddObjectInstance(rpgObject)).Returns(true);

			var actual = RPGDatabase.AddObject(rpgObject);
			Assert.IsTrue(actual);
		}
		
		[Test]
		public void GetObject_CallsDefaultDatabase()
		{
			var mockDB = new Mock<IRPGDatabase>();
			RPGDatabase.DefaultDatabase = mockDB.Object;
			var expected = new RPGObject();

			mockDB.Setup(db => db.GetObjectInstance(expected.Guid)).Returns(expected);

			var actual = RPGDatabase.GetObject(expected.Guid);
			Assert.AreEqual(expected, actual);
		}
			
		[Test]
		public void AddObjectInstance_CorrectInput_ReturnsTrue()
		{
			var target = new RPGDatabase();
			var rpgObject = new RPGObject();
			rpgObject.Guid = Guid.NewGuid().ToString();

			var actual = target.AddObjectInstance(rpgObject);
			Assert.IsTrue(actual);
		}
		
		[Test]
		public void AddObjectInstance_AlreadyHasObject_ReturnsFalse()
		{
			var target = new RPGDatabase();
			var rpgObject = new RPGObject();
			rpgObject.Guid = Guid.NewGuid().ToString();

			target.AddObjectInstance(rpgObject);
			var actual = target.AddObjectInstance(rpgObject);
			Assert.IsFalse(actual);
		}
		
		[Test]
		public void AddObjectInstance_AlreadyHasMatchingGuid_ReturnsFalse()
		{
			var target = new RPGDatabase();
			var rpgObject = new RPGObject();
			rpgObject.Guid = Guid.NewGuid().ToString();
			var rpgObject2 = new RPGObject();
			rpgObject2.Guid = rpgObject.Guid;

			target.AddObjectInstance(rpgObject);
			var actual = target.AddObjectInstance(rpgObject2);
			Assert.IsFalse(actual);
		}
		
		[Test]
		public void GetObjectInstance_HasObject_ReturnsObject()
		{
			var target = new RPGDatabase();
			var expected = new RPGObject();
			expected.Guid = Guid.NewGuid().ToString();

			target.AddObjectInstance(expected);
			var actual = target.GetObjectInstance(expected.Guid);
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void GetObjectInstance_DoesntHaveObject_ReturnsNull()
		{
			var target = new RPGDatabase();
			var rpgObject = new RPGObject();
			rpgObject.Guid = Guid.NewGuid().ToString();

			target.AddObjectInstance(rpgObject);
			var actual = target.GetObjectInstance(Guid.NewGuid().ToString());
			Assert.IsNull(actual);
		}

		[Test]
		public void GetSaveData_ReturnsValidJson()
		{
			var target = new RPGDatabase();

			var json = target.GetSaveData();
			var actual = JsonConvert.DeserializeObject<RPGDatabaseAsset>(json);
			
			Assert.IsNotNull(actual);
		}
		
		[Test]
		public void GetSaveData_IncludesMarkedObjects()
		{
			var target = new RPGDatabase();
			var rpgObject = new RPGObject
			{
				Guid = Guid.NewGuid().ToString(),
				IncludedInSavedGame = true
			};

			target.AddObjectInstance(rpgObject);

			var json = target.GetSaveData();
			var savedGame = JsonConvert.DeserializeObject<RPGSavedGame>(json);
			var actual = savedGame.RpgObjects.FirstOrDefault(obj => obj.Guid == rpgObject.Guid);
			Assert.IsNotNull(actual);
		}
		
		[Test]
		public void GetSaveData_DoesNotIncludeUnmarkedObjects()
		{
			var target = new RPGDatabase();
			var rpgObject = new RPGObject
			{
				Guid = Guid.NewGuid().ToString(),
				IncludedInSavedGame = false
			};

			target.AddObjectInstance(rpgObject);

			var json = target.GetSaveData();
			var savedGame = JsonConvert.DeserializeObject<RPGSavedGame>(json);
			var actual = savedGame.RpgObjects.FirstOrDefault(obj => obj.Guid == rpgObject.Guid);
			Assert.IsNull(actual);
		}

		[Test]
		public void ApplySaveData_AddsNewObjects()
		{
			var target = new RPGDatabase();
			
			var savedGame = new RPGSavedGame();
			var rpgObject = new RPGObject
			{
				Guid = Guid.NewGuid().ToString()
			};
			
			savedGame.RpgObjects.Add(rpgObject);

			var json = JsonConvert.SerializeObject(savedGame, Formatting.None, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
			
			target.ApplySaveData(json);
			var actual = target.GetObjectInstance(rpgObject.Guid);
			
			Assert.IsNotNull(actual);
		}
		
		[Test]
		public void ApplySaveData_IncludesChanges()
		{
			var target = new RPGDatabase();
			
			var rpgObject = new RPGObject
			{
				Guid = Guid.NewGuid().ToString()
			};

			var testComponent = rpgObject.AddComponent<TestComponent>();
			testComponent.TestValue = "I like cheese";
			
			target.AddObjectInstance(rpgObject);

			var json =
				$"{{\"RpgObjects\":[{{\"_components\":{{\"WolfRPG.Core.Tests.Runtime.TestComponent, WolfRPG.Core.Tests, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\":{{\"$type\":\"WolfRPG.Core.Tests.Runtime.TestComponent, WolfRPG.Core.Tests\",\"TestValue\":\"I like pizza\"}}}},\"Name\":null,\"Guid\":\"{rpgObject.Guid}\",\"Category\":0,\"IncludedInSavedGame\":false}}]}}";

			target.ApplySaveData(json);

			rpgObject = (RPGObject)target.GetObjectInstance(rpgObject.Guid);
			testComponent = rpgObject.GetComponent<TestComponent>();
			
			Assert.AreEqual("I like pizza", testComponent.TestValue);
		}

		[Test]
		public void GetObjectByName_HasObject_ReturnsCorrectObject()
		{
			var target = new RPGDatabase();
			
			var notExpected = new RPGObject
			{
				Guid = Guid.NewGuid().ToString(),
				Name = "Not Bob"
			};
			
			var expected = new RPGObject
			{
				Guid = Guid.NewGuid().ToString(),
				Name = "Bob"
			};

			target.AddObjectInstance(notExpected);
			target.AddObjectInstance(expected);

			var actual = target.GetObjectByName("Bob");
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void GetObjectByName_DoesNotHaveObject_ReturnsNull()
		{
			var target = new RPGDatabase();
			
			var notExpected = new RPGObject
			{
				Guid = Guid.NewGuid().ToString(),
				Name = "Not Bob"
			};

			target.AddObjectInstance(notExpected);

			var actual = target.GetObjectByName("Bob");
			Assert.IsNull(actual);
		}
	}
}