namespace WolfRPG.Core.Statistics
{
	public class AttributeStatusEffect
	{
		public Attributes Attribute { get; set; }
		public float Effect { get; set; }
		public bool Permanent { get; set; }
		public float Duration { get; set; }
	}
	
	public class SkillStatusEffect
	{
		public Skills Skill { get; set; }
		public float Effect { get; set; }
		public bool Permanent { get; set; }
		public float Duration { get; set; }
	}
}