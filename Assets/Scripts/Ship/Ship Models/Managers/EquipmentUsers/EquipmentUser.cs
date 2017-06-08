using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ICanUseEquipment
{
	EquipmentUser equipmentUser { get; }
}

public abstract class EquipmentUser
{
	public delegate void StatusEffectToShipSectorDeleg (StatusEffect appliedEffect);
	public static event StatusEffectToShipSectorDeleg EAppliedStatusEffectToPlayerShipSector;

	protected StatusEffectManager statusEffectManager;
	protected ICanSpendEnergy energyUser;
	//protected ShipHealthManager healthManager;
	ShipModel parentShip;

	public EquipmentUser(ShipModel parentShip, IHasEnergy energyUserOwner)
	{
		this.parentShip = parentShip;
		statusEffectManager = parentShip.statusEffectManager;
		this.energyUser = energyUserOwner.energyUser;
		//healthManager = parentShip.healthManager;
	}

	public abstract void InitializeForBattle();
	public abstract void Dispose();

	public void UseEquipment(ShipEquipment equipment)
	{
		if (equipment.onSelfEffect != null)
			statusEffectManager.AddNewStatusEffect(equipment.onSelfEffect);
		if (equipment.onSectorEffect != null)
			ApplyStatusEffectToPlayerShipSector(equipment.onSectorEffect);
		if (equipment.onOpponentEffect != null)
			ApplyStatusEffectToOpponent(equipment.onOpponentEffect);

		if (equipment.equipmentType == EquipmentTypes.Weapon)
		{
			ShipWeapon weapon = equipment as ShipWeapon;
			//Debug.Assert(shipWeapons.Contains(weapon), "Fired weapon : " + weapon.name + " not found in ship's weapons list!");
			AttackInfo attack = weapon.ActivateWeapon();

			energyUser.SpendEnergyFromEquipmentUse(equipment);
			DoWeaponFireEvent(attack);
		}
		else
		{
			energyUser.SpendEnergyFromEquipmentUse(equipment);
			equipment.ActivateEquipment();
		}

	}

	void ApplyStatusEffectToPlayerShipSector(StatusEffect effect)
	{
		EAppliedStatusEffectToPlayerShipSector(effect);
	}

	protected abstract void ApplyStatusEffectToOpponent(StatusEffect effect);
	protected abstract void DoWeaponFireEvent(AttackInfo attack);

	public bool EnoughEnergyToUseEquipment(ShipEquipment equipment)
	{
		return energyUser.EnoughEnergyToUseEquipment(equipment);
	}
}

