using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class BodyPartSpawner : MonoBehaviour, IPointerClickHandler
{
    public GameObject PartReference;

    public Transform SpawnParent;
    public string partName;
    public TextMeshProUGUI stock;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!GameManager.instance.TakeLimb(partName))
            return;
        GameObject newPart = Instantiate(PartReference, SpawnParent);

        newPart.GetComponent<SnapingParts>().defaultParent = SpawnParent;
        newPart.GetComponent<SnapingParts>().inventoryID = partName;

        Debug.Log("Spawner Clicked");
    }

    public void setStock(int LimbStock)
    {
        stock.text = LimbStock.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
