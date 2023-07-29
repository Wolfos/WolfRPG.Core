using System.Collections.Generic;

namespace WolfRPG.Core
{
	/// <summary>
	/// Object templates that can be selected in the editor.
	/// For example, the user can select the "Item: consumable" template to create a new object with ItemData and ConsumableData components
	/// </summary>
	public interface IWolfRPGTemplate
	{
		public string Name { get; }
		public IEnumerable<IRPGComponent> GetComponents();
	}
}