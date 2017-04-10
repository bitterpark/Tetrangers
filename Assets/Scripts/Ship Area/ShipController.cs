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
		model.EShieldsChanged += UpdateShields;
		model.EShieldsGainChanged += UpdateShieldsGain;
		model.EEnergyChanged += UpdateEnergy;
		model.EEnergyGainChanged += UpdateEnergyGain;
		model.EGeneratorLevelChanged += UpdateGenerator;
		//model.EEquipmentTypeUsed += SwitchToRelevantEquipmentTab;

		view.SetNameAndSprite(model.shipName,model.shipSprite);

		UpdateHealth();
		UpdateShields();
		UpdateShieldsGain();
		UpdateGenerator();
		UpdateEnergy();
		UpdateEnergyGain();
	}

	protected abstract EquipmentListController CreateEquipmentController(ShipModel model, ShipView view);

	public virtual void DisposeController(bool disposeModel)
	{
		equipmentController.DisposeController(disposeModel);
		model.EHealthChanged -= UpdateHealth;
		model.EShieldsChanged -= UpdateShields;
		model.EShieldsGainChanged -= UpdateShieldsGain;
		model.EEnergyChanged -= UpdateEnergy;
		model.EEnergyGainChanged -= UpdateEnergyGain;
		model.EGeneratorLevelChanged -= UpdateGenerator;
		//model.EEquipmentTypeUsed -= SwitchToRelevantEquipmentTab;

		if (disposeModel)
			model.DisposeModel();
	}

	void SwitchToRelevantEquipmentTab(EquipmentTypes tabType)
	{
		equipmentController.ShowEquipmentTypeViews(tabType);
	}

	protected void UpdateHealth()
	{
		view.SetHealth(model.shipHealth,model.shipHealthMax);
	}

	protected void UpdateShields()
	{
		view.SetShields(model.shipShields, model.shipShieldsMax);
	}

	void UpdateShieldsGain()
	{
		view.SetShieldsGain(model.shipShieldsCurrentGain);
	}

	void UpdateGenerator()
	{
		view.SetGenLevel(model.generatorLevel);
	}

	void UpdateEnergy()
	{
		view.SetBlueEnergy(model.shipBlueEnergy,model.shipBlueEnergyMax);
		view.SetGreenEnergy(model.shipGreenEnergy, model.shipGreenEnergyMax);
	}

	void UpdateEnergyGain()
	{
		view.SetEnergyGainLevels(model.blueEnergyGain, model.greenEnergyGain);
	}
	
	

}
