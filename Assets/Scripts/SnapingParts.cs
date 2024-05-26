using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SnapingParts : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler
{
    public static float snapDistance = 1f;
    public List<SnapPositions> PartAttachments;
    public Transform defaultParent;
    [SerializeField] public SerializedDictionary<BodyParts, int> bodyElements;
    public BodyParts partType;

    public bool IsRoot {
        get { return _childLink == null; }
    }

    private LineRenderer _InfoLine;

    private SnapPositions _childLink;
    private Vector3 _previousMousePosition = Vector2.zero;

    private void Awake()
    {
        _InfoLine = GetComponent<LineRenderer>();
        if (_InfoLine == null)
            _InfoLine = gameObject.AddComponent<LineRenderer>();
        populateChild();
        initBodyPartDict();
        FillBodypartToDict(partType);
    }

    private void populateChild()
    {
        _childLink = null;

        foreach (SnapPositions part in PartAttachments)
        {
            part.snapManager = this;
            if (part.snapType == EsnapType.child)
            {
                if (_childLink != null)
                    Debug.LogError($"Only one child attachment is allowed");
                _childLink = part;
            }
        }
    }

    private void initBodyPartDict()
    {
        // bodyElements = new SerializedDictionary<int, int>();
        bodyElements.Clear();
        for (int i = 1; System.Enum.IsDefined(typeof(BodyParts), (BodyParts)i); i *= 2)
        {
            bodyElements.Add((BodyParts)i, 0);
        }
    }

    private void FillBodypartToDict(BodyParts parts)
    {
        for (int i = 1; System.Enum.IsDefined(typeof(BodyParts), (BodyParts)i); i *= 2)
        {
            if (((BodyParts)i & parts) != 0)
                bodyElements[(BodyParts)i] += 1;
        }
    }

    public void addBodyList(Dictionary<BodyParts, int> toAdd)
    {
        foreach (BodyParts key in toAdd.Keys)
            bodyElements[key] += toAdd[key];
        if (_childLink != null && _childLink.snappedTo != null)
            _childLink.snappedTo.snapManager.addBodyList(toAdd);
    }
    public void removeBodyList(Dictionary<BodyParts, int> toRemove)
    {
        foreach (BodyParts key in toRemove.Keys)
            bodyElements[key] -= toRemove[key];
        if (_childLink != null && _childLink.snappedTo != null)
            _childLink.snappedTo.snapManager.removeBodyList(toRemove);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_childLink != null && _childLink.sewed)
            return;

        Vector3 newMousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        newMousePosition.z = 0;
         //transform.position += (newMousePosition - _previousMousePosition);
         transform.position = newMousePosition;
        //_previousMousePosition = newMousePosition;

        if (_childLink == null)
            return;

        SnapPositions parent = FindNeerestAttach();
        if (_childLink.snappedTo != null)
        {
            _childLink.snappedTo.snappedTo = null;
            _childLink.snappedTo = null;
        }
        if (parent != null)
        {
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

    public void rotationPreview()
    {
        Quaternion rot = Quaternion.FromToRotation(_childLink.snappedTo.transform.up, _childLink.transform.up);
        transform.rotation *= rot;
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
        rotationPreview();
        transform.position -= _childLink.transform.position;
        transform.position += _childLink.snappedTo.transform.position;
        transform.SetParent(_childLink.snappedTo.transform);
        _childLink.snappedTo.snapManager.addBodyList(bodyElements);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_childLink != null && _childLink.sewed)
            return;
        if (_childLink != null && _childLink.snappedTo != null)
            _childLink.snappedTo.snapManager.removeBodyList(bodyElements);
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
