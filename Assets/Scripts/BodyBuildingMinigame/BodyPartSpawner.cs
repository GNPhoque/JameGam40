using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BodyPartSpawner : MonoBehaviour, IPointerClickHandler
{
    public GameObject PartReference;

    public RectTransform SpawnParent;

    public void OnPointerClick(PointerEventData eventData)
    {
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
