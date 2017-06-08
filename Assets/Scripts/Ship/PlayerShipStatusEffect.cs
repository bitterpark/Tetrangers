using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class PlayerShipStatusEffect : ShipStatusEffect
{
	protected override void ExtenderActivation(object activateOnObject)
	{
		PlayerShipModel activateOnPlayerShip = activateOnObject as PlayerShipModel;
		Debug.Assert(activateOnPlayerShip != null, "Trying to activate player ship effect on non-player ship!");
		CastExtenderActivation(activateOnPlayerShip);
	}

	protected override void CastExtenderActivation(ShipModel useOnShipModel)
	{
		PlayerShipModel activateOnPlayerShip = useOnShipModel as PlayerShipModel;
		Debug.Assert(activateOnPlayerShip != null, "Trying to activate player ship effect on non-player ship!");
		CastExtenderActivation(activateOnPlayerShip);
	}

	protected abstract void CastExtenderActivation(PlayerShipModel useOnPlayerShipModel);
}
/*
public class MeltdownEffect : PlayerShipStatusEffect
{
	int blueGainAdded = 0;
	//PlayerShipModel activeOnPlayerShip;

	protected override void InitializeValues()
	{
		name = "Meltdown";
		icon = SpriteDB.Instance.overdriveEffectSprite;
		description = "Until next engagement: Overloads dropped figures, providing bonus energy for settling figures quicker";
		color = Color.yellow;
	}

	protected override void CastExtenderActivation(PlayerShipModel activateOnShip)
	{
		//Debug.Assert(activateOnShip.GetType() == typeof(PlayerShipModel), "Trying to activate player ship ability on an enemy ship!");
		//PlayerShipModel.energyGainPerSecondsSavedEnabled = true;
	}

	protected override void ExtenderDeactivation()
	{
		//PlayerShipModel.energyGainPerSecondsSavedEnabled = false;
	}
}*/

