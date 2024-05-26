using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuestNote : MonoBehaviour, IPointerClickHandler
{
	public event Action<QuestNote> OnQuestNoteClick;

	[SerializeField] Sprite[] noteSprites;
	[SerializeField] Image image;
	[SerializeField] TextMeshProUGUI requirementsText;
	[SerializeField] TextMeshProUGUI toothText;
	[SerializeField] TextMeshProUGUI pentacleText;

	public Quest quest;
	public bool isComplete;

	public void Setup()
	{
		image.sprite = noteSprites[Random.Range(0, noteSprites.Length)];
		toothText.text = quest.toothReward.ToString();
		pentacleText.text = quest.pentacleReward.ToString();
		List<KeyValuePair<BodyParts, int>> parts = quest.requirements.Where(x => x.Value > 0).ToList();
		List<string> final = new List<string>();
		foreach (var item in parts)
		{
			final.Add($"{item.Key} : {item.Value}");
		}

		requirementsText.text = $"Order:\n{string.Join("\n", final)}";
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!isComplete) return;

		OnQuestNoteClick?.Invoke(this);
	}
}
