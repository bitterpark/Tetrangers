using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public abstract class ShipEquipmentController : EquipmentListController
{
	protected new ShipEquipmentModel model;

	public ShipEquipmentController(ShipEquipmentModel shipModel, EquipmentListView equipmentView) : base(shipModel, equipmentView)
	{
		model = shipModel;
		ShipEquipment.EEquipmentCooldownChanged += UpdateCooldownTime;
		ShipWeapon.EWeaponLockonTimeChanged += UpdateWeaponLockonTime;
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		ShipEquipment.EEquipmentCooldownChanged -= UpdateCooldownTime;
		ShipWeapon.EWeaponLockonTimeChanged -= UpdateWeaponLockonTime;
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
		equipment.PlayActivationSound();
		//if (equipment != null)
			//ShowEquipmentTypeViews(equipment.equipmentType);
	}

	protected virtual void HandleEquipmentButtonAnimationFinish(ShipEquipmentView buttonView)
	{
		ShipEquipment equipment = GetEquipmentRepresentedByView(buttonView);
		model.UseEquipment(equipment);
	}

	void UpdateCooldownTime(ShipEquipment equipment, int currentCooldownTime)
	{
		List<ShipEquipmentView> allViews = new List<ShipEquipmentView>();
		allViews.AddRange(_equipmentViewPairings.Keys);

		ShipEquipmentView view = GetViewRepresentingEquipment(equipment);
		if (view != null)
			view.SetCooldownTime(currentCooldownTime, equipment.maxCooldownTime);
	}

	void UpdateWeaponLockonTime(ShipEquipment equipment, int currentLockonTime)
	{
		List<ShipEquipmentView> allViews = new List<ShipEquipmentView>();
		allViews.AddRange(_equipmentViewPairings.Keys);

		ShipEquipmentView view = GetViewRepresentingEquipment(equipment);
		if (view != null)
			view.SetLockonTime(currentLockonTime);
	}


}

