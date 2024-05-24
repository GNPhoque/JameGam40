using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Grave : MonoBehaviour, IPointerClickHandler
{
	public static event Action<Grave> OnGraveClicked;

	public SerializedDictionary<DiggableLimb, int> weightedDiggableLimbPrefabs;

	public void OnPointerClick(PointerEventData eventData)
	{
		print("Clicked Grave");
		OnGraveClicked?.Invoke(this);
	}
}