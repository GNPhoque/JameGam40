using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyBuildingMinigame : MonoBehaviour
{
	[SerializeField] GameObject bodyBuildingMinigameFrame;

	private void Awake()
	{
	}

	public void Show()
	{
		print("Showing BodybuildingMinigame");
		bodyBuildingMinigameFrame.SetActive(true);
	}

	public void Exit()
	{
		bodyBuildingMinigameFrame.SetActive(false);
	}
}
