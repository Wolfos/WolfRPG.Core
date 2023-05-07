namespace WolfRPG.Core.Statistics
{
	public class AttributeStatusEffect
	{
		public string StatusEffectName { get; set; }
		public Attribute Attribute { get; set; }
		public int Effect { get; set; }
		public bool Permanent { get; set; }
		public bool ApplyEverySecond { get; set; }
		public float Duration { get; set; }
		// Time effect was added
		public float AddedTimeStamp { get; set; }
		// Last time effect was applied
		public float ApplyTimeStamp { get; set; }
	}
	
	public class SkillStatusEffect
	{
		public string StatusEffectName { get; set; }
		public Skill Skill { get; set; }
		public int Effect { get; set; }
		public bool Permanent { get; set; }
		public float Duration { get; set; }
		// Time effect was added
		public float AddedTimeStamp { get; set; }
		// Last time effect was applied
		public float ApplyTimeStamp { get; set; }
		
	}
}