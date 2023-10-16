using System;
using Newtonsoft.Json;
using UnityEngine;
using WolfRPG.Core.Localization;

namespace WolfRPG.Core.Quests
{
	public enum QuestStageType
	{
		KillX, FindObject, SpeakTo, Finished
	}
	
	[Serializable]
	public class QuestProgress
	{
		public string Guid { get; set; }
		// Whether quest is complete
		public bool Complete { get; set; }
		// Current quest stage
		public int CurrentStage { get; set; }
		// Optional, for when a stage has more than one task to complete
		public int StageProgress { get; set; }

		public QuestData GetQuest()
		{
			var questObject = RPGDatabase.GetObject(Guid);
			var questData = questObject.GetComponent<QuestData>();
			
			return questData;
		}
	}
	
	[Serializable]
	public class QuestStage
	{
		public QuestStageType Type { get; set; }
		[DBReference(2)] public RPGObjectReference Target { get; set; }
		public int Number { get; set; }
		public string Description { get; set; }
		
		[HideInInspector] public int Progress { get; set; }
		[HideInInspector] public bool Complete { get; set; }
	}
	
	public class QuestData: IRPGComponent
	{
		public LocalizedString QuestName { get; set; }
		public QuestStage[] Stages { get; set; }
	}
}