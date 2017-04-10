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

	public static event UnityAction<int> EPlayerBlueGainChanged;
	public static event UnityAction<int> EPlayerGreenGainChanged;
	public static event UnityAction<int> EPlayerShieldGainChanged;
	/*
	public static event UnityAction<int> EPlayerGainedGreenEnergy;
	public static event UnityAction<int> EPlayerGainedShieldEnergy;*/
	//public static event UnityAction<int, int, int> EPlayerGainedEnergy;

	public static bool energyGainPerSecondsSavedEnabled = true;

	public static PlayerShipModel main;

	//readonly int energyGainPerRow;
	//public static float energyGainPerSecondSaved;
	int energyGainPerSecondSaved;
	readonly int maxAmountOfSavedSeconds = 8;

	public static PlayerShipModel CreatePlayerShipModelInstance()
	{
		return new PlayerShipModel();
	}

	public static int GetPlayerResourceIncome(BlockType resourceType)
	{
		switch (resourceType)
		{
			case BlockType.Blue: return main.blueEnergyGain;
			case BlockType.Green: return main.greenEnergyGain;
			case BlockType.Shield: return main.shipShieldsCurrentGain;
		}
		return 0;
	}
	public PlayerShipModel()
	{
		main = this;
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

		Grid.EBlocksCleared += HandleDestroyedBlocks;

		EEnergyGainChanged += HandleEnergyGainChange;
		EShieldsGainChanged += HandleShieldGainChange;
		//SettledBlock.EBlockCleared += GainEnergyFromDestroyedBlock;
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
		Grid.EBlocksCleared -= HandleDestroyedBlocks;

		EEnergyGainChanged -= HandleEnergyGainChange;
		EShieldsGainChanged -= HandleShieldGainChange;
		//SettledBlock.EBlockCleared -= GainEnergyFromDestroyedBlock;

		//EPlayerWeaponFired = null;
		//EPlayerDied = null;
		//EPlayerAppliedStatusEffectToEnemy = null;
	}

	TotalEnergyGain HandleDestroyedBlocks(int blueBlocks, int greenBlocks, int shieldBlocks)
	{
		int blueGain = 0;
		int greenGain = 0;
		int shieldGain = 0;

		for (int i = 0; i < blueBlocks; i++)
			blueGain += GainEnergyFromDestroyedBlock(BlockType.Blue);
		for (int i = 0; i < greenBlocks; i++)
			greenGain += GainEnergyFromDestroyedBlock(BlockType.Green);
		for (int i = 0; i < shieldBlocks; i++)
			shieldGain += GainEnergyFromDestroyedBlock(BlockType.Shield);

		return new TotalEnergyGain(blueGain, greenGain, shieldGain);
	}

	public struct TotalEnergyGain
	{
		public int greenGain;
		public int blueGain;
		public int shieldGain;

		public TotalEnergyGain(int blue,int green, int shield)
		{
			blueGain = blue;
			greenGain = green;
			shieldGain = shield;
		}
	}

	int GainEnergyFromDestroyedBlock(BlockType blockType)
	{
		if (blockType == BlockType.Green)
			return GainGreenEnergy();
		if (blockType == BlockType.Blue)
			return GainBlueEnergy();
		if (blockType == BlockType.Shield)
			return GainShields();

		return 0;
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
	/*
	public override void GainShields(int gain)
	{
		int actualGain = ChangeShields(gain);
		if (EPlayerGainedShieldEnergy != null) EPlayerGainedShieldEnergy(actualGain);
	}

		public override void GainGreenEnergy(int gains, bool absoluteValue)
	{
		int actualGain = TryGainBlueEnergy(gains, absoluteValue);
		if (EPlayerGainedGreenEnergy != null) EPlayerGainedGreenEnergy(actualGain);
	}

	*/
	public override int GainBlueEnergy(int gains, bool absoluteValue)
	{
		int actualGain = base.GainBlueEnergy(gains, absoluteValue);
		if (EPlayerGainedBlueEnergy != null) EPlayerGainedBlueEnergy(actualGain);
		return actualGain;
	}

	void HandleShieldGainChange()
	{
		if (EPlayerShieldGainChanged != null) EPlayerShieldGainChanged(shipShieldsCurrentGain);
	}

	void HandleEnergyGainChange()
	{
		if (EPlayerBlueGainChanged != null) EPlayerBlueGainChanged(blueEnergyGain);
		if (EPlayerBlueGainChanged != null) EPlayerGreenGainChanged(greenEnergyGain);
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
