using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract	class EnemyShipStatusEffect: ShipStatusEffect
{

	protected override void ExtenderActivation(object activateOnObject)
	{
		EnemyShipModel activateOnEnemyShip = activateOnObject as EnemyShipModel;
		Debug.Assert(activateOnEnemyShip != null, "Trying to activate enemy ship effect on non-player ship!");
		CastExtenderActivation(activateOnEnemyShip);
	}

	protected override void CastExtenderActivation(ShipModel useOnShipModel)
	{
		EnemyShipModel activateOnEnemyShip = useOnShipModel as EnemyShipModel;
		Debug.Assert(activateOnEnemyShip != null, "Trying to activate enemy ship effect on non-player ship!");
		CastExtenderActivation(activateOnEnemyShip);
	}

	protected abstract void CastExtenderActivation(EnemyShipModel useOnEnemyShipModel);
}


