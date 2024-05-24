using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiggableLimb : MonoBehaviour
{
	public bool isBonus;
	public int energyBonus;
	public string inventoryLimbName;

	public List<Vector2Int> coveredPositionsSetup;
	public List<Vector2Int> coveredPositions;
}
