using System.Collections.Generic;

namespace WolfRPG.Core
{
	public interface IWolfRPGTemplate
	{
		public string Name { get; }
		public IEnumerable<IRPGComponent> GetComponents();
	}
}