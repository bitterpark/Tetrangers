using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentListController
{
	protected EquipmentListView view;
	protected IEquipmentListModel model;

	protected Dictionary<ShipEquipmentView, ShipEquipment> equipmentViewPairings = new Dictionary<ShipEquipmentView, ShipEquipment>();

	public EquipmentListController(IEquipmentListModel equipmentListModel, EquipmentListView equipmentView)
	{
		this.model = equipmentListModel;
		this.view = equipmentView;
		//shipController = new ShipController(playerShip,playerShipView);

		ShipEquipmentView.EEquipmentMouseoverStopped += HandleEquipmentMouseoverStop;

		List<ShipEquipment> storedEquipment = model.GetStoredEquipment();

		Debug.Assert(view != null, "view is null!");

		view.EEquipmentTabPressed += ShowEquipmentTab;
		view.EWeaponsTabPressed += ShowWeaponsTab;
		view.ESkillsTabPressed += ShowSkillsTab;

		foreach (ShipEquipment equipment in storedEquipment)
		{
			ShipEquipmentView newView = view.CreateEquipmentView();
			SetupEquipmentView(newView,equipment);
		}

		ShowWeaponsTab();

	}

	protected virtual void SetupEquipmentView(ShipEquipmentView newView, ShipEquipment equipment)
	{
		newView.SetDisplayValues(equipment.blueEnergyCostToUse, equipment.greenEnergyCostToUse, equipment.generatorLevelDelta, equipment.name);
		if (equipment.equipmentType == EquipmentTypes.Weapon)
		{
			ShipWeapon weapon = equipment as ShipWeapon;
			newView.SetDamage(weapon.damage);
		}
		newView.SetCooldownTime(equipment.cooldownTimeRemaining, equipment.maxCooldownTime);
		newView.SetButtonInteractable(false);
		newView.EEquipmentMousedOver += HandleEquipmentMouseover;
		

		equipmentViewPairings.Add(newView, equipment);
	}

	protected virtual void ShowWeaponsTab()
	{
		ShowEquipmentTypeViews(EquipmentTypes.Weapon);
	}
	protected virtual void ShowEquipmentTab()
	{
		ShowEquipmentTypeViews(EquipmentTypes.Equipment);
	}
	protected virtual void ShowSkillsTab()
	{
		ShowEquipmentTypeViews(EquipmentTypes.Skill);
	}

	protected virtual void ShowEquipmentTypeViews(EquipmentTypes showType)
	{
			foreach (ShipEquipmentView view in equipmentViewPairings.Keys)
			{
				if (equipmentViewPairings[view].equipmentType == showType)
					view.SetViewHidden(false);
				else
					view.SetViewHidden(true);
			}

	}

	void HandleEquipmentMouseover(ShipEquipmentView equipmentView)
	{
		ShipEquipment equipment = GetEquipmentRepresentedByView(equipmentView);

		if (equipment != null && equipment.hasDescription)
			TooltipManager.Instance.CreateTooltip(equipment.description, equipmentView.transform);
	}

	protected ShipEquipment GetEquipmentRepresentedByView(ShipEquipmentView view)
	{
		ShipEquipment equipmentInView = null;
		equipmentViewPairings.TryGetValue(view, out equipmentInView);
		return equipmentInView;
	}

	protected ShipEquipmentView GetViewRepresentingEquipment(ShipEquipment equipment)
	{
		ShipEquipmentView resultView = null;

		foreach (ShipEquipmentView view in equipmentViewPairings.Keys)
		{
			if (equipmentViewPairings[view] == equipment)
			{
				resultView = view;
				break;
			}
		}

		return resultView;
	}

	void HandleEquipmentMouseoverStop()
	{
		TooltipManager.Instance.DestroyAllTooltips();
	}

	public virtual void DisposeController(bool disposeModel)
	{
		//ShipEquipment.EEquipmentCooldownChanged -= UpdateCooldownTime;
		ShipEquipmentView.EEquipmentMouseoverStopped -= HandleEquipmentMouseoverStop;

		foreach (ShipEquipmentView equipmentView in new List<ShipEquipmentView>(view.GetEnabledEquipmentViews()))
			UnsubscribeFromEquipmentView(equipmentView);

		view.EEquipmentTabPressed -= ShowEquipmentTab;
		view.EWeaponsTabPressed -= ShowWeaponsTab;
		view.ESkillsTabPressed -= ShowSkillsTab;

		view.ClearView();
	}

	protected virtual void UnsubscribeFromEquipmentView(ShipEquipmentView view)
	{
		equipmentViewPairings.Remove(view);
		view.EEquipmentMousedOver -= HandleEquipmentMouseover;
	}

}
