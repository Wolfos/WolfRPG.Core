using UnityEngine;

namespace WolfRPG.Core
{
	// TODO: For testing purposes. Don't forget to remove
	// Yeah this is definitely making it into production
	public class MyComponent: IRPGComponent
	{
		public int MyInt { get; set; }
		public float MyFloat { get; set; }
		public string MyString { get; set; }
		public Vector2 MyVector2 { get; set; }
		public Vector3 MyVector3 { get; set; }
		public Vector4 MyVector4 { get; set; }
		public Vector2Int MyVector2Int { get; set; }
		public Vector3Int MyVector3Int { get; set; }
		public Rect MyRect { get; set; }
		public Bounds MyBounds { get; set; }
		public Color MyColor { get; set; }
		public AnimationCurve MAnimationCurve { get; set; }
		public Gradient MyGradient { get; set; }
		public MyEnum MyEnum { get; set; }
		public LayerMask MyLayerMask { get; set; }
		// Not supported yet
		//public GameObject MyPrefab { get; set; }
	}

	public enum MyEnum
	{
		One, Two, Banana, Pancakes, With, Bacon
	}
}