using System;
using System.Collections.Generic;
using UnityEngine;
using DraggableUIObjects;

public class HangarEquipmentController : TabbedEquipmentListController
{
	public static event UnityEngine.Events.UnityAction<Transform, ShipEquipmentView, ShipEquipment> EEquipmentDroppedIntoList;

	public HangarEquipmentController(IEquipmentListModel equipmentListModel, TabbedEquipmentListView equipmentView) 
		: base(equipmentListModel, equipmentView)
	{
		EEquipmentDroppedIntoList += HandleEquipmentDropEvent;
	}

	protected override TabController CreateTabController(TabButtonsView tabsView)
	{
		return new HangarTabController(tabsView, this);
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		EEquipmentDroppedIntoList -= HandleEquipmentDropEvent;
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

	void HandleEquipmentDropEvent(Transform droppedIntoTransform,ShipEquipmentView droppedView, ShipEquipment droppedEquipment)
	{
		if (droppedIntoTransform == view.GetEquipmentAreaTransform())
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

