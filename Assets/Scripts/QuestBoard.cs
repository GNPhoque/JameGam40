using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuestBoard : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI teethText;
	[SerializeField] TextMeshProUGUI pentacleText;
	[SerializeField] QuestNote questPrefab;
	[SerializeField] List<Quest> quests;
	[SerializeField] Transform noteGrid;
	
	List<QuestNote> questNotes = new List<QuestNote>();

	private void Awake()
	{
		GameManager.instance.onTeethValueChanged += GameManager_onTeethValueChanged;
		GameManager.instance.onPentaclesValueChanged += Instance_onPentaclesValueChanged; ;
	}

	private void GameManager_onTeethValueChanged(int obj)
	{
		teethText.text = obj.ToString();
	}

	private void Instance_onPentaclesValueChanged(int obj)
	{
		pentacleText.text = obj.ToString();
	}

	private void OnEnable()
	{
		foreach (var quest in quests)
		{
			if (questNotes.Any(x => x.quest == quest)) continue;

			QuestNote note = Instantiate(questPrefab, noteGrid);
			note.quest = quest;
			note.OnQuestNoteClick += Note_OnQuestNoteClick;
			note.Setup();
			questNotes.Add(note);
		}

		foreach (var note in questNotes)
		{
			if (IsQuestComplete(note.quest))
			{
				//Show complete
				note.isComplete = true;
			}
			else
			{
				//HideComplete
				note.isComplete = false;
			}
		}
	}

	private void Note_OnQuestNoteClick(QuestNote note)
	{
		SnapingParts body = GetQuestCompletingBody(note.quest);
		
		if (body == null)
		{
			Debug.LogError("Quest is marked as complete but no body fits the quest requirements");
			return;
		}

		GameManager.instance.teeth += note.quest.toothReward;
		GameManager.instance.pentacles += note.quest.pentacleReward;

		GameManager.instance.BuildedBodys.Remove(body);
		quests.Remove(note.quest);
		Destroy(body.gameObject);
		Destroy(note.gameObject);
	}

	public bool IsQuestComplete(Quest quest)
	{
		foreach (var body in GameManager.instance.BuildedBodys)
		{
			if (DoesBodyFitQuest(body, quest.requirements)) return true;
		}

		return false;
	}

	SnapingParts GetQuestCompletingBody(Quest quest)
	{
		foreach (var body in GameManager.instance.BuildedBodys)
		{
			if (DoesBodyFitQuest(body, quest.requirements)) return body;
		}

		return null;
	}

	bool DoesBodyFitQuest(SnapingParts body, SerializedDictionary<BodyParts, int> requirements)
	{
		foreach (var req in requirements)
		{
			if (!body.bodyElements.ContainsKey(req.Key) || body.bodyElements[req.Key] < req.Value) //body part not attached or not enough
			{
				return false;
			}
		}
		return true;
	}
}