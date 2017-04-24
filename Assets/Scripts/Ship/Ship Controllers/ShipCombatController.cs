using System;
using System.Collections.Generic;
using UnityEngine;
using StatusEffects;

public abstract	class ShipCombatController: ShipController
{

	StatusEffectController effectsController;

	public ShipCombatController(ShipModel model, IShipViewProvider viewProvider):base(model, viewProvider)
	{
		//ShipEquipment.EEquipmentCooldownChanged += UpdateCooldownTime;
		effectsController = new StatusEffectController(viewProvider.shipView.statusEffectDisplayer, model.statusEffectManager);
		model.healthManager.EHealthDamaged += DisplayHealthDamage;
		model.healthManager.EShieldsDamaged += DisplayShieldsDamage;
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		effectsController.Dispose();
		model.healthManager.EHealthDamaged -= DisplayHealthDamage;
		model.healthManager.EShieldsDamaged -= DisplayShieldsDamage;
	}

	void DisplayShieldsDamage()
	{
		UpdateShields();
		view.PlayGotHitFX();
	}

	void DisplayHealthDamage()
	{
		UpdateHealth();
		view.PlayGotHitFX();
	}

}

