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
		public bool Complete { get; set; }
		public int CurrentStage { get; set; }

		public QuestData GetQuest()
		{
			var questObject = RPGDatabase.GetObject(Guid);
			var questData = questObject.GetComponent<QuestData>();
				
			questData.Progress = this;
			return questData;
		}
	}
	
	[Serializable]
	public class QuestStage
	{
		public QuestStageType Type { get; set; }
		public string Target { get; set; }
		public int Number { get; set; }
		public string Description { get; set; }
		
		[HideInInspector] public int Progress { get; set; }
		[HideInInspector] public bool Complete { get; set; }
	}
	
	public class QuestData: IRPGComponent
	{
		public LocalizedString QuestName { get; set; }
		public QuestStage[] Stages { get; set; }
		[JsonIgnore] public QuestProgress Progress { get; set; } = new();
		
		public QuestStage CurrentStage => Stages[Progress.CurrentStage];
	}
}