using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ShipEquipmentModel : IEquipmentListModel
{
	public List<ShipWeapon> shipWeapons
	{
		get { return _shipWeapons; }
	}
	List<ShipWeapon> _shipWeapons = new List<ShipWeapon>();
	public List<ShipEquipment> shipOtherEquipment
	{
		get { return _otherEquipment; }
	}
	List<ShipEquipment> _otherEquipment = new List<ShipEquipment>();

	ICanUseEquipment equipmentOwner;
	ShipModel parentShip;

	public ShipEquipmentModel(ShipModel parentShip, ICanUseEquipment equipmentUser)
	{
		this.equipmentOwner = equipmentUser;
		this.parentShip = parentShip;
	}


	public void InitializeForBattle()
	{
		BattleManager.EEngagementModeEnded += DoRoundoverGains;
	}

	public void LowerAllCooldowns(int lowerBy)
	{
		foreach (ShipEquipment equipment in GetStoredEquipment())
			equipment.cooldownTimeRemaining -= lowerBy;
	}

	public void ResetAllCooldowns()
	{
		List<ShipEquipment> allEquipment = GetStoredEquipment();//new List<ShipEquipment>();

		foreach (ShipEquipment equipment in allEquipment)
			equipment.ResetEquipment();
	}

	public void AddEquipment(params ShipEquipment[] equipment)
	{
		foreach (ShipEquipment equipmentUnit in equipment)
		{
			if (equipmentUnit.equipmentType == EquipmentTypes.Weapon)
				AddWeapons(equipmentUnit);
			else
				AddOtherEquipment(equipmentUnit);
			equipmentUnit.SetOwner(equipmentOwner);
		}
	}

	protected void AddWeapons(params ShipEquipment[] addedWeapons)
	{
		foreach (ShipEquipment equipmentUnit in addedWeapons)
		{
			Debug.Assert(equipmentUnit.equipmentType == EquipmentTypes.Weapon, "Trying to add non-weapons to ship weapons list!");
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

	public void DisposeModel()
	{
		foreach (ShipWeapon weapon in shipWeapons)
			weapon.Dispose();
		foreach (ShipEquipment equipment in shipOtherEquipment)
			equipment.Dispose();

		BattleManager.EEngagementModeEnded -= DoRoundoverGains;
	}

	public bool TryGetAllUsableEquipment(out List<ShipEquipment> allEquipment)
	{
		return TryGetAllUsableEquipment(out allEquipment, true);
	}

	public bool TryGetAllUsableEquipment(out List<ShipEquipment> allEquipment, bool checkEnergy)
	{
		allEquipment = GetAllUsableEquipment(checkEnergy);

		if (allEquipment.Count > 0)
			return true;
		else
			return false;

	}

	public List<ShipEquipment> GetAllUsableEquipment(bool checkEnergy)
	{
		List<ShipEquipment> allEquipment = new List<ShipEquipment>();
		List<ShipWeapon> weapons;
		if (TryGetActivatableWeapons(out weapons, checkEnergy))
			allEquipment.AddRange(weapons.ToArray());
		List<ShipEquipment> equipment;
		if (TryGetUseableEquipment(out equipment, checkEnergy))
			allEquipment.AddRange(equipment);

		return allEquipment;
	}

	public bool TryGetActivatableWeapons(out List<ShipWeapon> weapons, bool checkEnergy)
	{
		weapons = new List<ShipWeapon>();
		ShipWeapon weapon;
		for (int i = 0; i < shipWeapons.Count; i++)
		{
			weapon = shipWeapons[i];
			if (WeaponUsable(weapon, checkEnergy))
				weapons.Add(weapon);
		}

		if (weapons.Count > 0)
			return true;
		else
			return false;
	}

	public virtual bool TryGetUseableEquipment(out List<ShipEquipment> equipment, bool checkEnergy)
	{
		equipment = new List<ShipEquipment>();

		ShipEquipment equipmentUnit;
		for (int i = 0; i < shipOtherEquipment.Count; i++)
		{
			equipmentUnit = shipOtherEquipment[i];
			if (EquipmentUsable(equipmentUnit, checkEnergy))
				equipment.Add(equipmentUnit);
		}
		if (equipment.Count > 0)
			return true;
		else
			return false;
	}

	bool WeaponUsable(ShipWeapon weapon, bool checkEnergy)
	{
		if (weapon.lockedOn)
			return EquipmentUsable(weapon, checkEnergy);
		else
			return (weapon.IsUsableByShip(parentShip));
	}

	protected bool EquipmentUsable(ShipEquipment equipment, bool checkEnergy)
	{
		return ((equipmentOwner.equipmentUser.EnoughEnergyToUseEquipment(equipment) || !checkEnergy) 
			&& equipment.IsUsableByShip(parentShip));
	}


	public List<ShipEquipment> GetStoredEquipment()
	{
		List<ShipEquipment> equipment = new List<ShipEquipment>();
		equipment.AddRange(shipWeapons.ToArray());
		equipment.AddRange(shipOtherEquipment.ToArray());

		return equipment;
	}


	public void UseEquipment(ShipEquipment equipment)
	{
		equipmentOwner.equipmentUser.UseEquipment(equipment);
	}

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

}

