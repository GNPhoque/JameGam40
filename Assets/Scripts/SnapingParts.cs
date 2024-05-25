using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SnapingParts : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler
{
    public static float snapDistance = 1f;
    public List<SnapPositions> PartAttachments;
    public Transform defaultParent;

    private LineRenderer _InfoLine;

    private SnapPositions _childLink;
    private Vector3 _previousMousePosition = Vector2.zero;

    private void Awake()
    {
        _InfoLine = GetComponent<LineRenderer>();
        if (_InfoLine == null)
            _InfoLine = gameObject.AddComponent<LineRenderer>();
        populateChild();
    }

    private void populateChild()
    {
        _childLink = null;

        foreach (SnapPositions part in PartAttachments)
        {
            if (part.snapType == EsnapType.child)
            {
                if (_childLink != null)
                    Debug.LogError($"Only one child attachment is allowed");
                _childLink = part;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_childLink != null && _childLink.sewed)
            return;

        Vector3 newMousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        newMousePosition.z = 0;
        transform.position += (newMousePosition - _previousMousePosition);
        _previousMousePosition = newMousePosition;

        if (_childLink == null)
            return;

        SnapPositions parent = FindNeerestAttach();
        if (_childLink.snappedTo != null)
        {
            Debug.Log("Reset snap");
            _childLink.snappedTo.snappedTo = null;
            _childLink.snappedTo = null;
        }
        if (parent != null)
        {
            Debug.Log("Snap");
            _childLink.snappedTo = parent;
            _childLink.snappedTo.snappedTo = _childLink;
            AttachmentLine();
            rotationPrevious();
            return;
        }
        if (gameObject.GetComponent<LineRenderer>() != null)
        {
            _InfoLine.enabled = false;
        }
    }

    public void rotationPrevious()
    {
        transform.rotation = _childLink.snappedTo.transform.rotation;
    }

    public void AttachmentLine()
    {
        _InfoLine.enabled = true;
        _InfoLine.material = new Material(Shader.Find("Sprites/Default"));
        _InfoLine.widthMultiplier = 0.1f;

        _InfoLine.startColor = Color.red;
        _InfoLine.endColor = Color.red;
        _InfoLine.SetPosition(0, _childLink.transform.position);
        _InfoLine.SetPosition(1, _childLink.snappedTo.transform.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_childLink)
            return;
        _InfoLine.enabled = false;
        if (_childLink.snappedTo == null)
            return;
        transform.SetParent(_childLink.snappedTo.transform);
        transform.position = _childLink.snappedTo.transform.position;
        transform.rotation = _childLink.snappedTo.transform.rotation;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        if (_childLink != null && _childLink.sewed)
            return;
        transform.SetParent(defaultParent);
        _previousMousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        _previousMousePosition.z = 0;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    private SnapPositions FindNeerestAttach()
    {
        SnapPositions parentAttach = null;
        float NeerestDistance = float.MaxValue;
        foreach (SnapPositions parent in SnapPositions.parentSnapers)
        {
            if (parent.snappedTo != null && parent.snappedTo != _childLink)
                continue;

            if (PartAttachments.Contains(parent))
                continue;

            float dist = Vector2.Distance(_childLink.transform.position, parent.transform.position);

            if (dist > snapDistance || dist > NeerestDistance)
                continue;
            NeerestDistance = dist;
            parentAttach = parent;
        }
        return parentAttach;
    }
}
