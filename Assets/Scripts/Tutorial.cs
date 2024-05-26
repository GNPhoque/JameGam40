using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
	public static event Action OnTutorialEventTriggered;

	public List<StringList> list = new List<StringList>();

	StringList currentList;
	bool isShowing;
	bool isEnded;

	private void Awake()
	{
		OnTutorialEventTriggered += Tutorial_OnTutorialEventTriggered;
	}

	private void OnDestroy()
	{
		OnTutorialEventTriggered -= Tutorial_OnTutorialEventTriggered;
	}

	private void Tutorial_OnTutorialEventTriggered()
	{
		ShowTutorial();
	}

	private void Update()
	{
		if (isEnded || !isShowing) return;

		if (Input.GetMouseButtonDown(0))
		{
			if (currentList.list.Count > 0) ShowTutorial();
		}
	}

	private void ShowTutorial()
	{

	}

	void GetNextStringList()
	{
		if(isEnded) return;

		currentList = new StringList();

		foreach (var item in list.First().list)
		{
			currentList.list.Add(item);
		}

		list.RemoveAt(0);
		if(list.Count <= 0) isEnded = true;
	}
}
