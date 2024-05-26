using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
	public List<StringList> list = new List<StringList>();
	[SerializeField] TextMeshProUGUI tutoText;

	Action latestAction;
	[SerializeField] StringList currentList;
	bool isShowing;
	bool isFinalList;
	bool isFinalString;
	bool firstTimeEvent = true;
	int eventNumber;

	private void Start()
	{
		isShowing = true;
		GetNextStringList();
	}

	private void Update()
	{
		if ((isFinalList && isFinalString) || !isShowing) return;

		if (Input.GetMouseButtonDown(0))
		{
			ShowNextString();
		}
	}

	void GetNextStringList()
	{
		if (isFinalList && isFinalString) return;

		isShowing = true;
		gameObject.SetActive(true);

		currentList.list.Clear();

		//copy string list
		foreach (var item in list.First().list)
		{
			currentList.list.Add(item);
		}

		//remove copied string list
		list.RemoveAt(0);
		if (list.Count <= 0) isFinalList = true;
		isFinalString = false;

		//subscribe to next event
		switch (eventNumber)
		{
			case 0:
				QuestBoard.OnQuestBoardOpen += GetNextStringList;
				break;
			case 1:
				QuestBoard.OnQuestBoardOpen -= GetNextStringList;
				UIDropInOutAnimator.OnUIClosed += GetNextStringList;
				break;
			case 2:
				UIDropInOutAnimator.OnUIClosed -= GetNextStringList;
				CameraMovement.OnWorkshopCameraFocused += GetNextStringList;
				break;
			case 3:
				CameraMovement.OnWorkshopCameraFocused -= GetNextStringList;
				CameraMovement.OnGraveyardCameraFocused += GetNextStringList;
				break;
			case 4:
				CameraMovement.OnGraveyardCameraFocused -= GetNextStringList;
				GraveyardMinigame.OnGraveyardMinigameOpen += GetNextStringList;
				break;
			case 5:
				GraveyardMinigame.OnGraveyardMinigameOpen -= GetNextStringList;
				CameraMovement.OnWorkshopCameraFocused += GetNextStringList;
				break;
			case 6:
				CameraMovement.OnWorkshopCameraFocused -= GetNextStringList;
				QTE.OnBodyMended += GetNextStringList;
				break;
			case 7:
				QTE.OnBodyMended -= GetNextStringList;
				CameraMovement.OnShopCameraFocused += GetNextStringList;
				break;
			case 8:
				CameraMovement.OnShopCameraFocused -= GetNextStringList;
				break;
			default:
				break;
		}
		eventNumber++;

		ShowNextString();
	}

	void ShowNextString()
	{
		if(currentList.list.Count <= 0)
		{
			isFinalString = true;
			isShowing = false;
			gameObject.SetActive(false);
			return;
		}

		tutoText.text = currentList.list.First();
		currentList.list.RemoveAt(0);
	}
}
