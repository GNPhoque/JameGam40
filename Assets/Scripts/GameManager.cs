using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[SerializeField] BodyBuildingMinigame bodyBuildingMinigame;
	[SerializeField] GraveyardMinigame graveyardMinigame;

	//Game state
	public bool isPaused;
	public bool isMinigameOpened;

	public bool canChangeRoom { get => !isPaused && !isMinigameOpened; }

	private void Awake()
	{
		if (instance) Destroy(instance.gameObject);

		instance = this;
		Grave.OnGraveClicked += OpenGraveyardMinigame;
	}

	private void OnDestroy()
	{
		Grave.OnGraveClicked -= OpenGraveyardMinigame;
	}

	public void OpenGraveyardMinigame()
	{
		isMinigameOpened = true;
		graveyardMinigame.Show();
	}

	public void CloseGraveyardMinigame()
	{
		isMinigameOpened = false;
		graveyardMinigame.Exit();
	}
}
