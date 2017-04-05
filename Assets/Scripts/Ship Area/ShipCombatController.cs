using System;
using System.Collections.Generic;
using UnityEngine;
using StatusEffects;

public abstract	class ShipCombatController: ShipController
{

	public ShipCombatController(ShipModel model, ShipView view):base(model,view)
	{
		//ShipEquipment.EEquipmentCooldownChanged += UpdateCooldownTime;
		model.EStatusEffectGained += HandleStatusEffectAdding;
		model.EHealthDamaged += DisplayHealthDamage;
		model.EShieldsDamaged += DisplayShieldsDamage;
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		//ShipEquipment.EEquipmentCooldownChanged -= UpdateCooldownTime;
		model.EStatusEffectGained -= HandleStatusEffectAdding;
		model.EHealthDamaged -= DisplayHealthDamage;
		model.EShieldsDamaged -= DisplayShieldsDamage;
	}

	void HandleStatusEffectAdding(IDisplayableStatusEffect effect)
	{
		view.ShowStatusEffect(effect);
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

