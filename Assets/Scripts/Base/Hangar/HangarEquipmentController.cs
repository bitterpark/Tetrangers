using System;
using System.Collections.Generic;
using UnityEngine;
using DraggableUIObjects;

public class HangarEquipmentController : EquipmentListController
{
	public static event UnityEngine.Events.UnityAction<Transform, ShipEquipmentView, ShipEquipment> EEquipmentDroppedIntoList;

	static event UnityEngine.Events.UnityAction<EquipmentTypes> EGlobalTabSwitch;

	public HangarEquipmentController(IEquipmentListModel equipmentListModel, EquipmentListView equipmentView) : base(equipmentListModel, equipmentView)
	{
		//DraggableUIObject.EDroppedOnNewTarget += HandleEquipmentDropEvent;
		EEquipmentDroppedIntoList += HandleEquipmentDropEvent;
		EGlobalTabSwitch += ShowEquipmentTypeViews;
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		EEquipmentDroppedIntoList -= HandleEquipmentDropEvent;
		EGlobalTabSwitch -= ShowEquipmentTypeViews;
		//DraggableUIObject.EDroppedOnNewTarget -= HandleEquipmentDropEvent;
	}

	protected override void SetupEquipmentView(ShipEquipmentView newView, ShipEquipment equipment)
	{
		base.SetupEquipmentView(newView, equipment);
		newView.GetComponent<DraggableUIObject>().EInstanceDroppedOnNewTarget += HandleEquipmentDraggedAway;
	}

	protected override void UnsubscribeFromEquipmentView(ShipEquipmentView view)
	{
		base.UnsubscribeFromEquipmentView(view);
		view.GetComponent<DraggableUIObject>().EInstanceDroppedOnNewTarget -= HandleEquipmentDraggedAway;
	}

	protected override void ShowEquipmentTab()
	{
		if (EGlobalTabSwitch != null)
		{
			EGlobalTabSwitch -= ShowEquipmentTypeViews;
			EGlobalTabSwitch(EquipmentTypes.Equipment);
			EGlobalTabSwitch += ShowEquipmentTypeViews;
		}
		base.ShowEquipmentTab();
	}
	protected override void ShowSkillsTab()
	{
		if (EGlobalTabSwitch != null)
		{
			EGlobalTabSwitch -= ShowEquipmentTypeViews;
			EGlobalTabSwitch(EquipmentTypes.Skill);
			EGlobalTabSwitch += ShowEquipmentTypeViews;
		}
		base.ShowSkillsTab();
	}
	protected override void ShowWeaponsTab()
	{
		if (EGlobalTabSwitch != null)
		{
			EGlobalTabSwitch -= ShowEquipmentTypeViews;
			EGlobalTabSwitch(EquipmentTypes.Weapon);
			EGlobalTabSwitch += ShowEquipmentTypeViews;
		}
		base.ShowWeaponsTab();
	}

	void HandleEquipmentDropEvent(Transform droppedIntoTransform,ShipEquipmentView droppedView, ShipEquipment droppedEquipment)
	{
		if (droppedIntoTransform==view.GetEquipmentAreaTransform())
		{
			model.AddEquipment(droppedEquipment);
			SetupEquipmentView(droppedView, droppedEquipment);
			//Debug.Log("Equipment has been dropped into " + view.name);
		}
	}

	void HandleEquipmentDraggedAway(Transform droppedIntoTransform, DraggableUIObject draggedAwayObject)
	{
		ShipEquipmentView draggedAwayView = draggedAwayObject.GetComponent<ShipEquipmentView>();
		Debug.Assert(draggedAwayView != null, "Cannon find view attached to dragged away DraggableUIObject!");
		ShipEquipment draggedAwayEquipment = GetEquipmentRepresentedByView(draggedAwayView);
		Debug.Assert(draggedAwayEquipment != null, "Cannot find equipment in dragged away DraggableUIObject!");

		UnsubscribeFromEquipmentView(draggedAwayView);
		model.RemoveEquipment(draggedAwayEquipment);
		//Debug.Log("Equipment has been dragged away from "+view.name);
		if (EEquipmentDroppedIntoList != null) EEquipmentDroppedIntoList(droppedIntoTransform,draggedAwayView,draggedAwayEquipment);
	}

}

