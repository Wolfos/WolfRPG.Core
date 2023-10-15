using UnityEngine;

namespace WolfRPG.Core.Quests
{
	public static class Quest
	{
		public static void ProgressToNextStage(QuestProgress progress)
		{
			Debug.Log("Quest progress");
			progress.CurrentStage++;

			var data = progress.GetQuest();
			if (data.Stages[progress.CurrentStage].Type == QuestStageType.Finished)
			{
				progress.Complete = true;
			}
		}

		public static QuestData GetQuest(string guid)
		{
			var questObject = RPGDatabase.GetObject(guid);
			return questObject.GetComponent<QuestData>();
		}

		public static void SetStage(QuestProgress progress, int newStage)
		{
			var data = progress.GetQuest();

			progress.CurrentStage = newStage;
			if (data.Stages[progress.CurrentStage].Type == QuestStageType.Finished)
			{
				progress.Complete = true;
			}
		}
	}
}