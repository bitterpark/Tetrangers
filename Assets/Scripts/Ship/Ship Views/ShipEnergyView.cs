using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ShipEnergyView : MonoBehaviour
{
	[SerializeField]
	Transform blueEnergyBarObject;
	Text blueEnergyText;
	RectTransform blueEnergyBar;
	[SerializeField]
	Text blueEnergyGainText;

	[SerializeField]
	Transform greenEnergyBarObject;
	Text greenEnergyText;
	RectTransform greenEnergyBar;
	[SerializeField]
	Text greenEnergyGainText;

	void Awake()
	{
		Initialize();
	}

	void Initialize()
	{
		blueEnergyText = blueEnergyBarObject.transform.FindChild("Value").GetComponent<Text>();
		blueEnergyBar = blueEnergyBarObject.transform.FindChild("Underbar").FindChild("Bar").GetComponent<RectTransform>();

		greenEnergyText = greenEnergyBarObject.transform.FindChild("Value").GetComponent<Text>();
		greenEnergyBar = greenEnergyBarObject.transform.FindChild("Underbar").FindChild("Bar").GetComponent<RectTransform>();
	}

	public void SetBlueEnergy(int newEnergy, int maxEnergy)
	{
		blueEnergyText.text = newEnergy.ToString() + "/" + maxEnergy.ToString();
		float barPercentage;
		if (maxEnergy > 0)
			barPercentage = (float)newEnergy / (float)maxEnergy;
		else
			barPercentage = 1;
		blueEnergyBar.anchorMax = new Vector2(barPercentage, blueEnergyBar.anchorMax.y);
	}

	public void SetGreenEnergy(int newEnergy, int maxEnergy)
	{
		greenEnergyText.text = newEnergy.ToString() + "/" + maxEnergy.ToString();

		float barPercentage;
		if (maxEnergy > 0)
			barPercentage = (float)newEnergy / (float)maxEnergy;
		else
			barPercentage = 1;

		greenEnergyBar.anchorMax = new Vector2(barPercentage, greenEnergyBar.anchorMax.y);
	}

	public void SetEnergyGainLevels(int blueEnergyGain, int greenEnergyGain)
	{
		SetBlueEnergyGain(blueEnergyGain);
		SetGreenEnergyGain(greenEnergyGain);
	}
	void SetBlueEnergyGain(int gain)
	{
		blueEnergyGainText.text = gain.ToString();
	}
	void SetGreenEnergyGain(int gain)
	{
		greenEnergyGainText.text = gain.ToString();
	}

}

