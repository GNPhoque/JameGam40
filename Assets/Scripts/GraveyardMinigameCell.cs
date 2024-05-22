using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraveyardMinigameCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public static Action<GraveyardMinigameCell> OnCellClicked;
	public static Action<GraveyardMinigameCell> OnCellHoverIn;
	public static Action<GraveyardMinigameCell> OnCellHoverOut;

	public int durability;
	public Vector2Int position;


	public void OnPointerEnter(PointerEventData eventData)
	{
		OnCellHoverIn(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OnCellHoverOut(this);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnCellClicked?.Invoke(this);
	}

	public void DigUp(int amount = 1)
	{
		durability -= amount;
		ShowDurability();
		if (durability <= 0) Reveal();
	}

	public void Highlight()
	{
		GetComponent<SpriteRenderer>().color = Color.green;
	}

	public void ShowDurability()
	{
		GetComponent<SpriteRenderer>().color = durability < 3 ? Color.red : Color.white;
	}

	void Reveal()
	{
		//TODO : show what is hidden under the cell, if fully revealed, add to storage
		gameObject.SetActive(false);
	}
}
