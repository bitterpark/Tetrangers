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
		model.EHealthDamaged += DisplayDamage;
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		//ShipEquipment.EEquipmentCooldownChanged -= UpdateCooldownTime;
		model.EStatusEffectGained -= HandleStatusEffectAdding;
		model.EHealthDamaged -= DisplayDamage;
	}
	/*
	protected override void UnsubscribeFromEquipmentView(ShipEquipmentView view)
	{
		base.UnsubscribeFromEquipmentView(view);
		view.EEquipmentButtonPressed -= HandleEquipmentButtonPress;
		view.EEquipmentButtonAnimationFinished -= HandleEquipmentButtonAnimationFinish;
	}
	
	protected override void SetupEquipmentView(ShipEquipmentView newView, ShipEquipment equipment)
	{
		base.SetupEquipmentView(newView, equipment);
		newView.EEquipmentButtonPressed += HandleEquipmentButtonPress;
		newView.EEquipmentButtonAnimationFinished += HandleEquipmentButtonAnimationFinish;
	}

	protected virtual void HandleEquipmentButtonPress(ShipEquipmentView buttonView)
	{
		ShipEquipment equipment = GetEquipmentRepresentedByView(buttonView);

		if (equipment != null)
			HandleEquipmentActivating(equipment);
	}
	void HandleEquipmentActivating(ShipEquipment equipment)
	{
		ShowEquipmentTypeViews(equipment.equipmentType);

		if (equipment.GetType().BaseType == typeof(ShipWeapon))
			model.FireWeapon((ShipWeapon)equipment);
		else
			model.UseEquipment(equipment);
	}
	protected abstract void HandleEquipmentButtonAnimationFinish();
	*/
	void HandleStatusEffectAdding(IDisplayableStatusEffect effect)
	{
		view.ShowStatusEffect(effect);
	}
	/*
	void UpdateCooldownTime(ShipEquipment equipment, int currentCooldownTime)
	{
		List<ShipEquipmentView> allViews = new List<ShipEquipmentView>();
		allViews.AddRange(equipmentViewPairings.Keys);

		foreach (ShipEquipmentView equipmentView in allViews)
		{
			if (equipmentViewPairings.ContainsKey(equipmentView) && equipmentViewPairings[equipmentView] == equipment)
			{
				equipmentView.SetCooldownTime(currentCooldownTime, equipment.maxCooldownTime);
				return;
			}
		}
	}*/

	void DisplayDamage()
	{
		UpdateHealth();
		view.PlayGotHitFX();
	}

}

