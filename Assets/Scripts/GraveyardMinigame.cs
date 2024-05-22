using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardMinigame : MonoBehaviour
{
	[SerializeField] GameObject graveyardMinigameFrame;

	public void Show()
	{
		print("Showing GraveyardMinigame");
		graveyardMinigameFrame.SetActive(true);
	}

	public void Exit()
	{
		graveyardMinigameFrame.SetActive(false);
	}
}
