using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObjects/Shovel")]
public class Shovel : ScriptableObject
{
	[SerializeField] public int energyCost;
	[SerializedDictionary("Position", "Damage")] public SerializedDictionary<Vector2Int, int> targets;
}
