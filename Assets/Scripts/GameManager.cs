using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	public event Action<int> onTeethValueChanged;
	public event Action<int> onPentaclesValueChanged;

	[SerializeField] UIManager uiManager;
	//[SerializeField] BodyBuildingMinigame bodyBuildingMinigame;
	[SerializeField] public GraveyardMinigame graveyardMinigame;
	[SerializeField] Inventory inventory;
	[SerializeField] Inventory startingInventory;
	[SerializeField] SerializedDictionary<Shovel, bool> shovels;
	[SerializeField] SerializedDictionary<Shovel, bool> startingShovels;
	public List<SnapingParts> BuildedBodys { get; private set;}
	private int _teeth;
	private int _pentacles;

	//Game state
	public bool isPaused;
	public bool isMinigameOpened;

	[SerializeField] bool resetshovels;

	public bool canChangeRoom { get => !isPaused && !isMinigameOpened; }
	public int teeth
	{
		get => _teeth; 
		set 
		{ 
			_teeth = value; 
			onTeethValueChanged?.Invoke(value); 
		}
	}
	public int pentacles { 
		get => _pentacles; 
		set 
		{
			_pentacles = value; 
			onPentaclesValueChanged?.Invoke(value); 
		} 
	}

	private void Awake()
	{
		if (instance) Destroy(instance.gameObject);

		instance = this;
		BuildedBodys = new List<SnapingParts>();
		GraveyardMinigame.OnEnergyValueChanged += uiManager.UpdateUraveyardEnergyText;
		Grave.OnGraveClicked += OpenGraveyardMinigame;
		inventory.SetInventory(startingInventory);
		if (resetshovels)
		{
			ResetShovels();
		}
	}

	private void ResetShovels()
	{
		shovels.Clear();
		foreach (var item in startingShovels)
		{
			shovels.Add(item.Key, item.Value);
		}
	}

	private void OnDestroy()
	{
		GraveyardMinigame.OnEnergyValueChanged -= uiManager.UpdateUraveyardEnergyText;
		Grave.OnGraveClicked -= OpenGraveyardMinigame;
	}

	public void OpenGraveyardMinigame(Grave grave)
	{
		isMinigameOpened = true;
		uiManager.ShowGraveyardEnergy();
		graveyardMinigame.SetWeightedDiggableLimb(grave);
		graveyardMinigame.Show();
	}

	public void CloseGraveyardMinigame()
	{
		isMinigameOpened = false;
		uiManager.HideGraveyardEnergy();
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
	public int GetLimbStock(string limb)
	{
		return inventory.GetLimbStock(limb);
	}

	public void UnlockShovel(Shovel shovel)
	{
		shovels[shovel] = true;
	}

	[ContextMenu("AddTeeth")]
	public void AddTeeth()
	{
		teeth += 5000;
	}
}
