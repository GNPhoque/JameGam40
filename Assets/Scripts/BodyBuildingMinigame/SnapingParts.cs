using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SnapingParts : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler
{
    public static float snapDistance = 1f;
    public List<SnapPositions> PartAttachments;

    private RectTransform _rectTransform;
    private RectTransform _parentTransform;
    private LineRenderer _InfoLine;

    private SnapPositions _childLink;

    private void OnValidate()
    {
        _rectTransform = GetComponent<RectTransform>();
        if (transform.parent == null)
            return;
        _parentTransform = transform.parent.GetComponent<RectTransform>();

        _childLink = null;

        _InfoLine = GetComponent<LineRenderer>();
        if (_InfoLine == null)
            _InfoLine = gameObject.AddComponent<LineRenderer>();

        foreach (SnapPositions part in PartAttachments)
        {
            if (part.snapType == EsnapType.child)
            {
                if (_childLink != null)
                    Debug.LogError($"Only one child attachment is allowed");
                _childLink = part;
            }
        }
        if (_childLink != null)
            _childLink.transform.localPosition = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_childLink == null)
            return;
        Vector2 ratioSize = new Vector2(_parentTransform.rect.width / Screen.width, _parentTransform.rect.height / Screen.height);
        _rectTransform.anchoredPosition += eventData.delta * ratioSize;

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
            return;
        }
        if (gameObject.GetComponent<LineRenderer>() != null)
        {
            _InfoLine.enabled = false;
        }
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
        if (_childLink.snappedTo == null)
        {
            _InfoLine.enabled = false;
            return;
        }
        _rectTransform.SetParent(_childLink.snappedTo.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Inverse(_childLink.transform.localRotation);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        _rectTransform.SetParent(_parentTransform);
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
