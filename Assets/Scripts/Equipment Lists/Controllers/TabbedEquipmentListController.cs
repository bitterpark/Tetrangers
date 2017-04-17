using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class TabbedEquipmentListController : EquipmentListController
{
	protected TabController tabController;

	public TabbedEquipmentListController(IEquipmentListModel listModel, TabbedEquipmentListView listView) : base(listModel, listView)
	{
		tabController = CreateTabController(listView.tabButtonsView);
		tabController.ShowWeaponsTab();
	}

	protected virtual TabController CreateTabController(TabButtonsView tabsView)
	{
		return new TabController(tabsView, this);
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		tabController.Dispose();
	}
}

