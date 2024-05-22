using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	[SerializeField] CinemachineVirtualCamera camShop;
	[SerializeField] CinemachineVirtualCamera camWorkshop;
	[SerializeField] CinemachineVirtualCamera camGraveyard;

	private void Start()
	{
		FocusOnWorkshop();
	}

	private void Update()
	{
		if (!GameManager.instance.canChangeRoom) return;

		if(Input.GetKeyDown(KeyCode.Alpha1)) FocusOnShop();
		if(Input.GetKeyDown(KeyCode.Alpha2)) FocusOnWorkshop();
		if(Input.GetKeyDown(KeyCode.Alpha3)) FocusOnGraveyard();
	}

	void UnfocusCameras()
	{
		camShop.Priority = 0;
		camWorkshop.Priority = 0;
		camGraveyard.Priority = 0;
	}

	public void FocusOnShop()
	{
		UnfocusCameras();
		camShop.Priority = 10;
	}

	public void FocusOnWorkshop()
	{
		UnfocusCameras();
		camWorkshop.Priority = 10;
	}

	public void FocusOnGraveyard()
	{
		UnfocusCameras();
		camGraveyard.Priority = 10;
	}
}