﻿using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace WolfRPG.Core.Tests.Runtime
{
	public class RPGObjectTests
	{
		[Test]
		public void AddComponent_ReturnsComponentThatIsNotNull()
		{
			var target = new RPGObject();
			var actual = target.AddComponent<TestComponent>();
			Assert.IsNotNull(actual);
		}
		
		[Test]
		public void GetComponent_ComponentExists_ReturnsComponent()
		{
			var target = new RPGObject();
			var expected = target.AddComponent<TestComponent>();
			var actual = target.GetComponent<TestComponent>();
			
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetComponent_ComponentDoesNotExist_ReturnsNull()
		{
			var target = new RPGObject();
			
			var actual = target.GetComponent<TestComponent>();
			Assert.IsNull(actual);
		}
	}
}