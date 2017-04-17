using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipController : ShipCombatController 
{

	List<ShipSectorController> sectorControllers = new List<ShipSectorController>();

	public PlayerShipController(ShipModel model, PlayerShipViewProvider viewsProvider)
		: base(model, viewsProvider)
	{
		CreateEquipmentSectorControllers(viewsProvider);
	}

	protected override EquipmentListController CreateEquipmentController(ShipEquipmentModel model, IShipViewProvider viewProvider)
	{
		return new TabbedPlayerShipEquipmentController(model, view.equipmentListView);
	}

	void CreateEquipmentSectorControllers(PlayerShipViewProvider viewsProvider)
	{
		PlayerShipModel playerShip = model as PlayerShipModel;

		for (int i = 0; i < playerShip.shipSectors.Length; i++)
			sectorControllers.Add(new ShipSectorController(playerShip.shipSectors[i], viewsProvider.sectorViews[i]));
	}

	public override void DisposeController(bool disposeModel)
	{
		foreach (ShipSectorController controller in sectorControllers)
			controller.Dispose();
		base.DisposeController(disposeModel);
	}

}
