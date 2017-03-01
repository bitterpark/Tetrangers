using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipModel : ShipModel {

	
	const int energyGainPerRow = 15;

	public PlayerShipModel(int shipHealthMax, int shipEnergy, Sprite shipSprite, string shipName)
		: base(shipHealthMax, shipEnergy, shipSprite, shipName) 
	{
		Grid.ERowsCleared += GainEnergyOnRowClears;
	}

	void GainEnergyOnRowClears(int rowsCount)
	{
		ChangeEnergy(energyGainPerRow*rowsCount);
	}
}
