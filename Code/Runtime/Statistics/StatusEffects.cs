using Newtonsoft.Json;

namespace WolfRPG.Core.Statistics
{
	public enum StatusEffectType
	{
		NONE,
		ApplyOnce,
		ApplyForDuration,
		ApplyUntilRemoved,
		MAX
	}

	public enum StatusEffectModifierType
	{
		Attribute, Skill
	}

	public class StatusEffectModifier
	{
		public StatusEffectModifierType Type { get; set; }
		public Attribute Attribute { get; set; }
		public Skill Skill { get; set; }
		public int Modifier { get; set; }
	}
	
	public class StatusEffect : IRPGComponent
	{
		public int Id { get; set; }
		
		public StatusEffectModifier[] Modifiers { get; set; }
		public StatusEffectType Type { get; set; }
		public float Duration { get; set; }
		
		// Time effect was added
		[JsonIgnore]
		public float AddedTimeStamp { get; set; }
		// Last time effect was applied
		[JsonIgnore]
		public float ApplyTimeStamp { get; set; }

		public StatusEffect GetInstance() // Required because class is mutable
		{
			return new()
			{
				Id = Id,
				Modifiers = Modifiers,
				Type = Type,
				Duration = Duration,
				AddedTimeStamp = 0,
				ApplyTimeStamp = 0
			};
		}
	}
}