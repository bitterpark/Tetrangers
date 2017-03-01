using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipModel : ShipModel {

	const int energyGainPerPlayerMove = 5;

	public EnemyShipModel(int shipHealthMax, int shipEnergy, Sprite shipSprite, string shipName)
		: base(shipHealthMax, shipEnergy, shipSprite, shipName) 
	{
		Grid.ENewPlayerMoveDone += GainEnergyOnPlayerMove;
	}

	void GainEnergyOnPlayerMove()
	{
		ChangeEnergy(energyGainPerPlayerMove);
	}

}
