using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Grave : MonoBehaviour, IPointerClickHandler
{
	public static event Action OnGraveClicked;

	public void OnPointerClick(PointerEventData eventData)
	{
		print("Clicked Grave");
		OnGraveClicked?.Invoke();
	}
}
