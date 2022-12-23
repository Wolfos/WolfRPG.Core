using System;
using System.Collections.Generic;
using UnityEngine;

namespace WolfRPG.Core
{
	[Serializable]
	public class RPGObject: IRPGObject
	{
		public Guid Guid { get; set; }
		
		public string Name;
		
		private Dictionary<Type, IRPGComponent> _components = new();


		public T AddComponent<T>() where T : class, IRPGComponent, new()
		{
			var newComponent = new T();
			_components.Add(typeof(T), newComponent);
			
			return newComponent;
		}
		
		public T GetComponent<T>() where T : class, IRPGComponent, new()
		{
			if (_components.ContainsKey(typeof(T)))
			{
				return (T)_components[typeof(T)];
			}
			
			Debug.LogError($"No component of type {typeof(T)} was present on object {Name}");
			return null;
		}
	}
}