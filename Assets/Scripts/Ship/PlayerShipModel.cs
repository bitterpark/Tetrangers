using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerShipModel : ShipModel {

	//public delegate void WeaponFiredDeleg(int damage);
	public static event UnityAction<int> EPlayerWeaponFired;
	public static event UnityAction<StatusEffect> EPlayerAppliedStatusEffectToEnemy;
	public static event UnityAction EPlayerDied;

	public static event UnityAction<int> EPlayerGainedBlueEnergy;
	public static event UnityAction<int> EPlayerGainedGreenEnergy;

	public static bool energyGainPerSecondsSavedEnabled = true;

	//readonly int energyGainPerRow;
	//public static float energyGainPerSecondSaved;
	int energyGainPerSecondSaved;
	readonly int maxAmountOfSavedSeconds = 8;

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
		//Grid.ERowsCleared += GainEnergyOnRowClears;
		//FigureController.EFigureSettledTimeClocked += GainEnergyOnFigureSettle;
		EnemyShipModel.EEnemyWeaponFired += TakeDamage;
		EnemyShipModel.EEnemyAppliedStatusEffectToPlayer += AddNewStatusEffect;
		//BattleManager.EEngagementModeEnded += GainGreenOnNewRound;
		BattleManager.EBattleFinished += ResetToStartingStatsKeepHealth;
		MissionManager.EMissionStarted += ResetToStartingStats;

		SettledBlock.EBlockCleared += GainEnergyFromDestroyedBlock;
	}

	protected override void InitializeClassStats()
	{
		SetStartingStats(BalanceValuesManager.Instance.playerShipHealth
			, BalanceValuesManager.Instance.playerShipShields
			, BalanceValuesManager.Instance.playerShieldGain
			, BalanceValuesManager.Instance.playerBlueMax
			, BalanceValuesManager.Instance.playerGreenMax
			, SpriteDB.Instance.shipsprite
			,"Player ship");

		blueEnergyGain = BalanceValuesManager.Instance.playerBlueGain;
		greenEnergyGain = BalanceValuesManager.Instance.playerGreenGain;
		energyGainPerSecondSaved = BalanceValuesManager.Instance.playerBlueGainPerSecondSaved;
		AddWeapons(new LaserGun(),new HeavyLaser());
		AddOtherEquipment(new Overdrive(), new BlitzMode(), new Forcefield(), new BlockEjector(), new ReactiveArmor());
	}


	public override void DisposeModel()
	{
		base.DisposeModel();
		//Grid.ERowsCleared -= GainEnergyOnRowClears;
		//FigureController.EFigureSettledTimeClocked -= GainEnergyOnFigureSettle;
		EnemyShipModel.EEnemyWeaponFired -= TakeDamage;
		EnemyShipModel.EEnemyAppliedStatusEffectToPlayer -= AddNewStatusEffect;
		//BattleManager.EEngagementModeEnded -= GainGreenOnNewRound;

		BattleManager.EBattleFinished -= ResetToStartingStatsKeepHealth;
		MissionManager.EMissionStarted -= ResetToStartingStats;

		SettledBlock.EBlockCleared -= GainEnergyFromDestroyedBlock;

		//EPlayerWeaponFired = null;
		//EPlayerDied = null;
		//EPlayerAppliedStatusEffectToEnemy = null;
	}
	
	void GainEnergyFromDestroyedBlock(BlockType blockType)
	{
		if (blockType == BlockType.Green)
			GainGreenEnergy();
		else if (blockType == BlockType.Blue)
			GainBlueEnergy();
		else if (blockType == BlockType.Shield)
			GainShields();
	}

	void GainGreenOnNewRound()
	{
		GainGreenEnergy();
	}

	void GainEnergyOnRowClears(int rowsCount)
	{
		//Debug.Log("Rowscount:"+rowsCount);
		int energyGain = rowsCount * greenEnergyGain;
		GainGreenEnergy(energyGain, true);
		if (EPlayerGainedGreenEnergy != null) EPlayerGainedGreenEnergy(energyGain);
		//ChangeGreenEnergy(energyGainPerRow*rowsCount);
	}

	void GainEnergyOnFigureSettle(float timeSinceDropped)
	{
		//if (blueDelta>0)
		//ChangeBlueEnergy(blueDelta);
		if (energyGainPerSecondsSavedEnabled)
		{
			float savedTime = Mathf.Max(0, maxAmountOfSavedSeconds - timeSinceDropped);
			int blueDelta = Mathf.FloorToInt(blueEnergyGain * savedTime / maxAmountOfSavedSeconds);//(maxAmountOfSavedSeconds - Mathf.RoundToInt(timeSinceDropped))*energyGainPerSecondSaved);
			//if (blueDelta > 0)
				GainBlueEnergy(blueDelta,true);
		}

		//GainBlueEnergy();
	}

	public override void GainBlueEnergy(int gains, bool absoluteValue)
	{
		int actualGain = TryGainBlueEnergy(gains, absoluteValue);
		if (EPlayerGainedBlueEnergy != null) EPlayerGainedBlueEnergy(actualGain);
	}

	protected override void DoWeaponFireEvent(int weaponDamage)
	{
		if (EPlayerWeaponFired != null)
			EPlayerWeaponFired(weaponDamage);
	}

	protected override void ApplyStatusEffectToOpponent(StatusEffect effect)
	{
		if (EPlayerAppliedStatusEffectToEnemy != null)
			EPlayerAppliedStatusEffectToEnemy(effect);
	}

	protected override void DoDeathEvent()
	{
		if (EPlayerDied != null)
			EPlayerDied();
	}

	
}
