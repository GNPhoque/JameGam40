using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI graveyardEnergyText;
	[SerializeField] GameObject shopUI;

	public void ShowGraveyardEnergy()
	{
		graveyardEnergyText.gameObject.SetActive(true);
	}

	public void HideGraveyardEnergy()
	{
		graveyardEnergyText.gameObject.SetActive(false);
	}

	public void UpdateUraveyardEnergyText(int value)
	{
		graveyardEnergyText.text = $"Energy : {value}";
	}

	public void ShowShopUI()
	{
		shopUI.GetComponent<UIDropInOutAnimator>().DropIn();
	}

	public void HideShopUI()
	{
		shopUI.GetComponent<UIDropInOutAnimator>().DropOut();
	}
}
