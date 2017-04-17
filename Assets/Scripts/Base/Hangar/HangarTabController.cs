using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class HangarTabController : TabController
{
	public HangarTabController(TabButtonsView view, IEquipmentListController parentController) : base(view, parentController)
	{
		TabButtonsView.EEquipmentTabPressedGlobal += ShowEquipmentTab;
		TabButtonsView.EWeaponsTabPressedGlobal += ShowWeaponsTab;
		TabButtonsView.ESkillsTabPressedGlobal += ShowSkillsTab;
	}

	public override void Dispose()
	{
		TabButtonsView.EEquipmentTabPressedGlobal -= ShowEquipmentTab;
		TabButtonsView.EWeaponsTabPressedGlobal -= ShowWeaponsTab;
		TabButtonsView.ESkillsTabPressedGlobal -= ShowSkillsTab;
		base.Dispose();
	}
}

