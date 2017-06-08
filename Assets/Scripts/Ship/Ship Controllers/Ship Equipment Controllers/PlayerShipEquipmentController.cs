using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipEquipmentController : ShipEquipmentController
{
	//List<SectorEquipmentListView> sectorListViews;

	public PlayerShipEquipmentController(ShipEquipmentModel shipModel
		, EquipmentListView equipmentView) 
		: base(shipModel, equipmentView)
	{
		//this.sectorListViews = sectorListViews;

		BattleAI.EAITurnStarted += DisableEquipmentViewButtons;
		BattleManager.EEngagementModeEnded += TryEnableEquipmentViewButtons;
		//BattleManager.EEngagementModeEnded += DisableEquipmentViewButtons;
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		BattleAI.EAITurnStarted -= DisableEquipmentViewButtons;
		BattleManager.EEngagementModeEnded -= TryEnableEquipmentViewButtons;
		//BattleManager.EEngagementModeEnded -= DisableEquipmentViewButtons;
	}


	void TryEnableEquipmentViewButtons()
	{
		List<ShipEquipment> usableEquipment;
		if (model.TryGetAllUsableEquipment(out usableEquipment))
		{
			foreach (ShipEquipmentView equipmentView in _equipmentViewPairings.Keys)
				if (usableEquipment.Contains(_equipmentViewPairings[equipmentView]))
					equipmentView.SetButtonInteractable(true);
				else
					equipmentView.SetButtonInteractable(false);
		}
	}

	void DisableEquipmentViewButtons()
	{
		SetAllEquipmentViewButtonsEnabled(false);
	}

	protected void SetAllEquipmentViewButtonsEnabled(bool enabled)
	{
		//Debug.Log("Setting player equipment vies to enabled:"+enabled);
		foreach (ShipEquipmentView weaponView in view.GetEnabledEquipmentViews())
			weaponView.SetButtonInteractable(enabled);
	}

	protected override void HandleEquipmentButtonPress(ShipEquipmentView buttonView)
	{
		base.HandleEquipmentButtonPress(buttonView);
		SetAllEquipmentViewButtonsEnabled(false);
	}

	protected override void HandleEquipmentButtonAnimationFinish(ShipEquipmentView equipmentView)
	{
		ShipEquipment equipmentInView = GetEquipmentRepresentedByView(equipmentView);
		IRequiresPlayerTargetSelect targetingInterface = equipmentInView as IRequiresPlayerTargetSelect;

		if (targetingInterface == null)
			HandleActivatingEquipment(equipmentView, true);
		else
		{
			targetingInterface.CallTargetSelectManager();

			UnityEngine.Events.UnityAction<bool> equipmentActivationWrapper = null;
			equipmentActivationWrapper =
				(bool targetSelected) =>
				{
					HandleActivatingEquipment(equipmentView, targetSelected);
					PlayerTargetSelectManager.ETargetSelectionFinished -= equipmentActivationWrapper;
				};
			PlayerTargetSelectManager.ETargetSelectionFinished += equipmentActivationWrapper;
		}
	}

	void HandleActivatingEquipment(ShipEquipmentView equipmentView, bool activate)
	{
		if (activate)
			base.HandleEquipmentButtonAnimationFinish(equipmentView);
		TryEnableEquipmentViewButtons();
	}

}

