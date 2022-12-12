﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace WolfRPG.Core
{
	[Serializable]
	public class RPGObject: IRPGObject
	{
		public Guid Guid = Guid.NewGuid();
		public string Name;
		
		private Dictionary<Type, IRPGComponent> _components = new();

		public bool GetComponent<T>(out T component) where T : class, IRPGComponent, new()
		{
			if (_components.ContainsKey(typeof(T)))
			{
				component = (T)_components[typeof(T)];
				return true;
			}
			
			Debug.LogError($"No component of type {typeof(T)} was present on object {Name}");
			component = new();
			return false;
		}

		public T AddComponent<T>() where T : class, IRPGComponent, new()
		{
			var newComponent = new T();
			_components.Add(typeof(T), newComponent);
			
			return newComponent;
		}
	}
}