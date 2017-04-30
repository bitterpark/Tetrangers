using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class PlayerShipSectorModel: ShipSectorModel, ICanUseEquipment
{
	public int index { get; private set; }
	

	public PlayerShipSectorModel(ShipModel parentShip, int index) : base(parentShip)
	{
		this.index = index;
	}

	protected override EquipmentUser CreateAppropriateEquipmentUser(ShipModel parentShip)
	{
		return new PlayerEquipmentUser(parentShip, this);
	}

	public override void InitializeForBattle()
	{
		Grid.Instance.GridSegments[index].EBlocksCleared += HandleDestroyedBlocks;
		EquipmentUser.EAppliedStatusEffectToPlayerShipSector += HandleSectorStatusEffectApplication;
		base.InitializeForBattle();
	}

	public override void Dispose()
	{
		
		Grid.Instance.GridSegments[index].EBlocksCleared -= HandleDestroyedBlocks;
		EquipmentUser.EAppliedStatusEffectToPlayerShipSector -= HandleSectorStatusEffectApplication;
		base.Dispose();
	}

	void HandleSectorStatusEffectApplication(StatusEffect effect, int appliesToSectorIndex)
	{
		if (index == appliesToSectorIndex)
			effectsManager.AddNewStatusEffect(effect);
	}

	protected override void HandleHealthRunningOut()
	{
		isDamaged = true;
		Grid.Instance.GridSegments[index].isUsable = false;
	}

	

	PlayerShipModel.TotalEnergyGain HandleDestroyedBlocks(int blueBlocks, int greenBlocks, int shieldBlocks, int shipBlocks, int gridSegmentIndexUnused)
	{
		int blueGain = 0;
		int greenGain = 0;
		int shieldGain = 0;

		blueGain = GainEnergyFromDestroyedBlocks(BlockType.Blue, blueBlocks);
		greenGain = GainEnergyFromDestroyedBlocks(BlockType.Green, greenBlocks);
		shieldGain = GainEnergyFromDestroyedBlocks(BlockType.Shield, shieldBlocks);

		return new PlayerShipModel.TotalEnergyGain(blueGain, greenGain, shieldGain, 0);
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

	
}

