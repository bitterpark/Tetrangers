using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EquipmentListView: MonoBehaviour
{
	public event UnityAction EWeaponsTabPressed;
	public event UnityAction EEquipmentTabPressed;
	public event UnityAction ESkillsTabPressed;

	[SerializeField]
	Transform equipmentArea;

	[SerializeField]
	Button weaponsTabButton;
	[SerializeField]
	Button equipmentTabButton;
	[SerializeField]
	Button skillsTabButton;

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

	public ShipEquipmentView[] GetEquipmentViews()
	{
		return equipmentArea.GetComponentsInChildren<ShipEquipmentView>();
	}

	void Awake()
	{
		Initialize();	
	}

	protected virtual void Initialize()
	{
		weaponsTabButton.onClick.AddListener(() => { if (EWeaponsTabPressed != null) EWeaponsTabPressed(); });
		equipmentTabButton.onClick.AddListener(() => { if (EEquipmentTabPressed != null) EEquipmentTabPressed(); });
		skillsTabButton.onClick.AddListener(() => { if (ESkillsTabPressed != null) ESkillsTabPressed(); });
	}

	public virtual void ClearView()
	{
		EWeaponsTabPressed = null;
		EEquipmentTabPressed = null;
		ESkillsTabPressed = null;
		ClearEquipmentViews();
	}

}

