using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.TestTools;

namespace WolfRPG.Core.Tests.Data.Runtime
{
	public class RPGObjectTests
	{
		[Test]
		public void GetComponent_ComponentExists_ReturnsTrue()
		{
			var target = new RPGObject();
			target.AddComponent<TestComponent>();
			var actual = target.GetComponent<TestComponent>(out _);
			
			Assert.IsTrue(actual);
		}
		
		[Test]
		public void GetComponent_ComponentExists_SetsComponent()
		{
			var target = new RPGObject();
			var expected = target.AddComponent<TestComponent>();
			target.GetComponent<TestComponent>(out var actual);
			
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void GetComponent_ComponentDoesNotExist_ReturnsFalse()
		{
			var target = new RPGObject();
			
			LogAssert.Expect(LogType.Error, "No component of type WolfRPG.Core.Tests.Data.Runtime.TestComponent was present on object ");
			var actual = target.GetComponent<TestComponent>(out _);
			
			Assert.AreEqual(false, actual);
		}
		
		[Test]
		public void GetComponent_ComponentDoesNotExist_ComponentNotNull()
		{
			var target = new RPGObject();
			
			LogAssert.Expect(LogType.Error, "No component of type WolfRPG.Core.Tests.Data.Runtime.TestComponent was present on object ");
			target.GetComponent<TestComponent>(out var component);
			
			Assert.NotNull(component);
		}

		[Test]
		public void AddComponent_ReturnsComponent()
		{
			var target = new RPGObject();
			var actual = target.AddComponent<TestComponent>();
			
			Assert.AreEqual(typeof(TestComponent), actual.GetType());
		}
	}
}