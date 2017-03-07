using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipModel
{
	public delegate void EmptyDeleg();
	public event EmptyDeleg ETookDamage;
	public event EmptyDeleg EEnergyChanged;
	public event EmptyDeleg ECooldownsChanged;

	public string shipName { get; private set;}

	public int shipHealth {get; private set;}
	public int shipHealthMax { get; private set; }

	public int shipEnergy { get; private set; }
	public int shipEnergyMax { get; private set; }

	public Sprite shipSprite { get; private set; }

	public List<ShipWeapon> shipWeapons 
	{
		get { return _shipWeapons;} 
	}
	List<ShipWeapon> _shipWeapons = new List<ShipWeapon>();

	

	public ShipModel(int shipHealthMax, int shipEnergy, int shipEnergyMax, Sprite shipSprite, string shipName)
	{
		this.shipHealthMax = shipHealthMax;
		shipHealth = shipHealthMax;

		this.shipEnergy = shipEnergy;
		this.shipEnergyMax = shipEnergyMax;

		this.shipSprite = shipSprite;
		this.shipName = shipName;

		AddWeapons(new LaserGun(), new PlasmaCannon());
		BattleManager.EEngagementModeEnded += ReduceCooldownTimes;
	}

	public void AddWeapons(params ShipWeapon[] addedWeapons)
	{
		_shipWeapons.AddRange(addedWeapons);
	}

	public virtual void DisposeModel()
	{
		ETookDamage = null;
		EEnergyChanged = null;
		ECooldownsChanged = null;
		BattleManager.EEngagementModeEnded -= ReduceCooldownTimes;
	}


	public bool TryGetFireableWeapons(out List<int> weaponIndices)
	{
		weaponIndices = new List<int>();
		ShipWeapon weapon;
		for(int i=0; i<shipWeapons.Count; i++)
		{
			weapon = shipWeapons[i];
			if (weapon.energyCostToFire <= shipEnergy && weapon.cooldownTimeRemaining == 0)
				weaponIndices.Add(i);
		}

		if (weaponIndices.Count > 0)
			return true;
		else
			return false;
	}

	public List<int> GetWeaponCooldownTimes()
	{
		List<int> weaponCooldownTimes = new List<int>();
		foreach (ShipWeapon weapon in shipWeapons)
			weaponCooldownTimes.Add(weapon.cooldownTimeRemaining);

		return weaponCooldownTimes;
	}

	public void FireWeapon(int weaponIndex)
	{
		Debug.Assert(weaponIndex >= 0 && weaponIndex < _shipWeapons.Count, "Fired weapon index: "+weaponIndex+" not found in ship's weapons list!");
		ShipWeapon weapon = shipWeapons[weaponIndex];
		Debug.Assert(shipEnergy >= weapon.energyCostToFire,"Cannot fire weapon: not enough energy!");
		ChangeEnergy(-weapon.energyCostToFire);
		weapon.SetCooldown();
		DoWeaponFireEvent(weapon.damage);
		if (ECooldownsChanged != null) ECooldownsChanged();
	}

	protected abstract void DoWeaponFireEvent(int weaponDamage);

	void ReduceCooldownTimes()
	{
		foreach (ShipWeapon weapon in shipWeapons)
			weapon.cooldownTimeRemaining--;
		if (ECooldownsChanged != null) ECooldownsChanged();
	}

	protected void TakeDamage(int damage)
	{
		shipHealth -= damage;
		shipHealth = Mathf.Max(0, shipHealth);
		if (ETookDamage != null) ETookDamage();
		if (shipHealth == 0) DoDeathEvent();
	}
	protected abstract void DoDeathEvent();

	protected void ChangeEnergy(int delta)
	{
		shipEnergy = Mathf.Clamp(shipEnergy+delta,0,shipEnergyMax);
		if (EEnergyChanged != null)
			EEnergyChanged();
	}
	
}
