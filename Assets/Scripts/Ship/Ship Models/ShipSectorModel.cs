using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipSectorModel: ICanUseEquipment, IHasEnergy
{

	public ShipEnergyManager energyManager { get; private set; }
	public ShipEquipmentModel sectorEquipment;
	public EquipmentUser equipmentUser { get; private set; }

	int index;

	public ShipSectorModel(ShipModel parentShip, int index)
	{
		this.index = index;
		energyManager = new ShipEnergyManager
			(
			BalanceValuesManager.Instance.playerBlueGain
			,BalanceValuesManager.Instance.playerBlueMax
			,BalanceValuesManager.Instance.playerGreenGain
			,BalanceValuesManager.Instance.playerGreenMax
			);
		equipmentUser = new PlayerEquipmentUser(parentShip, this);
		sectorEquipment = new ShipEquipmentModel(parentShip, this);
		//sectorEquipment.AddEquipment(new LaserGun(), new BlockEjector(), new Overdrive());

		
	}

	public void InitializeForBattle()
	{
		Grid.Instance.GridSegments[index].EBlocksCleared += HandleDestroyedBlocks;
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
		Grid.Instance.GridSegments[index].EBlocksCleared -= HandleDestroyedBlocks;
		energyManager.Dispose();
		sectorEquipment.DisposeModel();
	}

}

