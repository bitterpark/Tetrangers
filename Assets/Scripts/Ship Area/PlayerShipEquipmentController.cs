using System;
using System.Collections.Generic;


public class PlayerShipEquipmentController : ShipEquipmentController
{
	public PlayerShipEquipmentController(ShipModel shipModel, EquipmentListView equipmentView) : base(shipModel, equipmentView)
	{
		EnemyShipEquipmentController.EEnemyTurnFinished += TryEnableEquipmentViewButtons;
		BattleManager.EEngagementModeEnded += DisableEquipmentViewButtons;
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		EnemyShipEquipmentController.EEnemyTurnFinished -= TryEnableEquipmentViewButtons;
		BattleManager.EEngagementModeEnded -= DisableEquipmentViewButtons;
	}

	protected override void SetupEquipmentView(ShipEquipmentView newView, ShipEquipment equipment)
	{
		base.SetupEquipmentView(newView, equipment);
	}

	protected override void UnsubscribeFromEquipmentView(ShipEquipmentView view)
	{
		base.UnsubscribeFromEquipmentView(view);
	}

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
		foreach (ShipEquipmentView weaponView in view.GetEnabledEquipmentViews())
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
	}

}

