using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ICanUseEquipment
{
	EquipmentUser equipmentUser { get; }
}

public abstract class EquipmentUser
{
	protected StatusEffectManager statusEffectManager;
	protected ShipEnergyManager energyManager;
	protected ShipHealthManager healthManager;
	ShipModel parentShip;

	public EquipmentUser(ShipModel parentShip, IHasEnergy energyUser)
	{
		this.parentShip = parentShip;
		statusEffectManager = parentShip.statusEffectManager;
		energyManager = energyUser.energyManager;
		healthManager = parentShip.healthManager;
	}

	public abstract void InitializeForBattle();
	public abstract void Dispose();

	public void UseEquipment(ShipEquipment equipment)
	{
		if (equipment.onSelfEffect != null)
			statusEffectManager.AddNewStatusEffect(equipment.onSelfEffect);
		if (equipment.onOpponentEffect != null)
			ApplyStatusEffectToOpponent(equipment.onOpponentEffect);

		if (equipment.equipmentType == EquipmentTypes.Weapon)
		{
			ShipWeapon weapon = equipment as ShipWeapon;
			//Debug.Assert(shipWeapons.Contains(weapon), "Fired weapon : " + weapon.name + " not found in ship's weapons list!");
			int damage = weapon.ActivateWeapon(parentShip);

			energyManager.blueEnergy -= equipment.blueEnergyCostToUse;
			energyManager.greenEnergy -= equipment.greenEnergyCostToUse;

			DoWeaponFireEvent(damage);
		}
		else
		{
			energyManager.blueEnergy -= equipment.blueEnergyCostToUse;
			energyManager.greenEnergy -= equipment.greenEnergyCostToUse;
			equipment.ActivateEquipment(parentShip);
		}

	}
	protected abstract void ApplyStatusEffectToOpponent(StatusEffect effect);
	protected abstract void DoWeaponFireEvent(int weaponDamage);

	public bool EnoughEnergyToUseEquipment(ShipEquipment equipment)
	{
		return energyManager.blueEnergy >= equipment.blueEnergyCostToUse && energyManager.greenEnergy >= equipment.greenEnergyCostToUse;
	}
}

