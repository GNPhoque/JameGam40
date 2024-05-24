using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BodyPartSpawner : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    public GameObject PartReference;

    public Transform SpawnParent;
    public string partName;

    private List<GameObject> _spawned;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropedObject = eventData.pointerDrag;
        Debug.Log($"droped {dropedObject.name} on {name}");
        if (!_spawned.Contains(dropedObject))
            return;
        GameManager.instance.AddLimb(partName);
        _spawned.Remove(dropedObject);
        Destroy(dropedObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!GameManager.instance.TakeLimb(partName))
            return;
        GameObject newPart = Instantiate(PartReference, SpawnParent);

        newPart.GetComponent<SnapingParts>().defaultParent = SpawnParent;
        _spawned.Add(newPart);

        Debug.Log("Spawner Clicked");
    }

    // Start is called before the first frame update
    void Start()
    {
        _spawned = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
