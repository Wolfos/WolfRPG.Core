using UnityEngine;

namespace WolfRPG.Core
{
	// TODO: For testing purposes. Don't forget to remove
	// Yeah this is definitely making it into production
	public class MyComponent: IRPGComponent
	{
		public string MyString { get; set; }
		public int MyInt { get; set; }
		public float MyFloat { get; set; }
		public GameObject MyPrefab { get; set; }
	}
}