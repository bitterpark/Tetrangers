using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipModel : ShipModel {

	public delegate void WeaponFiredDeleg(int damage);
	public static event WeaponFiredDeleg EPlayerWeaponFired;
	public static event UnityEngine.Events.UnityAction EPlayerDied;

	public static bool energyGainPerSecondsSavedEnabled = false;

	//readonly int energyGainPerRow;
	//public static float energyGainPerSecondSaved;
	int energyGainPerSecondSaved;
	readonly int maxAmountOfSavedSeconds = 5;

	public static PlayerShipModel GetPlayerShipModelInstance()
	{
		return new PlayerShipModel();
	}

	public PlayerShipModel()
	{
		InitializeEventSubscriptions();
	}

	protected override void InitializeEventSubscriptions()
	{
		base.InitializeEventSubscriptions();
		Grid.ERowsCleared += GainEnergyOnRowClears;
		FigureController.EFigureSettledTimeClocked += GainEnergyOnFigureSettle;
		EnemyShipModel.EEnemyWeaponFired += TakeDamage;
		BattleManager.EEngagementModeStarted += GainGreenOnEngagementStart;
		BattleManager.EBattleFinished += ResetToStartingStatsKeepHealth;
		MissionManager.EMissionStarted += ResetToStartingStats;
	}

	protected override void InitializeClassStats()
	{
		SetStartingStats(BalanceValuesManager.Instance.playerShipHealth, BalanceValuesManager.Instance.playerBlueMax, BalanceValuesManager.Instance.playerGreenMax
			,SpriteDB.Instance.shipsprite,"Player ship");

		blueEnergyGain = BalanceValuesManager.Instance.playerBlueGain;
		greenEnergyGain = BalanceValuesManager.Instance.playerGreenGain;
		energyGainPerSecondSaved = BalanceValuesManager.Instance.playerBlueGainPerSecondSaved;
		AddWeapons(new LaserGun());
		AddOtherEquipment(new Overdrive(), new Coolant());
	}


	public override void DisposeModel()
	{
		base.DisposeModel();
		Grid.ERowsCleared -= GainEnergyOnRowClears;
		FigureController.EFigureSettledTimeClocked -= GainEnergyOnFigureSettle;
		EnemyShipModel.EEnemyWeaponFired -= TakeDamage;
		BattleManager.EEngagementModeStarted -= GainGreenOnEngagementStart;

		BattleManager.EBattleFinished -= ResetToStartingStatsKeepHealth;
		MissionManager.EMissionStarted -= ResetToStartingStats;

		EPlayerWeaponFired = null;
	}
	
	void GainGreenOnEngagementStart()
	{
		GainGreenEnergy(2);
	}

	void GainEnergyOnRowClears(int rowsCount)
	{
		GainGreenEnergy(rowsCount);
		//ChangeGreenEnergy(energyGainPerRow*rowsCount);
	}

	void GainEnergyOnFigureSettle(float timeSinceDropped)
	{
		
		//if (blueDelta>0)
		//ChangeBlueEnergy(blueDelta);
		if (energyGainPerSecondsSavedEnabled)
		{
			int blueDelta = Mathf.FloorToInt((maxAmountOfSavedSeconds - Mathf.RoundToInt(timeSinceDropped))*energyGainPerSecondSaved);
			if (blueDelta > 0)
				GainBlueEnergy(blueDelta,true);
		}

		GainBlueEnergy();
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
