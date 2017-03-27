using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ShipModel: IEquipmentListModel
{
	public event UnityAction EHealthChanged;
	public event UnityAction EHealthDamaged;
	public event UnityAction EEnergyChanged;
	public event UnityAction<StatusEffect> EStatusEffectGained;

	public delegate int DefensesDeleg(int damage);
	public event DefensesDeleg EActivateDefences;
	//public event UnityAction<StatusEffect> EStatusEffectRemoved;

	public string shipName { get; private set;}

	public int shipHealth {get; protected set;}
	public int shipHealthMax { get; protected set; }

	public int shipBlueEnergy { get; protected set; }
	public int shipBlueEnergyMax { get; protected set; }
	public int shipGreenEnergy { get; protected set; }
	public int shipGreenEnergyMax { get; protected set; }

	public int blueEnergyGain;
	public int greenEnergyGain;

	public Sprite shipSprite { get; private set; }

	public List<ShipWeapon> shipWeapons 
	{
		get { return _shipWeapons;} 
	}
	List<ShipWeapon> _shipWeapons = new List<ShipWeapon>();
	public List<ShipEquipment> shipOtherEquipment
	{
		get { return _otherEquipment; }
	}
	List<ShipEquipment> _otherEquipment = new List<ShipEquipment>();

	List<StatusEffect> activeStatusEffects = new List<StatusEffect>();

	public ShipModel()//int shipHealthMax, int shipBlueEnergyMax, int shipGreenEnergyMax, Sprite shipSprite, string shipName)
	{
		InitializeClassStats();
	}

	protected virtual void InitializeEventSubscriptions()
	{
		BattleManager.EEngagementModeEnded += ReduceCooldownTimes;
	}

	protected abstract void InitializeClassStats();

	protected void SetStartingStats(int shipHealthMax, int shipBlueEnergyMax, int shipGreenEnergyMax, Sprite shipSprite, string shipName)
	{
		this.shipHealthMax = shipHealthMax;
		this.shipGreenEnergyMax = shipGreenEnergyMax;
		this.shipBlueEnergyMax = shipBlueEnergyMax;

		this.shipSprite = shipSprite;
		this.shipName = shipName;

		ResetToStartingStats();
	}

	protected void ResetToStartingStats()
	{
		shipHealth = shipHealthMax;
		if (EHealthChanged != null) EHealthChanged();
		ResetToStartingStatsKeepHealth();
	}

	protected void ResetToStartingStatsKeepHealth()
	{
		ResetAllCooldowns();
		shipBlueEnergy = 0;
		shipGreenEnergy = 0;
		if (EEnergyChanged != null)
			EEnergyChanged();
	}

	void ResetAllCooldowns()
	{
		List<ShipEquipment> allEquipment = new List<ShipEquipment>();
		allEquipment.AddRange(shipWeapons.ToArray());
		allEquipment.AddRange(shipOtherEquipment.ToArray());

		foreach (ShipEquipment equipment in allEquipment)
			equipment.ResetCooldown();
	}

	public void AddEquipment(params ShipEquipment[] equipment)
	{
		foreach (ShipEquipment equipmentUnit in equipment)
			if (equipmentUnit.equipmentType == EquipmentTypes.Weapon)
				AddWeapons(equipmentUnit);
			else
				AddOtherEquipment(equipmentUnit);
	}

	protected void AddWeapons(params ShipEquipment[] addedWeapons)
	{
		foreach (ShipEquipment equipmentUnit in addedWeapons)
		{
			Debug.Assert(equipmentUnit.equipmentType==EquipmentTypes.Weapon,"Trying to add non-weapons to ship weapons list!");
			AddWeapons(equipmentUnit as ShipWeapon);
		}
	}
	protected void AddWeapons(params ShipWeapon[] addedWeapons)
	{
		_shipWeapons.AddRange(addedWeapons);
	}

	protected void AddOtherEquipment(params ShipEquipment[] addedEquipment)
	{
		_otherEquipment.AddRange(addedEquipment);
	}

	public void RemoveEquipment(params ShipEquipment[] removedEquipment)
	{
		foreach (ShipEquipment equipmentUnit in removedEquipment)
			if (equipmentUnit.equipmentType == EquipmentTypes.Weapon)
				_shipWeapons.Remove(equipmentUnit as ShipWeapon);
			else
				_otherEquipment.Remove(equipmentUnit);
	}

	void AddNewStatusEffect(StatusEffect effect)
	{
		activeStatusEffects.Add(effect);
		effect.ActivateEffect(this);
		effect.EStatusEffectEnded += HandleStatusEffectDeactivation;
		if (EStatusEffectGained != null) EStatusEffectGained(effect);
	}

	void HandleStatusEffectDeactivation(StatusEffect effect)
	{
		Debug.Assert(activeStatusEffects.Contains(effect), "Trying to remove status effect from a ship that doesn't have it!");
		activeStatusEffects.Remove(effect);
		effect.EStatusEffectEnded -= HandleStatusEffectDeactivation;
		//if (EStatusEffectRemoved != null) EStatusEffectRemoved(effect);
	}

	public virtual void DisposeModel()
	{
		EHealthChanged = null;
		EHealthDamaged = null;
		EEnergyChanged = null;
		EActivateDefences = null;
		BattleManager.EEngagementModeEnded -= ReduceCooldownTimes;
	}

	public bool TryGetAllUsableEquipment(out List<ShipEquipment> allEquipment)
	{
		allEquipment = new List<ShipEquipment>();
		List<ShipWeapon> weapons;
		if (TryGetFireableWeapons(out weapons))
			allEquipment.AddRange(weapons.ToArray());
		List<ShipEquipment> equipment;
		if (TryGetUseableEquipment(out equipment))
			allEquipment.AddRange(equipment);

		if (allEquipment.Count > 0)
			return true;
		else
			return false;

	}

	public bool TryGetFireableWeapons(out List<ShipWeapon> weapons)
	{
		weapons = new List<ShipWeapon>();
		ShipWeapon weapon;
		for(int i=0; i<shipWeapons.Count; i++)
		{
			weapon = shipWeapons[i];
			if (EquipmentUsable(weapon))
				weapons.Add(weapon);
		}

		if (weapons.Count > 0)
			return true;
		else
			return false;
	}

	public bool TryGetUseableEquipment(out List<ShipEquipment> equipment)
	{
		equipment = new List<ShipEquipment>();

		ShipEquipment equipmentUnit;
		for (int i = 0; i < shipOtherEquipment.Count; i++)
		{
			equipmentUnit = shipOtherEquipment[i];
			if (EquipmentUsable(equipmentUnit))
					equipment.Add(equipmentUnit);
		}
		if (equipment.Count > 0)
			return true;
		else
			return false;
	}

	bool EquipmentUsable(ShipEquipment equipment)
	{
		if (equipment.blueEnergyCostToUse <= shipBlueEnergy && equipment.greenEnergyCostToUse <= shipGreenEnergy 
			&& equipment.cooldownTimeRemaining == 0 && equipment.isUseable)
			return true;
		else
			return false;
	}
	/*
	public List<int> GetWeaponCooldownTimes()
	{
		List<int> weaponCooldownTimes = new List<int>();
		foreach (ShipWeapon weapon in shipWeapons)
			weaponCooldownTimes.Add(weapon.cooldownTimeRemaining);

		return weaponCooldownTimes;
	}*/

	public void FireWeapon(ShipWeapon weapon)
	{
		Debug.Assert(shipWeapons.Contains(weapon), "Fired weapon : " + weapon.name + " not found in ship's weapons list!");
		//ShipWeapon weapon = shipWeapons[weaponIndex];
		ChangeBlueEnergy(-weapon.blueEnergyCostToUse);
		ChangeGreenEnergy(-weapon.greenEnergyCostToUse);
		int damage = weapon.FireWeapon(this);
		DoWeaponFireEvent(damage);
	}

	protected abstract void DoWeaponFireEvent(int weaponDamage);

	public void UseEquipment(ShipEquipment equipment)
	{
		Debug.Assert(shipOtherEquipment.Contains(equipment), "Activated equipment : " + equipment.name + " not found in ship's equipment list!");
		ChangeBlueEnergy(-equipment.blueEnergyCostToUse);
		ChangeGreenEnergy(-equipment.greenEnergyCostToUse);
		equipment.ActivateEquipment(this);
		if (equipment.GetType().BaseType == typeof(StatusEffectEquipment) || equipment.GetType().BaseType == typeof(Ability))
		{
			StatusEffectEquipment statusEquipment = equipment as StatusEffectEquipment;
			AddNewStatusEffect(statusEquipment.appliedStatusEffect);
		}
	}

	void ReduceCooldownTimes()
	{
		foreach (ShipWeapon weapon in shipWeapons)
			weapon.cooldownTimeRemaining--;
		foreach (ShipEquipment equipment in shipOtherEquipment)
			equipment.cooldownTimeRemaining--;
	}

	public void GainHealth(int healthGained)
	{
		ChangeHealth(healthGained);
	}

	protected void TakeDamage(int damage)
	{
		int totalDamage = damage;
		if (EActivateDefences != null)
			totalDamage = EActivateDefences(damage);
		ChangeHealth(-totalDamage);
	}

	void ChangeHealth(int delta)
	{
		shipHealth = Mathf.Clamp(shipHealth + delta, 0, shipHealthMax);
		if (delta > 0 && EHealthChanged != null) EHealthChanged();
		else
			if (delta < 0 && EHealthDamaged != null) EHealthDamaged();
		if (shipHealth == 0) DoDeathEvent();
	}

	protected abstract void DoDeathEvent();

	protected void GainBlueEnergy()
	{
		GainBlueEnergy(1,false);
	}
	protected void GainBlueEnergy(int gains)
	{
		GainBlueEnergy(gains, false);
	}
	public void GainBlueEnergy(int gains, bool absoluteValue)
	{
		if (gains > 0)
		{
			int delta;
			if (absoluteValue)
				delta = gains;
			else
				delta = gains * blueEnergyGain;
			ChangeBlueEnergy(delta);
		}
	}

	protected void GainGreenEnergy()
	{
		GainGreenEnergy(1,false);
	}
	protected void GainGreenEnergy(int gains)
	{
		GainGreenEnergy(gains, false);
	}
	public void GainGreenEnergy(int gains, bool absoluteValue)
	{
		if (gains > 0)
		{
			int delta;
			if (absoluteValue)
				delta = gains;
			else
				delta = gains * greenEnergyGain;
			ChangeGreenEnergy(delta);
		}
	}

	protected void ChangeBlueEnergy(int delta)
	{
		shipBlueEnergy = Mathf.Clamp(shipBlueEnergy+delta,0,shipBlueEnergyMax);
		if (EEnergyChanged != null)
			EEnergyChanged();
	}

	protected void ChangeGreenEnergy(int delta)
	{
		shipGreenEnergy = Mathf.Clamp(shipGreenEnergy + delta, 0, shipGreenEnergyMax);
		if (EEnergyChanged != null)
			EEnergyChanged();
	}

	public List<ShipEquipment> GetStoredEquipment()
	{
		List<ShipEquipment> equipment = new List<ShipEquipment>();
		equipment.AddRange(shipWeapons.ToArray());
		equipment.AddRange(shipOtherEquipment);

		return equipment;
	}
}
