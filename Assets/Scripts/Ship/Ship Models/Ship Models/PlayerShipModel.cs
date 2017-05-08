using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerShipModel : ShipModel {

	public static event UnityAction EPlayerDied;

	public static PlayerShipModel main;

	public readonly PlayerShipSectorModel[] shipSectors;

	public ShipResourceManager resourceManager { get; private set; }

	public static PlayerShipModel CreatePlayerShipModelInstance()
	{
		return new PlayerShipModel();
	}
	//Remove this later
	public static int GetPlayerResourceIncome(BlockType resourceType)
	{
		switch (resourceType)
		{
			//case BlockType.Blue: return main.energyUser.blueEnergyGain;
			//case BlockType.Green: return main.energyUser.greenEnergyGain;
			//case BlockType.Shield: return main.healthManager.shieldsCurrentGain;
		}
		return 0;
	}
	public PlayerShipModel()
	{
		main = this;
		
		//FigureSettler.EOverflowingBlocks += HandleOverflowDamage;
		//healthManager.EShieldsGainChanged += HandleShieldGainChange;

		shipEquipment.AddEquipment(new Heatsink(), new NukeLauncher());
		shipEquipment.AddEquipment(new Stabilizer(), new RepairDrones());

		int sectorCount = Grid.segmentCount;
		shipSectors = new PlayerShipSectorModel[sectorCount];
		for (int i = 0; i < sectorCount; i++)
			shipSectors[i] = new PlayerShipSectorModel(this, i);

		shipSectors[0].sectorEquipment.AddEquipment(new LaserGun(), new ReactiveArmor());
		shipSectors[1].sectorEquipment.AddEquipment(new PlasmaCannon(), new Forcefield());
		shipSectors[2].sectorEquipment.AddEquipment(new HeavyLaser(), new BlueEnergyTransmitter());
	}

	protected override void InitializeForBattle()
	{
		base.InitializeForBattle();
		EnemyEquipmentUser.EEnemyWeaponFired += TakeDamage;
		EnemyEquipmentUser.EEnemyAppliedStatusEffectToPlayer += statusEffectManager.AddNewStatusEffect;

		BattleManager.EBattleFinished += ResetToStartingStatsKeepHealth;
		MissionManager.EMissionStarted += ResetToStartingStats;

		Grid.EBlocksCleared += HandleDestroyedBlocks;

		foreach (ShipSectorModel sector in shipSectors)
			sector.InitializeForBattle();
		

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
	}

	protected override ICanSpendEnergy CreateOrGetAppropriateEnergyUser(int blueMax, int greenMax)
	{
		resourceManager = new ShipResourceManager(
			BalanceValuesManager.Instance.playerShipEnergyGain
			, BalanceValuesManager.Instance.playerShipEnergyMax
			, BalanceValuesManager.Instance.playerAmmoMax
			, BalanceValuesManager.Instance.playerPartsMax
			);
		return resourceManager;
	}

	protected override EquipmentUser CreateAppropriateEquipmentUser()
	{
		return new PlayerEquipmentUser(this, this);
	}

	public override void DisposeModel()
	{
		base.DisposeModel();
		EnemyEquipmentUser.EEnemyWeaponFired -= TakeDamage;
		EnemyEquipmentUser.EEnemyAppliedStatusEffectToPlayer -= statusEffectManager.AddNewStatusEffect;

		BattleManager.EBattleFinished -= ResetToStartingStatsKeepHealth;
		MissionManager.EMissionStarted -= ResetToStartingStats;

		Grid.EBlocksCleared -= HandleDestroyedBlocks;

		//FigureSettler.EOverflowingBlocks -= HandleOverflowDamage;
	}

	protected override void TakeDamage(int damage)
	{
		GetWeakestSectorHealthManager().TakeDamage(damage);

		bool allSectorsDamaged = true;
		foreach (ShipSectorModel sector in shipSectors)
			if (!sector.isDamaged)
			{
				allSectorsDamaged = false;
				break;
			}
		if (allSectorsDamaged)
			DoDeathEvent();
	}

	HealthAndShieldsManager GetWeakestSectorHealthManager()
	{
		HealthAndShieldsManager weakestManager = shipSectors[0].healthManager;
		foreach (ShipSectorModel sector in shipSectors)
		{
			if (!sector.isDamaged)
				if (sector.healthManager.health + sector.healthManager.shields < weakestManager.health + weakestManager.shields)
					weakestManager = sector.healthManager;
		}
		return weakestManager;
	}

	protected override void DoDeathEvent()
	{
		if (EPlayerDied != null)
			EPlayerDied();
	}

	PlayerShipModel.TotalEnergyGain HandleDestroyedBlocks(int blueBlocks, int greenBlocks, int shieldBlocks, int shipBlocks)
	{
		int shipEnergyGain = 0;

		shipEnergyGain = GainEnergyFromDestroyedBlocks(BlockType.ShipEnergy, shipBlocks);

		return new PlayerShipModel.TotalEnergyGain(0, 0, 0, shipEnergyGain);
	}

	int GainEnergyFromDestroyedBlocks(BlockType blockType, int blockCount)
	{
		if (blockType == BlockType.ShipEnergy)
			return resourceManager.GetActualShipEnergyDelta(blockCount, false);

		return 0;
	}

	public struct TotalEnergyGain
	{
		public int greenGain;
		public int blueGain;
		public int shieldGain;
		public int shipEnergyGain;

		public TotalEnergyGain(int blue,int green, int shield, int ship)
		{
			blueGain = blue;
			greenGain = green;
			shieldGain = shield;
			shipEnergyGain = ship;
		}
	}

	
}
