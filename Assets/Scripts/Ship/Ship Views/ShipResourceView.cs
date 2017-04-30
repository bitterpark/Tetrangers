using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipResourceView : MonoBehaviour
{
	[SerializeField]
	HorizontalEnergyBarView shipEnergyBar;

	public void SetShipEnergy(int newEnergy, int maxEnergy)
	{
		shipEnergyBar.SetBarValue(newEnergy, maxEnergy);
	}

	public void SetEnergyGain (int energyGain)
	{
		shipEnergyBar.SetGain(energyGain);
	}

}
