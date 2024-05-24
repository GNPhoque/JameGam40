using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EsnapType
{
    parent,
    child
}
public class SnapPositions : MonoBehaviour
{
    public EsnapType snapType;

    public static List<SnapPositions> parentSnapers;

    public SnapPositions snappedTo = null;

    private void Awake()
    {
        if (snapType == EsnapType.parent)
            parentSnapers.Add(this);
    }

    private void OnDisable()
    {
        if (snapType == EsnapType.parent)
            parentSnapers.Remove(this);
    }
}
