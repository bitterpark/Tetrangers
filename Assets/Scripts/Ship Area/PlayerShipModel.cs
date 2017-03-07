using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipModel : ShipModel {

	public delegate void WeaponFiredDeleg(int damage);
	public static event WeaponFiredDeleg EPlayerWeaponFired;
	public static event UnityEngine.Events.UnityAction EPlayerDied;

	const int energyGainPerRow = 15;

	public PlayerShipModel(int shipHealthMax, int shipEnergy, int shipEnergyMax, Sprite shipSprite, string shipName)
		: base(shipHealthMax, shipEnergy, shipEnergyMax, shipSprite, shipName) 
	{
		Grid.ERowsCleared += GainEnergyOnRowClears;
		EnemyShipModel.EEnemyWeaponFired += TakeDamage;
	}

	public override void DisposeModel()
	{
		base.DisposeModel();
		Grid.ERowsCleared -= GainEnergyOnRowClears;
		EnemyShipModel.EEnemyWeaponFired -= TakeDamage;
		EPlayerWeaponFired = null;
	}

	void GainEnergyOnRowClears(int rowsCount)
	{
		ChangeEnergy(energyGainPerRow*rowsCount);
	}

	protected override void DoWeaponFireEvent(int weaponDamage)
	{
		if (EPlayerWeaponFired != null)
			EPlayerWeaponFired(weaponDamage);
	}

	protected override void DoDeathEvent()
	{
		if (EPlayerDied != null)
			EPlayerDied();
	}
}
