using System;
using System.Collections.Generic;

namespace WolfRPG.Core
{
	public interface IRPGObject
	{
		string Name { get; set; }
		string Guid { get; set; }
		int Category { get; set; }
		T AddComponent<T>() where T : class, IRPGComponent, new();
		void AddComponent(IRPGComponent component);
		void RemoveComponent(IRPGComponent component);
		T GetComponent<T>() where T : class, IRPGComponent, new();
		bool HasComponent<T>();
		bool HasComponent(Type type);
		List<IRPGComponent> GetAllComponents();
	}
}