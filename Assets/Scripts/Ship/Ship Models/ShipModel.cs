using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ShipModel: ICanUseEquipment, ICanHaveStatusEffects, IHasEnergy
{
	public event UnityAction EGeneratorLevelChanged;

	public string shipName { get; private set;}

	public int generatorLevel
	{
		get { return _generatorLevel; }
		set {_generatorLevel = Mathf.Clamp(value,1,2);}
	}

	int _generatorLevel = 1;

	public ShipEnergyManager energyManager { get; protected set; }
	public ShipHealthManager healthManager { get; protected set; }
	public StatusEffectManager statusEffectManager { get; protected set; }
	public EquipmentUser equipmentUser { get; protected set; }

	public Sprite shipSprite { get; private set; }

	bool initializedForBattle = false;

	public ShipEquipmentModel shipEquipment;

	public ShipModel()
	{
		shipEquipment = new ShipEquipmentModel(this, this);
		statusEffectManager = new StatusEffectManager(this);
		InitializeClassStats();
	}

	public void TryInitializeForBattle()
	{
		if (!initializedForBattle) InitializeForBattle();
	}

	protected virtual void InitializeForBattle()
	{
		initializedForBattle = true;
		BattleManager.EEngagementModeStarted += TryRegenShields;
		equipmentUser.InitializeForBattle();
		shipEquipment.InitializeForBattle();
	}

	protected abstract void InitializeClassStats();

	protected void SetStartingStats(int healthMax, int shieldsMax, int shieldsGain, int blueEnergyMax, int greenEnergyMax, Sprite sprite, string name)
	{
		energyManager = SetupEnergyManager(blueEnergyMax, greenEnergyMax);
		healthManager = SetupHealthManager(healthMax, shieldsMax);
		equipmentUser = SetupEquipmentUser();
		healthManager.EHealthDepleted += DoDeathEvent;

		this.shipSprite = sprite;
		this.shipName = name;

		ResetToStartingStats();
	}

	protected abstract ShipEnergyManager SetupEnergyManager(int blueMax, int greenMax);
	protected abstract ShipHealthManager SetupHealthManager(int healthMax, int shieldsMax);
	protected abstract EquipmentUser SetupEquipmentUser();

	protected void ResetToStartingStats()
	{
		healthManager.ResetToStartingHealth();
		ResetToStartingStatsKeepHealth();
	}

	protected void ResetToStartingStatsKeepHealth()
	{
		shipEquipment.ResetAllCooldowns();
		healthManager.ResetToStartingShields();
		energyManager.ResetToStartingStats();

		generatorLevel = 1;
		if (EGeneratorLevelChanged != null) EGeneratorLevelChanged();

		
	}

	public virtual void DisposeModel()
	{
		EGeneratorLevelChanged = null;

		healthManager.EHealthDepleted -= DoDeathEvent;
		statusEffectManager.Dispose();
		healthManager.Dispose();
		energyManager.Dispose();
		equipmentUser.Dispose();
		shipEquipment.DisposeModel();
		BattleManager.EEngagementModeStarted -= TryRegenShields;
	}

	protected virtual void TryRegenShields()
	{
		healthManager.RegenShields();
	}

	protected abstract void DoDeathEvent();

	public void ChangeGeneratorLevel(int delta)
	{
		generatorLevel += delta;
		if (EGeneratorLevelChanged != null) EGeneratorLevelChanged();
	}
}
