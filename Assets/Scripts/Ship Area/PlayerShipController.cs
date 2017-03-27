using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipController : ShipCombatController 
{
	public PlayerShipController(ShipModel model, ShipView view)
		: base(model, view)
	{
		//EnemyShipController.EEnemyTurnFinished += TryEnableEquipmentViewButtons;
		//BattleManager.EEngagementModeEnded += DisableEquipmentViewButtons;
	}

	protected override EquipmentListController CreateEquipmentController(ShipModel model, ShipView view)
	{
		return new PlayerShipEquipmentController(model, view);
	}

	/*
	void TryEnableEquipmentViewButtons()
	{
		List<ShipEquipment> usableEquipment;
		if (model.TryGetAllUsableEquipment(out usableEquipment))
		{
			foreach (ShipEquipmentView equipmentView in equipmentViewPairings.Keys)
				if (usableEquipment.Contains(equipmentViewPairings[equipmentView]))
					equipmentView.SetButtonInteractable(true);
				else
					equipmentView.SetButtonInteractable(false);
		}
	}

	void DisableEquipmentViewButtons()
	{
		SetAllEquipmentViewButtonsEnabled(false);
	}

	void SetAllEquipmentViewButtonsEnabled(bool enabled)
	{
		foreach (ShipEquipmentView weaponView in view.GetEquipmentViews())
			weaponView.SetButtonInteractable(enabled);
	}

	protected override void HandleEquipmentButtonPress(ShipEquipmentView buttonView)
	{
		base.HandleEquipmentButtonPress(buttonView);
		SetAllEquipmentViewButtonsEnabled(false);
	}

	protected override void HandleEquipmentButtonAnimationFinish()
	{
		TryEnableEquipmentViewButtons();
	}*/
}
