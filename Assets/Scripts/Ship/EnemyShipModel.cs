using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyShipModel : ShipModel {

	public delegate void WeaponFiredDeleg(int damage);
	public static event WeaponFiredDeleg EEnemyWeaponFired;
	public static event UnityEngine.Events.UnityAction EEnemyDied;

	public bool energyGainForFigureHoverEnabled = false;
	//readonly int greenEnergyGainPerPlayerMove;
	//readonly int blueEnergyGainPerSecondOfHover;

	public static EnemyShipModel GetEnemyShipModelInstance()
	{
		float randomValue = UnityEngine.Random.value;

		EnemyShipModel result = null;

		if (randomValue < 0.5f)
			result = new AssaultShip();
		else
			if (randomValue < 1f)
				result = new HeavyShip();

		return result;
	}

	public EnemyShipModel()
	{
		greenEnergyGain = BalanceValuesManager.Instance.enemyGreenGainPerEngagement;
		blueEnergyGain = BalanceValuesManager.Instance.enemyBlueGainPerMove;
	}

	public void ActivateModel()
	{
		InitializeEventSubscriptions();
	}

	protected override void InitializeEventSubscriptions()
	{
		base.InitializeEventSubscriptions();
		TetrisManager.ECurrentPlayerMoveDone += GainEnergyOnPlayerMove;
		PlayerShipModel.EPlayerWeaponFired += TakeDamage;
		BattleManager.EEngagementModeStarted += GainGreenOnEngagementStart;
		FigureController.EFigureHoveredForOneSecond += GainEnergyOnFigureHover;
	}

	public override void DisposeModel()
	{
		base.DisposeModel();
		TetrisManager.ECurrentPlayerMoveDone -= GainEnergyOnPlayerMove;
		PlayerShipModel.EPlayerWeaponFired -= TakeDamage;
		BattleManager.EEngagementModeStarted -= GainGreenOnEngagementStart;
		FigureController.EFigureHoveredForOneSecond -= GainEnergyOnFigureHover;
	}

	void GainEnergyOnPlayerMove()
	{
		GainBlueEnergy();
		//ChangeGreenEnergy(greenEnergyGainPerPlayerMove);
		
	}

	void GainGreenOnEngagementStart()
	{
		GainGreenEnergy();
	}
	
	void GainEnergyOnFigureHover()
	{
		if (energyGainForFigureHoverEnabled)
			GainBlueEnergy(BalanceValuesManager.Instance.enemyBlueGainPerHoverSecond,true);
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

public class AssaultShip : EnemyShipModel
{
	protected override void InitializeClassStats()
	{
		SetStartingStats(BalanceValuesManager.Instance.assaultHealth
			,BalanceValuesManager.Instance.assaultMaxBlue
			,BalanceValuesManager.Instance.assaultMaxGreen
			, SpriteDB.Instance.shipsprite,"Assault Ship");

		AddWeapons(new PlasmaCannon());
		AddOtherEquipment(new Siphon());
	}
}

public class HeavyShip : EnemyShipModel
{
	protected override void InitializeClassStats()
	{
		SetStartingStats(BalanceValuesManager.Instance.heavyHealth
			, BalanceValuesManager.Instance.heavyMaxBlue
			, BalanceValuesManager.Instance.heavyMaxGreen
			, SpriteDB.Instance.shipsprite, "Heavy Ship");

		AddWeapons(new LaserGun(), new PlasmaCannon());
		AddOtherEquipment(new ReactiveArmor());
	}
}