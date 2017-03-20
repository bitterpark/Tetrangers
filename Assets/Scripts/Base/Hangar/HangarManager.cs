using System;
using System.Collections.Generic;
using UnityEngine;


public class HangarManager: Singleton<HangarManager>
{
	[SerializeField]
	ShipView playerShipView;
	[SerializeField]
	EquipmentListView equipmentListView;


	ShipController playerShipController;
	EquipmentListController equipmentListController;

	public void OpenHangarScreen(ShipModel playerShip, HangarModel hangar)
	{
		if (!gameObject.activeSelf)
			ActivateHangarManager();
		DisplayPlayerShip(playerShip);
		DisplayStoredEquipment(hangar);
	}

	void ActivateHangarManager()
	{
		gameObject.SetActive(true);
	}

	void DisplayPlayerShip(ShipModel playerShip)
	{
		playerShipController = new ShipController(playerShip, playerShipView);
	}

	void DisplayStoredEquipment(HangarModel hangar)
	{
		equipmentListController = new EquipmentListController(hangar,equipmentListView);
	}

}
