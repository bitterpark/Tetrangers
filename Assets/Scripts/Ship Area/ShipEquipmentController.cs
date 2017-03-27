using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public abstract class ShipEquipmentController : EquipmentListController
{
	protected new ShipModel model;

	public ShipEquipmentController(ShipModel shipModel, EquipmentListView equipmentView) : base(shipModel, equipmentView)
	{
		model = shipModel;
		ShipEquipment.EEquipmentCooldownChanged += UpdateCooldownTime;
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		ShipEquipment.EEquipmentCooldownChanged -= UpdateCooldownTime;
	}


	protected override void SetupEquipmentView(ShipEquipmentView newView, ShipEquipment equipment)
	{
		base.SetupEquipmentView(newView, equipment);
		newView.EEquipmentButtonPressed += HandleEquipmentButtonPress;
		newView.EEquipmentButtonAnimationFinished += HandleEquipmentButtonAnimationFinish;
	}

	protected override void UnsubscribeFromEquipmentView(ShipEquipmentView view)
	{
		base.UnsubscribeFromEquipmentView(view);
		view.EEquipmentButtonPressed -= HandleEquipmentButtonPress;
		view.EEquipmentButtonAnimationFinished -= HandleEquipmentButtonAnimationFinish;
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
	}

}

