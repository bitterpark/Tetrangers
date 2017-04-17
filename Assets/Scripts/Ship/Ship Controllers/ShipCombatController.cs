using System;
using System.Collections.Generic;
using UnityEngine;
using StatusEffects;

public abstract	class ShipCombatController: ShipController
{

	public ShipCombatController(ShipModel model, IShipViewProvider viewProvider):base(model, viewProvider)
	{
		//ShipEquipment.EEquipmentCooldownChanged += UpdateCooldownTime;
		model.statusEffectManager.EStatusEffectGained += HandleStatusEffectAdding;
		model.healthManager.EHealthDamaged += DisplayHealthDamage;
		model.healthManager.EShieldsDamaged += DisplayShieldsDamage;
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		//ShipEquipment.EEquipmentCooldownChanged -= UpdateCooldownTime;
		model.statusEffectManager.EStatusEffectGained -= HandleStatusEffectAdding;
		model.healthManager.EHealthDamaged -= DisplayHealthDamage;
		model.healthManager.EShieldsDamaged -= DisplayShieldsDamage;
	}

	void HandleStatusEffectAdding(IDisplayableStatusEffect effect)
	{
		view.statusEffectDisplayer.ShowStatusEffect(effect);
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

