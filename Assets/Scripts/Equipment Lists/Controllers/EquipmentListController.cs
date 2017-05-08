using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipmentListController
{
	List<ShipEquipmentView> equipmentViews { get; }
	Dictionary<ShipEquipmentView, ShipEquipment> equipmentViewPairings { get; }
}

public class EquipmentListController : IEquipmentListController
{
	public Dictionary<ShipEquipmentView, ShipEquipment> equipmentViewPairings { get { return _equipmentViewPairings; } }
	public List<ShipEquipmentView> equipmentViews { get { return new List<ShipEquipmentView>(_equipmentViewPairings.Keys); } }

	protected Dictionary<ShipEquipmentView, ShipEquipment> _equipmentViewPairings = new Dictionary<ShipEquipmentView, ShipEquipment>();

	protected EquipmentListView view;
	protected IEquipmentListModel model;

	public EquipmentListController(IEquipmentListModel equipmentListModel, EquipmentListView equipmentView)
	{
		this.model = equipmentListModel;
		this.view = equipmentView;

		ShipEquipmentView.EEquipmentMouseoverStopped += HandleEquipmentMouseoverStop;

		List<ShipEquipment> storedEquipment = model.GetStoredEquipment();

		Debug.Assert(view != null, "view is null!");		

		foreach (ShipEquipment equipment in storedEquipment)
		{
			ShipEquipmentView newView = view.CreateEquipmentView();
			SetupEquipmentView(newView,equipment);
		}
	}

	protected virtual void SetupEquipmentView(ShipEquipmentView newView, ShipEquipment equipment)
	{
		newView.SetDisplayValues(
			equipment.blueEnergyCostToUse
			, equipment.greenEnergyCostToUse
			,equipment.shipEnergyCostToUse
			,equipment.ammoCostToUse
			,equipment.partsCostToUse
			,equipment.generatorLevelDelta
			,equipment.maxCooldownTime
			, equipment.name
			);
		if (equipment.equipmentType == EquipmentTypes.Weapon)
		{
			ShipWeapon weapon = equipment as ShipWeapon;
			newView.SetDamage(weapon.damageInfo);
			newView.SetLockonTime(weapon.lockOnTimeRemaining);
		}
		newView.SetButtonInteractable(false);
		newView.EEquipmentMousedOver += HandleEquipmentMouseover;
		

		_equipmentViewPairings.Add(newView, equipment);
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
		_equipmentViewPairings.TryGetValue(view, out equipmentInView);
		return equipmentInView;
	}

	protected ShipEquipmentView GetViewRepresentingEquipment(ShipEquipment equipment)
	{
		ShipEquipmentView resultView = null;

		foreach (ShipEquipmentView view in _equipmentViewPairings.Keys)
		{
			if (_equipmentViewPairings[view] == equipment)
			{
				resultView = view;
				break;
			}
		}

		return resultView;
	}

	public List<ShipEquipmentView> GetViewsRepresentingEquipment(List<ShipEquipment> equipmentList)
	{
		List<ShipEquipmentView> views = new List<ShipEquipmentView>();

		List<ShipEquipment> equipmentListBuffer = new List<ShipEquipment>(equipmentList);
		foreach (ShipEquipmentView view in _equipmentViewPairings.Keys)
		{
			ShipEquipment equipmentInView = _equipmentViewPairings[view];
			if (equipmentListBuffer.Contains(equipmentInView))
			{
				views.Add(view);
				equipmentListBuffer.Remove(equipmentInView);
			}
		}
		return views;
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

		view.ClearView();
	}

	protected virtual void UnsubscribeFromEquipmentView(ShipEquipmentView view)
	{
		_equipmentViewPairings.Remove(view);
		view.EEquipmentMousedOver -= HandleEquipmentMouseover;
	}
}
