using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDropInOutAnimator : MonoBehaviour
{
	public static Action OnUIClosed;

	[Header("Drop In")]
	[SerializeField] AnimationCurve dropInAnimationCurve;
	[SerializeField] float dropInAnimationOffset;
	[SerializeField] float dropInAnimationMult;
	[SerializeField] float dropInAnimationDuration;
	[SerializeField] float currentDropInAnimationDuration;
	[SerializeField] bool dropInAnimating;

	[Header("Drop Out")]
	[SerializeField] AnimationCurve dropOutAnimationCurve;
	[SerializeField] float dropOutAnimationOffset;
	[SerializeField] float dropOutAnimationMult;
	[SerializeField] float dropOutAnimationDuration;
	[SerializeField] float currentDropOutAnimationDuration;
	[SerializeField] bool dropOutAnimating;

	RectTransform _rect;

	public RectTransform rect
	{
		get
		{
			if (_rect == null)  _rect = GetComponent<RectTransform>();
			return _rect;
		}
		set
		{
			_rect = value;
		}
	}

	public void DropIn()
	{
		rect.anchoredPosition = new Vector2(0, dropInAnimationOffset);
		currentDropInAnimationDuration = 0f;
		dropInAnimating = true;
		dropOutAnimating = false;
		gameObject.SetActive(true);
	}

	public void DropOut()
	{
		currentDropOutAnimationDuration = 0f;
		dropInAnimating = false;
		dropOutAnimating = true;
		OnUIClosed?.Invoke();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) GameManager.instance.CloseGraveyardMinigame();

		if (dropInAnimating)
		{
			currentDropInAnimationDuration += Time.deltaTime;
			rect.anchoredPosition = new Vector2(0, dropInAnimationCurve.Evaluate(currentDropInAnimationDuration / dropInAnimationDuration) * dropInAnimationMult + dropInAnimationOffset);
			if (currentDropInAnimationDuration > dropInAnimationDuration) dropInAnimating = false;
		}

		if (dropOutAnimating)
		{
			currentDropOutAnimationDuration += Time.deltaTime;
			rect.anchoredPosition = new Vector2(0,dropOutAnimationCurve.Evaluate(currentDropOutAnimationDuration / dropOutAnimationDuration) * dropOutAnimationMult + dropOutAnimationOffset);
			if (currentDropOutAnimationDuration > dropOutAnimationDuration)
			{
				dropOutAnimating = false;
				gameObject.SetActive(false);
			}
		}
	}

}
