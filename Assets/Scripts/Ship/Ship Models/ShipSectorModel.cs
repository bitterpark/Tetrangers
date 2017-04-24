using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipSectorModel: ICanUseEquipment, IHasEnergy
{

	public ShipEnergyManager energyManager { get; private set; }
	public ShipEquipmentModel sectorEquipment;
	public EquipmentUser equipmentUser { get; private set; }
	public StatusEffectManager effectsManager { get; private set;}
	public ShipHealthManager healthManager { get; private set; }

	public int index { get; private set; }

	public bool isDamaged { get; private set; }

	const int sectorHealth = 150;
	const int sectorShields = 150;
	//const int sectorShieldsGain = 50;

	public ShipSectorModel(ShipModel parentShip, int index)
	{
		this.index = index;
		energyManager = new SectorEnergyManager
			(
			0//BalanceValuesManager.Instance.playerBlueGain
			,BalanceValuesManager.Instance.playerBlueMax
			,0//BalanceValuesManager.Instance.playerGreenGain
			,BalanceValuesManager.Instance.playerGreenMax
			);
		equipmentUser = new PlayerEquipmentUser(parentShip, this);
		effectsManager = new StatusEffectManager(this);
		sectorEquipment = new ShipEquipmentModel(parentShip, this);
		healthManager = new ShipHealthManager(sectorHealth, sectorShields, parentShip.healthManager.shieldsCurrentGain);

		//sectorEquipment.AddEquipment(new LaserGun(), new BlockEjector(), new Overdrive());
		healthManager.EHealthDepleted += HandleHealthRunningOut;

	}

	void HandleHealthRunningOut()
	{
		isDamaged = true;
		Grid.Instance.GridSegments[index].isUsable = false;
		
	}

	void HandleSectorStatusEffectApplication(StatusEffect effect, int appliesToSectorIndex)
	{
		if (index == appliesToSectorIndex)
			effectsManager.AddNewStatusEffect(effect);
	}

	public void InitializeForBattle()
	{
		Grid.Instance.GridSegments[index].EBlocksCleared += HandleDestroyedBlocks;
		EquipmentUser.EAppliedStatusEffectToPlayerShipSector += HandleSectorStatusEffectApplication;
		sectorEquipment.InitializeForBattle();
	}

	PlayerShipModel.TotalEnergyGain HandleDestroyedBlocks(int blueBlocks, int greenBlocks, int shieldBlocks, int gridSegmentIndexUnused)
	{
		int blueGain = 0;
		int greenGain = 0;
		int shieldGain = 0;

		blueGain = GainEnergyFromDestroyedBlocks(BlockType.Blue, blueBlocks);
		greenGain = GainEnergyFromDestroyedBlocks(BlockType.Green, greenBlocks);
		shieldGain = GainEnergyFromDestroyedBlocks(BlockType.Shield, shieldBlocks);

		return new PlayerShipModel.TotalEnergyGain(blueGain, greenGain, shieldGain);
	}

	int GainEnergyFromDestroyedBlocks(BlockType blockType, int blockCount)
	{
		if (blockType == BlockType.Green)
			return energyManager.GetActualGreenDelta(blockCount, false);
		if (blockType == BlockType.Blue)
			return energyManager.GetActualBlueDelta(blockCount, false);

		return 0;
	}

	public void Dispose()
	{
		healthManager.EHealthDepleted -= HandleHealthRunningOut;

		Grid.Instance.GridSegments[index].EBlocksCleared -= HandleDestroyedBlocks;
		EquipmentUser.EAppliedStatusEffectToPlayerShipSector -= HandleSectorStatusEffectApplication;
		energyManager.Dispose();
		sectorEquipment.DisposeModel();
		healthManager.Dispose();
		equipmentUser.Dispose();
	}

}

