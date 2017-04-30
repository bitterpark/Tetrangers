using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public abstract class ShipSectorController
{
	
	SectorEnergyController energyController;
	StatusEffectController effectsController;
	ShipEquipmentController equipmentController;
	HealthController healthController;
	protected ShipSectorModel model;
	protected SectorView view;


	public ShipSectorController(ShipSectorModel model, SectorView view, ShipEquipmentController equipmentController)
	{
		this.model = model;
		this.view = view;

		this.equipmentController = equipmentController;
		energyController = new SectorEnergyController(view.energyView, model.energyManager);
		effectsController = new StatusEffectController(view.statusEffectDisplayer,model.effectsManager);

		healthController = new HealthController(view.healthView, model.healthManager);
	}

	//protected abstract ShipEquipmentController CreateOrGetAppropriateEquipmentController();

	public virtual void Dispose()
	{
		healthController.Dispose();
		energyController.DisposeController();
		effectsController.Dispose();
		equipmentController.DisposeController(false);
	}

	protected void UpdateHealth()
	{
		view.healthView.SetHealth(model.healthManager.health, model.healthManager.healthMax);
	}

	protected void UpdateShields()
	{
		view.healthView.SetShields(model.healthManager.shields, model.healthManager.shieldsMax);
	}

	void UpdateShieldsGain()
	{
		view.healthView.SetShieldsGain(model.healthManager.shieldsCurrentGain);
	}

	void DisplayShieldsDamage()
	{
		UpdateShields();
		view.healthView.PlayGotHitFX();
	}

	void DisplayHealthDamage()
	{
		UpdateHealth();
		view.healthView.PlayGotHitFX();
	}
}

