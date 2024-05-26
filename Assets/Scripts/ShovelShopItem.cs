using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShovelShopItem : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] GameObject graveyardTool;
	[SerializeField] GameObject soldoutObject;
	[SerializeField] Shovel shovel;
	[SerializeField] int cost;
	[SerializeField] bool soldout;

	private void Start()
	{
		if (soldout) ShowSoldOut();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (soldout) return;

		if (GameManager.instance.teeth >= cost)
		{
			GameManager.instance.teeth -= cost;
			GameManager.instance.UnlockShovel(shovel);
			graveyardTool.SetActive(true);
			ShowSoldOut();
			soldout = true;
		}
		else
		{
			//anim insuffisant funds
		}
	}

	void ShowSoldOut()
	{
		soldoutObject.SetActive(true);
	}
}
