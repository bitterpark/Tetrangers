using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipSectorModel: IHasEnergy, ICanUseEquipment
{

	public ICanSpendEnergy energyUser { get { return energyManager; } }
	public SectorEnergyManager energyManager { get; private set; }
	
	public StatusEffectManager effectsManager { get; private set;}
	public ShipHealthManager healthManager { get; private set; }
	public ShipEquipmentModel sectorEquipment;
	public EquipmentUser equipmentUser { get; private set; }

	public bool isDamaged { get; protected set; }

	const int sectorHealth = 150;
	const int sectorShields = 150;
	//const int sectorShieldsGain = 50;

	public ShipSectorModel(ShipModel parentShip)
	{
		energyManager = new SectorEnergyManager
			(
			BalanceValuesManager.Instance.playerBlueGain
			,BalanceValuesManager.Instance.playerBlueMax
			,BalanceValuesManager.Instance.playerGreenGain
			,BalanceValuesManager.Instance.playerGreenMax
			);
		
		effectsManager = new StatusEffectManager(this);

		equipmentUser = CreateAppropriateEquipmentUser(parentShip);
		sectorEquipment = new ShipEquipmentModel(parentShip, this);

		healthManager = new ShipHealthManager(sectorHealth, sectorShields, 10);

		//sectorEquipment.AddEquipment(new LaserGun(), new BlockEjector(), new Overdrive());
		healthManager.EHealthDepleted += HandleHealthRunningOut;
		BattleManager.EEngagementModeStarted += TryRegenShields;

	}

	protected abstract EquipmentUser CreateAppropriateEquipmentUser(ShipModel parentShip);

	public virtual void InitializeForBattle()
	{
		//sectorEquipment.InitializeForBattle();
	}

	public virtual void Dispose()
	{
		healthManager.EHealthDepleted -= HandleHealthRunningOut;
		BattleManager.EEngagementModeStarted -= TryRegenShields;

		energyManager.Dispose();
		equipmentUser.Dispose();
		sectorEquipment.DisposeModel();
		//sectorEquipment.DisposeModel();
		healthManager.Dispose();
		//equipmentUser.Dispose();
	}

	protected virtual void TryRegenShields()
	{
		healthManager.RegenShields();
	}

	protected abstract void HandleHealthRunningOut();
	

}

