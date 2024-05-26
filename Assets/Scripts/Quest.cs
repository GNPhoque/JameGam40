using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObjects/Quest")]
public class Quest : ScriptableObject
{
	public int toothReward;
	public int pentacleReward;

	public SerializedDictionary<BodyParts, int> requirements;
}
