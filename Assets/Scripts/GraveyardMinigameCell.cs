using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraveyardMinigameCell : MonoBehaviour, IPointerClickHandler
{
	public static Action<GraveyardMinigameCell> OnCellClicked;

	public int durability;
	public Vector2Int position;

	public void OnPointerClick(PointerEventData eventData)
	{
		OnCellClicked?.Invoke(this);
	}

	public void DigUp(int amount = 1)
	{
		durability -= amount;
		GetComponent<SpriteRenderer>().color = Color.red;
		if (durability <= 0) Reveal();
	}

	void Reveal()
	{
		//TODO : show what is hidden under the cell, if fully revealed, add to storage
		gameObject.SetActive(false);
	}
}
