using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class TabbedPlayerShipEquipmentController : PlayerShipEquipmentController
{

	protected TabController tabController;

	public TabbedPlayerShipEquipmentController(ShipEquipmentModel shipModel, TabbedEquipmentListView equipmentView) 
		: base(shipModel, equipmentView)
	{
		tabController = new TabController(equipmentView.tabButtonsView, this);
		tabController.ShowWeaponsTab();
	}

	public override void DisposeController(bool disposeModel)
	{
		tabController.Dispose();
		tabController = null;
		base.DisposeController(disposeModel);
	}
}

