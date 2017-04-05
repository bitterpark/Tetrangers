using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ShipModel: IEquipmentListModel
{
	public event UnityAction EHealthChanged;
	public event UnityAction EHealthDamaged;
	public event UnityAction EShieldsChanged;
	public event UnityAction EShieldsGainChanged;
	public event UnityAction EShieldsDamaged;
	public event UnityAction EEnergyChanged;
	public event UnityAction EEnergyGainChanged;
	public event UnityAction<StatusEffect> EStatusEffectGained;

	//public event UnityAction<EquipmentTypes> EEquipmentTypeUsed;

	public delegate int DefensesDeleg(int damage);
	public event DefensesDeleg EActivateDefences;
	//public event UnityAction<StatusEffect> EStatusEffectRemoved;

	public string shipName { get; private set;}

	public int shipHealth {get; protected set;}
	public int shipHealthMax { get; protected set; }
	public int shipShields { get; protected set; }
	public int shipShieldsMax { get; protected set; }
	protected int shipShieldsGain;
	public int shipShieldsCurrentGain { get; protected set; }

	public int shipBlueEnergy { get; protected set; }
	public int shipBlueEnergyMax { get; protected set; }
	public int shipGreenEnergy { get; protected set; }
	public int shipGreenEnergyMax { get; protected set; }

	public int blueEnergyGain { get; protected set; }
	public int greenEnergyGain { get; protected set; }

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
		BattleManager.EEngagementModeEnded += DoRoundoverGains;
		BattleManager.EEngagementModeStarted += TryRegenShields;
	}

	protected abstract void InitializeClassStats();

	protected void SetStartingStats(int shipHealthMax, int shipShieldsMax, int shieldsGain, int shipBlueEnergyMax, int shipGreenEnergyMax, Sprite shipSprite, string shipName)
	{
		this.shipHealthMax = shipHealthMax;

		this.shipShieldsMax = shipShieldsMax;

		this.shipShieldsGain = shieldsGain;

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

		this.shipShields = shipShieldsMax;
		this.shipShieldsCurrentGain = shipShieldsGain;

		if (EShieldsChanged != null) EShieldsChanged();
		if (EShieldsGainChanged != null) EShieldsGainChanged();

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
			equipment.ResetEquipment();
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

	protected void AddNewStatusEffect(StatusEffect effect)
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
		EShieldsChanged = null;
		EShieldsDamaged = null;
		EEnergyChanged = null;
		EActivateDefences = null;
		activeStatusEffects.Clear();
		BattleManager.EEngagementModeEnded -= DoRoundoverGains;
		BattleManager.EEngagementModeStarted -= TryRegenShields;
	}

	public bool TryGetAllUsableEquipment(out List<ShipEquipment> allEquipment)
	{
		allEquipment = new List<ShipEquipment>();
		List<ShipWeapon> weapons;
		if (TryGetActivatableWeapons(out weapons))
			allEquipment.AddRange(weapons.ToArray());
		List<ShipEquipment> equipment;
		if (TryGetUseableEquipment(out equipment))
			allEquipment.AddRange(equipment);

		if (allEquipment.Count > 0)
			return true;
		else
			return false;

	}

	public bool TryGetActivatableWeapons(out List<ShipWeapon> weapons)
	{
		weapons = new List<ShipWeapon>();
		ShipWeapon weapon;
		for(int i=0; i<shipWeapons.Count; i++)
		{
			weapon = shipWeapons[i];
			if (WeaponUsable(weapon))
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

	bool WeaponUsable(ShipWeapon weapon)
	{
		if (weapon.lockedOn)
			return EquipmentUsable(weapon);
		else
			return (weapon.IsUsableByShip(this));
	}

	bool EquipmentUsable(ShipEquipment equipment)
	{
		if (equipment.blueEnergyCostToUse <= shipBlueEnergy && equipment.greenEnergyCostToUse <= shipGreenEnergy 
			&& equipment.IsUsableByShip(this))
			return true;
		else
			return false;
	}
	
	

	/*
	public void FireWeapon(ShipWeapon weapon)
	{
		Debug.Assert(shipWeapons.Contains(weapon), "Fired weapon : " + weapon.name + " not found in ship's weapons list!");
		//ShipWeapon weapon = shipWeapons[weaponIndex];
		
		int damage = weapon.ActivateWeapon(this);
		DoWeaponFireEvent(damage);
	}*/

	protected abstract void DoWeaponFireEvent(int weaponDamage);

	public void UseEquipment(ShipEquipment equipment)
	{
		//Debug.Assert(shipOtherEquipment.Contains(equipment) || shipWeapons.Contains(equipment), "Activated equipment : " + equipment.name + " not found in ship's equipment list!");
		//if (EEquipmentTypeUsed != null) EEquipmentTypeUsed(equipment.equipmentType);

		
		

		if (equipment.onSelfEffect != null)
			AddNewStatusEffect(equipment.onSelfEffect);
		if (equipment.onOpponentEffect != null)
			ApplyStatusEffectToOpponent(equipment.onOpponentEffect);

		if (equipment.equipmentType == EquipmentTypes.Weapon)
		{
			ShipWeapon weapon = equipment as ShipWeapon;
			Debug.Assert(shipWeapons.Contains(weapon), "Fired weapon : " + weapon.name + " not found in ship's weapons list!");
			int damage = weapon.ActivateWeapon(this);
			ChangeBlueEnergy(-equipment.blueEnergyCostToUse);
			ChangeGreenEnergy(-equipment.greenEnergyCostToUse);
			DoWeaponFireEvent(damage);
			/*
			if (weapon.TryActivateWeapon(this, out damage))
			{
				ChangeBlueEnergy(-equipment.blueEnergyCostToUse);
				ChangeGreenEnergy(-equipment.greenEnergyCostToUse);
				DoWeaponFireEvent(damage);
			}*/
		}
		else
		{
			ChangeBlueEnergy(-equipment.blueEnergyCostToUse);
			ChangeGreenEnergy(-equipment.greenEnergyCostToUse);
			equipment.ActivateEquipment(this);
		}

	}
	protected abstract void ApplyStatusEffectToOpponent(StatusEffect effect);

	void DoRoundoverGains()
	{
		ReduceCooldownTimes();
	}

	void ReduceCooldownTimes()
	{
		foreach (ShipWeapon weapon in shipWeapons)
			weapon.cooldownTimeRemaining--;
			//weapon.TryChangeLockonTime(-1);
		foreach (ShipEquipment equipment in shipOtherEquipment)
			equipment.cooldownTimeRemaining--;
	}

	protected virtual void TryRegenShields()
	{
		//if (shipShieldsCurrentGain>0)
			//ChangeShields(shipShieldsCurrentGain);
		SetShieldsGain(shipShieldsGain);
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

		if (totalDamage > 0)
		{
			if (shipShields>0)
				totalDamage = TakeShieldDamage(totalDamage);
			SetShieldsGain(0);
		}
		if (totalDamage>0)
			ChangeHealth(-totalDamage);
	}

	int TakeShieldDamage(int damage)
	{
		int spilloverHealthDamage;

		if (damage<=shipShields)
		{
			ChangeShields(-damage);
			spilloverHealthDamage = 0;
		}
		else
		{
			spilloverHealthDamage = damage - shipShields;
			ChangeShields(-shipShields);
		}
		if (EShieldsDamaged != null) EShieldsDamaged();
		return spilloverHealthDamage;
	}

	public void GainShields()
	{
		GainShields(shipShieldsCurrentGain);
	}

	public void GainShields(int gain)
	{
		ChangeShields(gain);
	}

	public void ChangeShields(int delta)
	{
		shipShields = Mathf.Clamp(shipShields + delta, 0, shipShieldsMax);
		if (delta > 0 && EShieldsChanged != null) EShieldsChanged();
		else
			if (delta < 0 && EShieldsDamaged != null)
				EShieldsDamaged();
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

	public void ChangeShieldsGain(int delta)
	{
		SetShieldsGain(shipShieldsCurrentGain + delta);
	}

	public void SetShieldsGain(int newGain)
	{
		if (newGain != shipShieldsCurrentGain)
		{
			shipShieldsCurrentGain = Mathf.Max(newGain, 0);
			if (EShieldsGainChanged != null) EShieldsGainChanged();
		}
	}

	public void ChangeBlueEnergyGain(int blueDelta)
	{
		ChangeEnergyGain(blueDelta, 0);
	}

	public void ChangeGreenEnergyGain(int greenDelta)
	{
		ChangeEnergyGain(0, greenDelta);
	}

	public void ChangeEnergyGain(int blueDelta, int greenDelta)
	{
		blueEnergyGain =  blueEnergyGain + blueDelta;
		greenEnergyGain = greenEnergyGain + greenDelta;
		if (EEnergyGainChanged != null) EEnergyGainChanged();
	}

	protected void GainBlueEnergy()
	{
		GainBlueEnergy(1,false);
	}
	protected void GainBlueEnergy(int gains)
	{
		GainBlueEnergy(gains, false);
	}
	public virtual void GainBlueEnergy(int gains, bool absoluteValue)
	{
		TryGainBlueEnergy(gains,absoluteValue);
	}

	protected int TryGainBlueEnergy(int gains, bool absoluteValue)
	{
		if (gains > 0)
		{
			int delta;
			if (absoluteValue)
				delta = gains;
			else
				delta = gains * blueEnergyGain;
			ChangeBlueEnergy(delta);
			return delta;
		}
		return gains;
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
