namespace WolfRPG.Core
{
	public interface IRPGObjectFactory
	{
		IRPGObject CreateNewObject(string name, int category);
	}
}