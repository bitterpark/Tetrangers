using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SectorEnergyView : MonoBehaviour
{
	[SerializeField]
	HorizontalEnergyBarView blueEnergyBar;
	[SerializeField]
	HorizontalEnergyBarView greenEnergyBar;

	public void SetBlueEnergy(int newEnergy, int maxEnergy)
	{
		blueEnergyBar.SetBarValue(newEnergy, maxEnergy);
	}

	public void SetGreenEnergy(int newEnergy, int maxEnergy)
	{
		greenEnergyBar.SetBarValue(newEnergy, maxEnergy);
	}

	public void SetEnergyGainLevels(int blueEnergyGain, int greenEnergyGain)
	{
		blueEnergyBar.SetGain(blueEnergyGain);
		greenEnergyBar.SetGain(greenEnergyGain);
	}
}

