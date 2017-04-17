using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StatusEffects;

public abstract class ShipController {

	protected ShipView view;
	protected ShipModel model;

	protected EquipmentListController equipmentController;
	ShipEnergyController energyController;

	public ShipController(ShipModel model, IShipViewProvider viewProvider)//:base(model,view)
	{
		this.view = viewProvider.shipView;
		this.model = model;

		equipmentController = CreateEquipmentController(model.shipEquipment, viewProvider);

		model.healthManager.EHealthChanged += UpdateHealth;
		model.healthManager.EShieldsChanged += UpdateShields;
		model.healthManager.EShieldsGainChanged += UpdateShieldsGain;

		energyController = new ShipEnergyController(view.energyView, model.energyManager);

		model.EGeneratorLevelChanged += UpdateGenerator;

		view.SetNameAndSprite(model.shipName,model.shipSprite);

		UpdateHealth();
		UpdateShields();
		UpdateShieldsGain();
		UpdateGenerator();
	}

	protected abstract EquipmentListController CreateEquipmentController(ShipEquipmentModel model, IShipViewProvider viewProvider);

	public virtual void DisposeController(bool disposeModel)
	{
		equipmentController.DisposeController(disposeModel);
		energyController.DisposeController();
		model.healthManager.EHealthChanged -= UpdateHealth;
		model.healthManager.EShieldsChanged -= UpdateShields;
		model.healthManager.EShieldsGainChanged -= UpdateShieldsGain;

		model.EGeneratorLevelChanged -= UpdateGenerator;

		if (disposeModel)
			model.DisposeModel();
	}


	protected void UpdateHealth()
	{
		view.SetHealth(model.healthManager.health, model.healthManager.healthMax);
	}

	protected void UpdateShields()
	{
		view.SetShields(model.healthManager.shields, model.healthManager.shieldsMax);
	}

	void UpdateShieldsGain()
	{
		view.SetShieldsGain(model.healthManager.shieldsCurrentGain);
	}

	void UpdateGenerator()
	{
		view.SetGenLevel(model.generatorLevel);
	}
	
	

}
