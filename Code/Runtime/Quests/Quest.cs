using UnityEngine;

namespace WolfRPG.Core.Quests
{
	public static class Quest
	{
		public static void ProgressToNextStage(QuestData data)
		{
			if (data.CurrentStage.Type == QuestStageType.Finished) return;
			 
			Debug.Log("Quest progress");
			data.CurrentStage.Progress++;
			if (data.CurrentStage.Progress >= data.CurrentStage.Number)
			{
				data.CurrentStage.Complete = true;
				Debug.Log("Completed quest stage");
				data.Progress.CurrentStage++;
				if (data.CurrentStage.Type == QuestStageType.Finished) data.Progress.Complete = true;
			}
		}

		public static void SetStage(QuestData data, int newStage, bool failed = false)
		{
			data.CurrentStage.Complete = failed == false;
			
			data.Progress.CurrentStage = newStage;
			if (data.CurrentStage.Type == QuestStageType.Finished)
			{
				data.Progress.Complete = true;
				data.CurrentStage.Complete = true;
			}
		}
	}
}