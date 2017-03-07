using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipModel : ShipModel {

	public delegate void WeaponFiredDeleg(int damage);
	public static event WeaponFiredDeleg EEnemyWeaponFired;
	public static event UnityEngine.Events.UnityAction EEnemyDied;

	const int energyGainPerPlayerMove = 5;

	public EnemyShipModel(int shipHealthMax, int shipEnergy, int shipEnergyMax, Sprite shipSprite, string shipName)
		: base(shipHealthMax, shipEnergy, shipEnergyMax, shipSprite, shipName) 
	{
		
	}

	public void ActivateModel()
	{
		TetrisManager.ECurrentPlayerMoveDone += GainEnergyOnPlayerMove;
		PlayerShipModel.EPlayerWeaponFired += TakeDamage;
	}

	public override void DisposeModel()
	{
		base.DisposeModel();
		TetrisManager.ECurrentPlayerMoveDone -= GainEnergyOnPlayerMove;
		PlayerShipModel.EPlayerWeaponFired -= TakeDamage;
	}

	void GainEnergyOnPlayerMove()
	{
		ChangeEnergy(energyGainPerPlayerMove);
	}

	protected override void DoWeaponFireEvent(int weaponDamage)
	{
		if (EEnemyWeaponFired != null)
			EEnemyWeaponFired(weaponDamage);
	}

	protected override void DoDeathEvent()
	{
		if (EEnemyDied != null)
			EEnemyDied();
	}

}
