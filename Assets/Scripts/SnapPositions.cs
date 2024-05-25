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
    public bool sewed = false;

    public void sew()
    {
        if (!snappedTo)
            return;
        if (sewed)
            return;
        sewed = true;
        snappedTo.sew();
    }

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
