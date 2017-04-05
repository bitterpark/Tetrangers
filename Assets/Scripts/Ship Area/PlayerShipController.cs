using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipController : ShipCombatController 
{
	public PlayerShipController(ShipModel model, ShipView view)
		: base(model, view)
	{
		
	}

	protected override EquipmentListController CreateEquipmentController(ShipModel model, ShipView view)
	{
		return new PlayerShipEquipmentController(model, view);
	}
}
