using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class PlayerShipSectorModel: ShipSectorModel, ICanUseEquipment
{
	const int isolatedBlockExpiryDamage = 10;
	const int overflowingBlockDamage = 10;

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
		FigureSettler.EOverflowingBlocks += HandleOverflowingBlocksDamage;
		Grid.Instance.GridSegments[index].EBlocksCleared += HandleDestroyedBlocks;
		Grid.Instance.GridSegments[index].EIsolatedBlockExpired += HandleIsolatedBlocksExpiring;
		EquipmentUser.EAppliedStatusEffectToPlayerShipSector += HandleSectorStatusEffectApplication;
		base.InitializeForBattle();
	}

	public override void Dispose()
	{
		FigureSettler.EOverflowingBlocks -= HandleOverflowingBlocksDamage;
		Grid.Instance.GridSegments[index].EBlocksCleared -= HandleDestroyedBlocks;
		Grid.Instance.GridSegments[index].EIsolatedBlockExpired -= HandleIsolatedBlocksExpiring;
		EquipmentUser.EAppliedStatusEffectToPlayerShipSector -= HandleSectorStatusEffectApplication;
		base.Dispose();
	}

	void HandleIsolatedBlocksExpiring()
	{
		if (!isDamaged)
			healthManager.TakeDamage(isolatedBlockExpiryDamage);
	}

	void HandleOverflowingBlocksDamage(int overflowingBlocksCount)
	{
		if (!isDamaged)
			healthManager.TakeDamage(overflowingBlockDamage*overflowingBlocksCount);
	}

	void HandleSectorStatusEffectApplication(StatusEffect effect, int appliesToSectorIndex)
	{
		if (index == appliesToSectorIndex)
			effectsManager.AddNewStatusEffect(effect);
	}

	protected override void HandleDamagedStatusChange()
	{
		Grid.Instance.GridSegments[index].isUsable = !isDamaged;
		sectorEquipment.SetFunctioning(!isDamaged);
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

