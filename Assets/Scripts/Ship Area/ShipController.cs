using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatusEffects;

public abstract class ShipController {

	protected ShipView view;
	protected ShipModel model;

	protected EquipmentListController equipmentController;

	public ShipController(ShipModel model, ShipView view)//:base(model,view)
	{
		this.view = view;
		this.model = model;

		equipmentController = CreateEquipmentController(model, view);

		model.EHealthChanged += UpdateHealth;
		model.EEnergyChanged += UpdateEnergy;

		view.SetNameAndSprite(model.shipName,model.shipSprite);

		UpdateHealth();
		UpdateEnergy();
	}

	protected abstract EquipmentListController CreateEquipmentController(ShipModel model, ShipView view);

	public virtual void DisposeController(bool disposeModel)
	{
		equipmentController.DisposeController(disposeModel);
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
