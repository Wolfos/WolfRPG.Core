using System;

namespace WolfRPG.Core
{
	public interface IRPGObject
	{
		string Name { get; set; }
		string Guid { get; set; }
		T AddComponent<T>() where T : class, IRPGComponent, new();
		void AddComponent(IRPGComponent component);
		T GetComponent<T>() where T : class, IRPGComponent, new();
		bool HasComponent<T>();
		bool HasComponent(Type type);
	}
}