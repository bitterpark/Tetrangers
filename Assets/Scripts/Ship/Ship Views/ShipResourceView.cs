using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipResourceView : MonoBehaviour
{
	[SerializeField]
	HorizontalEnergyBarView shipEnergyBar;

	[SerializeField]
	Text ammoText;
	[SerializeField]
	Text partsText;

	public void SetShipEnergy(int newEnergy, int maxEnergy)
	{
		shipEnergyBar.SetBarValue(newEnergy, maxEnergy);
	}

	public void SetEnergyGain (int energyGain)
	{
		shipEnergyBar.SetGain(energyGain);
	}

	public void SetAmmo(int current, int max)
	{
		ammoText.text = current + "/" + max;
	}
	public void SetParts(int current, int max)
	{
		partsText.text = current + "/" + max;
	}

}
