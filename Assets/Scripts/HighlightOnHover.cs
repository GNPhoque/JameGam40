using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image image;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite hovered;
    [SerializeField] Sprite unhovered;
    [SerializeField] Vector3 hoverScale;
    [SerializeField] Vector3 unhoverScale;
    [SerializeField] bool isUI;

    private void Awake()
    {
        if(isUI)image = GetComponent<Image>();
        else spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isUI)
        {
            image.sprite = hovered;
        }
        else
        {
            spriteRenderer.sprite= hovered;
        }

        transform.localScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
	{
		if (isUI)
		{
			image.sprite = unhovered;
		}
		else
		{
			spriteRenderer.sprite = unhovered;
		}

		transform.localScale = unhoverScale;
	}
}
