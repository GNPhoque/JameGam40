using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class GraveyardMinigameCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public static Action<GraveyardMinigameCell> OnCellClicked;
	public static Action<GraveyardMinigameCell> OnCellHoverIn;
	public static Action<GraveyardMinigameCell> OnCellHoverOut;
	public static Action<GraveyardMinigameCell> OnCellRevealed;

	[SerializeField] SerializedDictionary<Sprite, Sprite> grassSprites;
	[SerializeField] SerializedDictionary<Sprite, Sprite> upperDirtSprites;
	[SerializeField] SerializedDictionary<Sprite, Sprite> lowerDirtSprites;

	public bool isRevealed;
	public int durability;
	public int incomingShovelDamage;
	public Vector2Int position;

	SpriteRenderer spriteRenderer;
	Sprite unselectedSprite;
	Sprite selectedSprite;

	bool isSelected;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		UpdateSprite();
	}

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

	public void UpdateSprite()
	{
		SerializedDictionary<Sprite, Sprite> pool;
		switch (durability)
		{
			case 2:
				pool = upperDirtSprites;
				break;
			case 1:
				pool = lowerDirtSprites;
				break;
			default:
				pool = grassSprites;
				break;
		}

		KeyValuePair<Sprite, Sprite> pair = pool.ElementAt(Random.Range(0, pool.Count));
		unselectedSprite = pair.Key;
		selectedSprite = pair.Value;

		spriteRenderer.sprite = isSelected ? selectedSprite : unselectedSprite;
	}

	public void DigUp(int amount = 1)
	{
		durability -= amount;
		UpdateSprite();
		if (durability <= 0) Reveal();
	}

	public void ShowTarget()
	{
		isSelected = true;
		spriteRenderer.sprite = selectedSprite;
	}

	public void HideTarget()
	{
		isSelected = false;
		spriteRenderer.sprite = unselectedSprite;
	}

	void Reveal()
	{		
		gameObject.SetActive(false);
		isRevealed = true;
		OnCellRevealed?.Invoke(this);
	}
}
