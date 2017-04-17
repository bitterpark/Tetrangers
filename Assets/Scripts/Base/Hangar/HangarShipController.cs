using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class HangarShipController : ShipController
{
	public HangarShipController(ShipModel model, ShipView view) : base(model, view)
	{

	}

	protected override EquipmentListController CreateEquipmentController(ShipEquipmentModel model, IShipViewProvider viewProvider)
	{
		return new HangarEquipmentController(model, viewProvider.shipView.equipmentListView);
	}

}

