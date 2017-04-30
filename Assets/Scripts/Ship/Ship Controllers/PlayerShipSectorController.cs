using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class PlayerShipSectorController : ShipSectorController
{
	public PlayerShipSectorController(PlayerShipSectorModel model, SectorView view) : base(model, view, CreateEquipmentController(model, view))
	{
		
	}

	static ShipEquipmentController CreateEquipmentController(PlayerShipSectorModel model, SectorView view)
	{
		return new PlayerShipEquipmentController(model.sectorEquipment, view.equipmentListView);
	}
}

