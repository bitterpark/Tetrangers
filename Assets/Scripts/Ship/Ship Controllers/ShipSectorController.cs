using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ShipSectorController
{
	PlayerShipEquipmentController equipmentController;
	ShipEnergyController energyController;
	ShipSectorModel model;
	SectorView view;

	public ShipSectorController(ShipSectorModel model, SectorView view)
	{
		this.model = model;
		this.view = view;

		equipmentController = new PlayerShipEquipmentController(model.sectorEquipment, view.equipmentListView);
		energyController = new ShipEnergyController(view.energyView, model.energyManager);
	}



	public void Dispose()
	{
		equipmentController.DisposeController(false);
		energyController.DisposeController();
	}
}

