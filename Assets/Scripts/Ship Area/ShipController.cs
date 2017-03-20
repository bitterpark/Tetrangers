using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatusEffects;

public class ShipController: EquipmentListController {

	protected new ShipView view;
	protected new ShipModel model;


	public ShipController(ShipModel model, ShipView view):base(model,view)
	{
		this.view = view;
		this.model = model;

		model.EHealthChanged += UpdateHealth;
		model.EEnergyChanged += UpdateEnergy;

		/*
		List<ShipWeapon> modelWeapons = model.shipWeapons;
		List<ShipEquipment> otherEquipment = model.shipOtherEquipment;
		

		
		ShipEquipmentView[] weaponViews = view.CreateEquipmentViews(modelWeapons.Count);
		for (int i = 0; i < modelWeapons.Count; i++)
		{
			ShipWeapon weapon = modelWeapons[i];	
			ShipEquipmentView weaponView = weaponViews[i];
			SetupEquipmentView(weaponView,weapon);
			weaponView.SetDamage(weapon.damage);
		}
		ShipEquipmentView[] otherEquipmentViews = view.CreateEquipmentViews(otherEquipment.Count);
		for (int j = 0; j < otherEquipment.Count; j++)
		{
			ShipEquipment equipment = otherEquipment[j];
			ShipEquipmentView equipmentView = otherEquipmentViews[j];
			SetupEquipmentView(equipmentView, equipment);
		}*/

		view.SetNameAndSprite(model.shipName,model.shipSprite);

		UpdateHealth();
		UpdateEnergy();
	}

	protected override void SetupEquipmentView(ShipEquipmentView newView, ShipEquipment equipment)
	{
		base.SetupEquipmentView(newView, equipment);
		if (equipment.equipmentType==EquipmentTypes.Weapon)
		{
			ShipWeapon weapon = equipment as ShipWeapon;
			newView.SetDamage(weapon.damage);
		}
	}


	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		model.EHealthChanged -= UpdateHealth;
		model.EEnergyChanged -= UpdateEnergy;

		if (disposeModel)
			model.DisposeModel();
	}

	

	protected void UpdateHealth()
	{
		view.SetHealth(model.shipHealth,model.shipHealthMax);
	}
	void UpdateEnergy()
	{
		view.SetBlueEnergy(model.shipBlueEnergy,model.shipBlueEnergyMax);
		view.SetGreenEnergy(model.shipGreenEnergy, model.shipGreenEnergyMax);
	}


	
	

}
