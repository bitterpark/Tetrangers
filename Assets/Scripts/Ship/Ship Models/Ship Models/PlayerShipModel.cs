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

		//shipEquipment.AddEquipment(new Heatsink(), new NukeLauncher());
		//shipEquipment.AddEquipment(new RepairDrones());

		int sectorCount = Grid.segmentCount;
		shipSectors = new PlayerShipSectorModel[sectorCount];
		for (int i = 0; i < sectorCount; i++)
		{
			shipSectors[i] = new PlayerShipSectorModel(this, i);
			shipSectors[i].ESectorDamaged += HandleLosingSector;
		}
		shipSectors[0].sectorEquipment.AddEquipment(new LaserGun(), new ShieldGenerator());
		shipSectors[1].sectorEquipment.AddEquipment(new ClusterLauncher(), new RepairDrones());
		shipSectors[2].sectorEquipment.AddEquipment(new DualRailgun(), new Stabilizer());
	}

	protected override void InitializeForBattle()
	{
		base.InitializeForBattle();
		EnemyEquipmentUser.EEnemyWeaponFired += GetHitByWeapon;
		EnemyEquipmentUser.EEnemyAppliedStatusEffectToPlayer += statusEffectManager.AddNewStatusEffect;
		EquipmentUser.EAppliedStatusEffectToPlayerShipSector += ApplyStatusEffectToRandomFunctioningSector;

		BattleManager.EBattleFinished += ResetToStartingStatsKeepHealth;
		MissionManager.EMissionStarted += ResetToStartingStats;

		Clearer.EBlocksCleared += HandleDestroyedBlocks;

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
		EnemyEquipmentUser.EEnemyWeaponFired -= GetHitByWeapon;
		EnemyEquipmentUser.EEnemyAppliedStatusEffectToPlayer -= statusEffectManager.AddNewStatusEffect;
		EquipmentUser.EAppliedStatusEffectToPlayerShipSector -= ApplyStatusEffectToRandomFunctioningSector;

		BattleManager.EBattleFinished -= ResetToStartingStatsKeepHealth;
		MissionManager.EMissionStarted -= ResetToStartingStats;

		Clearer.EBlocksCleared -= HandleDestroyedBlocks;

		//FigureSettler.EOverflowingBlocks -= HandleOverflowDamage;
	}

	void ApplyStatusEffectToRandomFunctioningSector(StatusEffect effect)
	{
		List<int> eligibleSectorIndices = new List<int>();
		foreach (PlayerShipSectorModel sector in shipSectors)
			if (!sector.isDamaged) eligibleSectorIndices.Add(sector.index);

		int randomIndex = eligibleSectorIndices[Random.Range(0, eligibleSectorIndices.Count)];
		shipSectors[randomIndex].HandleSectorStatusEffectApplication(effect);
	}

	protected override void GetHitByWeapon(AttackInfo attack)
	{
		GetWeakestSectorHealthManager().TakeDamage(attack);
	}

	HealthAndShieldsManager GetWeakestSectorHealthManager()
	{
		//Debug.Log("Getting weakest ship sector");
		HealthAndShieldsManager weakestManager = shipSectors[0].healthManager;
		foreach (ShipSectorModel sector in shipSectors)
		{
			if (!sector.isDamaged)
			{
				int weakestTotalHitpoints = weakestManager.health + weakestManager.shields;
				int checkedTotalHitpoints = sector.healthManager.health + sector.healthManager.shields;
				if (checkedTotalHitpoints < weakestTotalHitpoints || (checkedTotalHitpoints == weakestTotalHitpoints && Random.value<0.5f))
					weakestManager = sector.healthManager;
			}
		}
		return weakestManager;
	}

	void HandleLosingSector()
	{
		//Debug.Log("Handling losing sector");
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

	protected override void DoDeathEvent()
	{
		if (EPlayerDied != null)
			EPlayerDied();
	}

	PlayerShipModel.TotalEnergyGain HandleDestroyedBlocks(GridSegment.ClearedCellsInfo clearInfo)
	{
		int shipEnergyGain = 0;

		shipEnergyGain = GainEnergyFromDestroyedBlocks(BlockType.ShipEnergy, clearInfo.shipBlocksCount);

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
