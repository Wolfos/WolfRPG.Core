using System;
using System.Collections.Generic;

namespace WolfRPG.Core
{
	public struct RPGObject
	{
		public Guid Guid;
		public string Name;
		
		public List<IRPGComponent> Components;
	}
}