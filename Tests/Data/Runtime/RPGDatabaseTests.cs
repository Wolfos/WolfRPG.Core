using System;
using Moq;
using NUnit.Framework;

namespace WolfRPG.Core.Tests.Data.Runtime
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

			var actual = target.AddObjectInstance(rpgObject);
			Assert.IsTrue(actual);
		}
		
		[Test]
		public void AddObjectInstance_AlreadyHasObject_ReturnsFalse()
		{
			var target = new RPGDatabase();
			var rpgObject = new RPGObject();

			target.AddObjectInstance(rpgObject);
			var actual = target.AddObjectInstance(rpgObject);
			Assert.IsFalse(actual);
		}
		
		[Test]
		public void AddObjectInstance_AlreadyHasMatchingGuid_ReturnsFalse()
		{
			var target = new RPGDatabase();
			var rpgObject = new RPGObject();
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

			target.AddObjectInstance(expected);
			var actual = target.GetObjectInstance(expected.Guid);
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void GetObjectInstance_DoesntHaveObject_ReturnsNull()
		{
			var target = new RPGDatabase();
			var rpgObject = new RPGObject();

			target.AddObjectInstance(rpgObject);
			var actual = target.GetObjectInstance(Guid.NewGuid());
			Assert.IsNull(actual);
		}
		
	}
}