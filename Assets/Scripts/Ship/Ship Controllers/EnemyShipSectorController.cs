using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EnemyShipSectorController : ShipSectorController
{
	public EnemyShipSectorController(ShipSectorModel model, EnemyShipViewProvider viewProvider) 
		: base(model, viewProvider.sectorView, CreateEquipmentController(model, viewProvider))
	{

	}

	static ShipEquipmentController CreateEquipmentController(ShipSectorModel model, EnemyShipViewProvider viewProvider)
	{
		return new EnemyShipEquipmentController(model.sectorEquipment, viewProvider.shipView.equipmentListView);
	}

}
