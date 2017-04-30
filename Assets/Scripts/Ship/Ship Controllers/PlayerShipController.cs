using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipController : ShipCombatController 
{

	List<ShipSectorController> sectorControllers = new List<ShipSectorController>();

	ShipResourceController resourceController;
	public PlayerShipController(PlayerShipModel model, PlayerShipViewProvider viewsProvider)
		: base(model, viewsProvider)
	{
		CreateSectorControllers(viewsProvider);
		resourceController = new ShipResourceController(viewsProvider.resourceView, model.resourceManager);
	}

	protected override EquipmentListController CreateEquipmentController(ShipEquipmentModel model, IShipViewProvider viewProvider)
	{
		return new TabbedPlayerShipEquipmentController(model, view.equipmentListView);
	}

	void CreateSectorControllers(PlayerShipViewProvider viewsProvider)
	{
		PlayerShipModel playerShip = model as PlayerShipModel;

		for (int i = 0; i < playerShip.shipSectors.Length; i++)
			sectorControllers.Add(new PlayerShipSectorController(playerShip.shipSectors[i], viewsProvider.sectorViews[i]));
	}

	public override void DisposeController(bool disposeModel)
	{
		foreach (ShipSectorController controller in sectorControllers)
			controller.Dispose();
		resourceController.DisposeController();
		base.DisposeController(disposeModel);
	}

}
