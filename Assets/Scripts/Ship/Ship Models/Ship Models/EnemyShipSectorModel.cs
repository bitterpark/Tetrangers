using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class EnemyShipSectorModel : ShipSectorModel
{

	public EnemyShipSectorModel(ShipModel parentShip) : base(parentShip)
	{
	}

	protected override EquipmentUser CreateAppropriateEquipmentUser(ShipModel parentShip)
	{
		return new EnemyEquipmentUser(parentShip, this);
	}

	public override void InitializeForBattle()
	{
		PlayerEquipmentUser.EPlayerAppliedStatusEffectToEnemy += effectsManager.AddNewStatusEffect;
		TetrisManager.ECurrentPlayerMoveDone += GainEnergyOnPlayerMove;
		BattleManager.EEngagementModeEnded += GainEnergyOnNewRound;
		base.InitializeForBattle();
	}

	public override void Dispose()
	{
		PlayerEquipmentUser.EPlayerAppliedStatusEffectToEnemy -= effectsManager.AddNewStatusEffect;
		TetrisManager.ECurrentPlayerMoveDone -= GainEnergyOnPlayerMove;
		BattleManager.EEngagementModeEnded -= GainEnergyOnNewRound;
		base.Dispose();
	}

	void GainEnergyOnPlayerMove()
	{
		//GainBlueEnergy();
		//ChangeGreenEnergy(greenEnergyGainPerPlayerMove);

	}

	void GainEnergyOnNewRound()
	{
		energyManager.IncreaseBlueByGain();
		energyManager.IncreaseGreenByGain();
	}

	protected override void HandleDamagedStatusChange()
	{
		throw new NotImplementedException();
	}

	
}

