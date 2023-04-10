using System.Collections.Generic;

namespace WolfRPG.Core
{
	public class RPGSavedGame
	{
		public List<RPGObject> RpgObjects { get; set; } = new();
	}
}