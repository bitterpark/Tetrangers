using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EquipmentListView: MonoBehaviour
{
	[SerializeField]
	Transform equipmentArea;

	[SerializeField]
	ShipEquipmentView equipmentViewPrefab;

	public ShipEquipmentView[] CreateEquipmentViews(int count)
	{
		ShipEquipmentView[] views = new ShipEquipmentView[count];
		for (int i = 0; i < count; i++)
			views[i] = CreateEquipmentView();

		return views;
	}

	public ShipEquipmentView CreateEquipmentView()
	{
		ShipEquipmentView newView = Instantiate(equipmentViewPrefab);
		newView.transform.SetParent(equipmentArea, false);
		newView.Initialize();
		return newView;
	}

	public void ClearEquipmentViews()
	{
		foreach (ShipEquipmentView view in equipmentArea.GetComponentsInChildren<ShipEquipmentView>())
			view.DisposeView();
	}

	public ShipEquipmentView[] GetEnabledEquipmentViews()
	{
		return equipmentArea.GetComponentsInChildren<ShipEquipmentView>();
	}

	public Transform GetEquipmentAreaTransform()
	{
		return equipmentArea;
	}

	



	public virtual void ClearView()
	{
		ClearEquipmentViews();
	}

}

