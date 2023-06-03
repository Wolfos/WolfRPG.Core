using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;


namespace WolfRPG.Core
{
	[Serializable]
	public class RPGObject: IRPGObject
	{
		public const string Label = "WolfRPG Object";
		public string Name { get; set; }
		public string Guid { get; set; }
		public int Category { get; set; }
		public bool IncludedInSavedGame { get; set; }
		
		[JsonProperty]
		private Dictionary<Type, IRPGComponent> _components = new();

		public T AddComponent<T>() where T : class, IRPGComponent, new()
		{
			var newComponent = new T();
			_components.Add(typeof(T), newComponent);

			return newComponent;
		}

		public void AddComponent(IRPGComponent component)
		{
			_components.Add(component.GetType(), component);
		}

		public void RemoveComponent(IRPGComponent component)
		{
			_components.Remove(component.GetType());
		}

		public T GetComponent<T>() where T : class, IRPGComponent, new()
		{
			if (_components.ContainsKey(typeof(T)))
			{
				return (T)_components[typeof(T)];
			}
			
			return null;
		}

		public bool HasComponent<T>()
		{
			return _components.ContainsKey(typeof(T));
		}

		public bool HasComponent(Type type)
		{
			return _components.ContainsKey(type);
		}

		public List<IRPGComponent> GetAllComponents()
		{
			return _components.Select(kvp => kvp.Value).ToList();
		}
	}
}