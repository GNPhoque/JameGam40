using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	[SerializeField] BodyBuildingMinigame bodyBuildingMinigame;
	[SerializeField] GraveyardMinigame graveyardMinigame;
	[SerializeField] Inventory inventory;
	[SerializeField] Inventory startingInventory;

	//Game state
	public bool isPaused;
	public bool isMinigameOpened;

	public bool canChangeRoom { get => !isPaused && !isMinigameOpened; }

	private void Awake()
	{
		if (instance) Destroy(instance.gameObject);

		instance = this;
		Grave.OnGraveClicked += OpenGraveyardMinigame;
		inventory.SetInventory(startingInventory);
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

	public void AddLimb(string limb, int qty = 1)
	{
		inventory.AddLimb(limb, qty);
	}

	public bool TakeLimb(string limb, int qty = 1)
	{
		return inventory.TakeLimb(limb, qty);
	}
}
