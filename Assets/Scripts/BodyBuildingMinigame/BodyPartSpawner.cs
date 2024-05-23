using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BodyPartSpawner : MonoBehaviour, IPointerClickHandler
{
    public GameObject PartReference;

    public Transform SpawnParent;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject newPart = Instantiate(PartReference, SpawnParent);

        newPart.GetComponent<SnapingParts>().defaultParent = SpawnParent;

        Debug.Log("Spawner Clicked");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
