using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShovelChange : MonoBehaviour, IPointerClickHandler
{
	public static event Action<Shovel> OnShovelChanged;

	[SerializeField] SpriteRenderer spriteRenderer;
	[SerializeField] Color unselectedColor;
	[SerializeField] Shovel shovel;

	private void Awake()
	{
		OnShovelChanged += ShovelChange_OnShovelChanged;
	}

	private void OnDestroy()
	{
		OnShovelChanged -= ShovelChange_OnShovelChanged;
	}

	private void ShovelChange_OnShovelChanged(Shovel obj)
	{
		if (shovel = obj) spriteRenderer.color = Color.white;
		else spriteRenderer.color = unselectedColor;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		GameManager.instance.graveyardMinigame.currentShovel = shovel;
		OnShovelChanged?.Invoke(shovel);
	}
}
