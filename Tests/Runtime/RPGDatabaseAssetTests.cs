using NUnit.Framework;

namespace WolfRPG.Core.Tests.Runtime
{
	public class RPGDatabaseAssetTests
	{
		// More of a smoke test really
		[Test]
		public void Get_Works()
		{
			var obj = new RPGObject();
			obj.AddComponent<TestComponent>();
			obj.GetComponent<TestComponent>().TestValue = "Test01";

			var expected = new RPGDatabase();
			expected.AddObjectInstance(obj);

			var dbAsset = new RPGDatabaseAsset();
			dbAsset.CreateFrom(expected);

			var actual = dbAsset.Get();
			Assert.AreEqual(expected, actual);
		}
	}
}