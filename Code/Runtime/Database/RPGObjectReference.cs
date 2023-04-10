using System;

namespace WolfRPG.Core
{
	/// <summary>
	/// Reference to RPG Object as stored by Unity serializer
	/// </summary>
	[Serializable]
	public class RPGObjectReference
	{
		public string Guid;

		private IRPGObject _rpgObject;
		
		/// <summary>
		/// Retrieve the object associated with this reference. Slow on first run, cached afterwards
		/// </summary>
		/// <returns>The object, if one was associated with the reference. Otherwise returns null</returns>
		public IRPGObject GetObject()
		{
			if (_rpgObject == null)
			{
				_rpgObject = RPGDatabase.GetObject(Guid);
			}

			return _rpgObject;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetComponent<T>() where T : class, IRPGComponent, new()
		{
			return GetObject().GetComponent<T>();
		}
	}
}