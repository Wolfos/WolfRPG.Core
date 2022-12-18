using System;

namespace WolfRPG.Core
{
	public interface IRPGObject
	{
		Guid Guid { get; set; }
		T AddComponent<T>() where T : class, IRPGComponent, new();
		T GetComponent<T>() where T : class, IRPGComponent, new();
	}
}