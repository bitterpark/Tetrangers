﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class PassiveShipEquipment : ShipEquipment
{

	public override bool IsUsableByShip(ShipModel ship)
	{
		return false;
	}

	public override void SetOwner(object ownerObject)
	{
		base.SetOwner(ownerObject);

		BattleManager.EBattleStarted += ActivatePassiveEffect;
		BattleManager.EBattleFinished += DeactivatePassiveEffect;
	}

	public override void Dispose()
	{
		BattleManager.EBattleStarted -= ActivatePassiveEffect;
		BattleManager.EBattleFinished -= DeactivatePassiveEffect;
	}

	protected abstract void ActivatePassiveEffect();
	protected abstract void DeactivatePassiveEffect();

}

