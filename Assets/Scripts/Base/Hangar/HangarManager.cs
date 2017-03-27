using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HangarManager: BaseSubscreen
{
	[SerializeField]
	ShipView playerShipView;
	[SerializeField]
	EquipmentListView equipmentListView;


	ShipController playerHangarShipController;
	EquipmentListController equipmentListController;

	public override void OpenSubscreen()
	{
		base.OpenSubscreen();
		DisplayPlayerShip(GameDataManager.Instance.playerShip);
		DisplayStoredEquipment(GameDataManager.Instance.playerHangar);
	}

	void DisplayPlayerShip(ShipModel playerShip)
	{
		playerHangarShipController = new HangarShipController(playerShip, playerShipView);
	}

	void DisplayStoredEquipment(HangarModel hangar)
	{
		equipmentListController = new HangarEquipmentController(hangar,equipmentListView);
	}

	protected override void CloseSubscreen()
	{
		playerHangarShipController.DisposeController(false);
		equipmentListController.DisposeController(false);
		base.CloseSubscreen();
	}

}
