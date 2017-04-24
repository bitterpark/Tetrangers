using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerShipModel : ShipModel {

	public static event UnityAction EPlayerDied;
	
	public static event UnityAction<int> EPlayerShieldGainChanged;

	public static bool energyGainPerSecondsSavedEnabled = true;

	public static PlayerShipModel main;

	public readonly ShipSectorModel[] shipSectors;

	public static PlayerShipModel CreatePlayerShipModelInstance()
	{
		return new PlayerShipModel();
	}

	public static int GetPlayerResourceIncome(BlockType resourceType)
	{
		switch (resourceType)
		{
			case BlockType.Blue: return main.energyManager.blueEnergyGain;
			case BlockType.Green: return main.energyManager.greenEnergyGain;
			case BlockType.Shield: return main.healthManager.shieldsCurrentGain;
		}
		return 0;
	}
	public PlayerShipModel()
	{
		main = this;

		FigureSettler.EOverflowingBlocks += HandleOverflowDamage;
		healthManager.EShieldsGainChanged += HandleShieldGainChange;

		shipEquipment.AddEquipment(new Forcefield());

		int sectorCount = Grid.segmentCount;
		shipSectors = new ShipSectorModel[sectorCount];
		for (int i = 0; i < sectorCount; i++)
			shipSectors[i] = new ShipSectorModel(this, i);

		shipSectors[0].sectorEquipment.AddEquipment(new LaserGun(), new ReactiveArmor());
		shipSectors[1].sectorEquipment.AddEquipment(new PlasmaCannon(), new Forcefield());
		shipSectors[2].sectorEquipment.AddEquipment(new HeavyLaser(), new BlueEnergyTransmitter());
	}

	void HandleOverflowDamage(int overflowingBlocks)
	{
		healthManager.TakeDamage(overflowingBlocks * 25);
	}

	protected override void InitializeForBattle()
	{
		base.InitializeForBattle();
		EnemyEquipmentUser.EEnemyWeaponFired += HandleTakingDamage;
		EnemyEquipmentUser.EEnemyAppliedStatusEffectToPlayer += statusEffectManager.AddNewStatusEffect;

		BattleManager.EBattleFinished += ResetToStartingStatsKeepHealth;
		MissionManager.EMissionStarted += ResetToStartingStats;

		foreach (ShipSectorModel sector in shipSectors)
			sector.InitializeForBattle();
		Grid.EBlocksCleared += HandleDestroyedBlocks;
		Grid.EBlocksCleared += HandleDestroyedBlocks;
		Grid.EBlocksCleared += HandleDestroyedBlocks;

	}

	void HandleTakingDamage(int damage)
	{
		GetWeakestSectorHealthManager().TakeDamage(damage);
	}

	ShipHealthManager GetWeakestSectorHealthManager()
	{
		ShipHealthManager weakestManager = shipSectors[0].healthManager;
		foreach (ShipSectorModel sector in shipSectors)
		{
			if (!sector.isDamaged)
				if (sector.healthManager.health + sector.healthManager.shields < weakestManager.health + weakestManager.shields)
					weakestManager = sector.healthManager;
		}
		return weakestManager;
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

		shipEquipment.AddEquipment(new Overdrive());
		
	}

	protected override ShipEnergyManager SetupEnergyManager(int blueMax, int greenMax)
	{
		return new ShipEnergyManager(
			BalanceValuesManager.Instance.playerBlueGain
			, blueMax
			, BalanceValuesManager.Instance.playerGreenGain
			, greenMax);
	}

	protected override ShipHealthManager SetupHealthManager(int healthMax, int shieldsMax)
	{
		return new ShipHealthManager(healthMax, shieldsMax, BalanceValuesManager.Instance.playerShieldGain);
	}

	protected override EquipmentUser SetupEquipmentUser()
	{
		return new PlayerEquipmentUser(this, this);
	}

	public override void DisposeModel()
	{
		base.DisposeModel();
		EnemyEquipmentUser.EEnemyWeaponFired -= HandleTakingDamage;
		EnemyEquipmentUser.EEnemyAppliedStatusEffectToPlayer -= statusEffectManager.AddNewStatusEffect;

		BattleManager.EBattleFinished -= ResetToStartingStatsKeepHealth;
		MissionManager.EMissionStarted -= ResetToStartingStats;

		Grid.EBlocksCleared -= HandleDestroyedBlocks;
		Grid.EBlocksCleared -= HandleDestroyedBlocks;
		Grid.EBlocksCleared -= HandleDestroyedBlocks;

		FigureSettler.EOverflowingBlocks -= HandleOverflowDamage;
	}

	TotalEnergyGain HandleDestroyedBlocks(int blueBlocks, int greenBlocks, int shieldBlocks)
	{
		int blueGain = 0;
		int greenGain = 0;
		int shieldGain = 0;


			//blueGain = GainEnergyFromDestroyedBlocks(BlockType.Blue, blueBlocks);
			//greenGain = GainEnergyFromDestroyedBlocks(BlockType.Green, greenBlocks);
			shieldGain = GainEnergyFromDestroyedBlocks(BlockType.Shield, shieldBlocks);

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

	int GainEnergyFromDestroyedBlocks(BlockType blockType, int blockCount)
	{
		if (blockType == BlockType.Green)
			return energyManager.GetActualGreenDelta(blockCount, false);
		if (blockType == BlockType.Blue)
			return energyManager.GetActualBlueDelta(blockCount, false);
		if (blockType == BlockType.Shield)
			return healthManager.GetActualShieldsDelta(blockCount, false);

		return 0;
	}

	void GainEnergyOnRowClears(int rowsCount)
	{
		energyManager.IncreaseGreenByGains(rowsCount);
	}

	void HandleShieldGainChange()
	{
		if (EPlayerShieldGainChanged != null) EPlayerShieldGainChanged(healthManager.shieldsCurrentGain);
	}

	protected override void DoDeathEvent()
	{
		if (EPlayerDied != null)
			EPlayerDied();
	}

	
}
