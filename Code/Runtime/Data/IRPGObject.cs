namespace WolfRPG.Core
{
	public interface IRPGObject
	{
		T AddComponent<T>() where T : class, IRPGComponent, new();
		T GetComponent<T>() where T : class, IRPGComponent, new();
	}
}