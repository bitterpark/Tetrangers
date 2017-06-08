using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class PlayerShipSectorModel: ShipSectorModel, ICanUseEquipment
{
	public event UnityEngine.Events.UnityAction ESectorDamaged;

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
		//EquipmentUser.EAppliedStatusEffectToPlayerShipSector += HandleSectorStatusEffectApplication;
		base.InitializeForBattle();
	}

	public override void Dispose()
	{
		FigureSettler.EOverflowingBlocks -= HandleOverflowingBlocksDamage;
		Grid.Instance.GridSegments[index].EBlocksCleared -= HandleDestroyedBlocks;
		Grid.Instance.GridSegments[index].EIsolatedBlockExpired -= HandleIsolatedBlocksExpiring;
		ESectorDamaged = null;
		//EquipmentUser.EAppliedStatusEffectToPlayerShipSector -= HandleSectorStatusEffectApplication;
		base.Dispose();
	}

	void HandleIsolatedBlocksExpiring()
	{
		//if (!isDamaged)
			//healthManager.TakeDamage(isolatedBlockExpiryDamage);
	}

	void HandleOverflowingBlocksDamage(int overflowingBlocksCount)
	{
		if (!isDamaged)
			healthManager.TakeDamage(overflowingBlockDamage*overflowingBlocksCount);
	}

	public void HandleSectorStatusEffectApplication(StatusEffect effect)
	{
		effectsManager.AddNewStatusEffect(effect);
	}

	protected override void HandleDamagedStatusChange(bool becameDamaged)
	{
		if (becameDamaged && ESectorDamaged != null)
			ESectorDamaged();

		Grid.Instance.GridSegments[index].isUsable = !becameDamaged;
		sectorEquipment.SetFunctioning(!becameDamaged);
	}

	

	PlayerShipModel.TotalEnergyGain HandleDestroyedBlocks(GridSegment.ClearedCellsInfo clearInfo, int gridSegmentIndexUnused)
	{
		int blueGain = 0;
		int greenGain = 0;
		int shieldGain = 0;

		blueGain = GainEnergyFromDestroyedBlocks(BlockType.Blue, clearInfo.blueBlocksCount);
		blueGain += GainEnergyFromDestroyedBlocks(BlockType.Blue, clearInfo.greenBlocksCount);
		blueGain += GainEnergyFromDestroyedBlocks(BlockType.Blue, clearInfo.shieldBlocksCount);
		blueGain += GainEnergyFromDestroyedBlocks(BlockType.Blue, clearInfo.shipBlocksCount);

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

