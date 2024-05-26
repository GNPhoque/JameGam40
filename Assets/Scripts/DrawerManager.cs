using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerManager : MonoBehaviour
{
    public Transform drawer;
    public float refreshRate = 1;

    private Dictionary<string, GameObject> _drawerComponents;
    private float _refreshCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        _drawerComponents = new Dictionary<string, GameObject>();
        foreach (Transform child in drawer)
        {
            BodyPartSpawner spawner = child.gameObject.GetComponent<BodyPartSpawner>();
            _drawerComponents[spawner.partName] = child.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _refreshCounter -= Time.deltaTime;
        if (_refreshCounter > 0)
            return;
        foreach(string key in _drawerComponents.Keys)
        {
            int stock = GameManager.instance.GetLimbStock(key);
            if (stock == 0)
            {
                _drawerComponents[key].SetActive(false);
                continue;
            }
            _drawerComponents[key].SetActive(true);
            _drawerComponents[key].GetComponent<BodyPartSpawner>().setStock(stock);
        }
        _refreshCounter = refreshRate;                
    }
}
