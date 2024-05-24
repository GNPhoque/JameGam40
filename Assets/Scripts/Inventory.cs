using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObjects/Inventory")]
public class Inventory : ScriptableObject
{
	public SerializedDictionary<string, int> limbs;

	public void EmptyInventory()
	{
		limbs.Clear();
	}

	public void SetInventory(Inventory source)
	{
		EmptyInventory();
		foreach (var item in source.limbs)
		{
			limbs.Add(item.Key, item.Value);
		}
	}

	public void AddLimb(string limb, int qty = 1)
	{
		if (!limbs.ContainsKey(limb)) Debug.LogError($"Inventory does not contain {limb}, check spelling in ScriptableObjects/CurrentInventory");
		limbs[limb] += qty;
	}

	public bool TakeLimb(string limb, int qty = 1)
	{
		if (!limbs.ContainsKey(limb))
		{
			Debug.LogError($"Inventory does not contain {limb}, check spelling in ScriptableObjects/CurrentInventory");
			return false;
		}

		if (limbs[limb] < qty) return false;

		limbs[limb] -= qty;
		return true;
	}
}
