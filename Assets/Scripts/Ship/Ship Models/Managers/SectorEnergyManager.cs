using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SectorEnergyManager : ShipEnergyManager
{

	int blueEnergyGainModifier = 0;
	int greenEnergyGainModifier = 0;

	ShipEnergyManager playerShipEnergyManager = PlayerShipModel.main.energyManager;

	public SectorEnergyManager(int blueGain, int blueMax, int greenGain, int greenMax) : base(blueGain, blueMax, greenGain, greenMax)
	{
	}


	protected override int GetBlueGainPropertyCalled()
	{
		return Mathf.Max(0, playerShipEnergyManager.blueEnergyGain + blueEnergyGainModifier);
	}
	protected override int GetGreenGainPropertyCalled()
	{
		return Mathf.Max(0, playerShipEnergyManager.greenEnergyGain + greenEnergyGainModifier);
	}
	protected override void SetBlueGainPropertyCalled(int value)
	{
		blueEnergyGainModifier = value;
	}
	protected override void SetGreenGainPropertyCalled(int value)
	{
		greenEnergyGainModifier = value;
	}

}

