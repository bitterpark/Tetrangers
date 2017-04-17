using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class TabController
{
	IEquipmentListController parentController;
	TabButtonsView view;

	public TabController(TabButtonsView view , IEquipmentListController parentController)
	{
		this.parentController = parentController;
		this.view = view;

		view.EEquipmentTabPressed += ShowEquipmentTab;
		view.EWeaponsTabPressed += ShowWeaponsTab;
		view.ESkillsTabPressed += ShowSkillsTab;
	}

	public virtual void Dispose()
	{
		view.EEquipmentTabPressed -= ShowEquipmentTab;
		view.EWeaponsTabPressed -= ShowWeaponsTab;
		view.ESkillsTabPressed -= ShowSkillsTab;
	}

	public virtual void ShowWeaponsTab()
	{
		ShowEquipmentTypeViews(EquipmentTypes.Weapon);
	}
	public virtual void ShowEquipmentTab()
	{
		ShowEquipmentTypeViews(EquipmentTypes.Equipment);
	}
	public virtual void ShowSkillsTab()
	{
		ShowEquipmentTypeViews(EquipmentTypes.Skill);
	}

	public virtual void ShowEquipmentTypeViews(EquipmentTypes showType)
	{
		Dictionary<ShipEquipmentView, ShipEquipment> equipmentViewPairings = parentController.equipmentViewPairings;

		foreach (ShipEquipmentView view in equipmentViewPairings.Keys)
		{
			if (equipmentViewPairings[view].equipmentType == showType)
				view.SetViewHidden(false);
			else
				view.SetViewHidden(true);
		}

	}
}

