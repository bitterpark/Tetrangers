using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class ShipStatusEffect: StatusEffect
{
	protected override void SubclassActivation(object activateOnObject)
	{
		//if (activateOnObject.GetType() == typeof(PlayerShipModel))
			//BattleAI.EAITurnFinished += DeactivateEffect;
		//else if (activateOnObject.GetType().BaseType == typeof(EnemyShipModel))
			BattleManager.EEngagementModeStarted += DeactivateEffect;
		//else throw new NotImplementedException();
	}

	protected override void SubclassDeactivation()
	{
		//BattleAI.EAITurnFinished -= DeactivateEffect;
		BattleManager.EEngagementModeStarted -= DeactivateEffect;
	}

	protected override void ExtenderActivation(object activateOnObject)
	{
		ShipModel activateOnShip = activateOnObject as ShipModel;
		Debug.Assert(activateOnShip != null, "Trying to activate ship effect on non-ship!");
		CastExtenderActivation(activateOnShip);
	}

	protected abstract void CastExtenderActivation(ShipModel useOnShipModel);
}

public class OverdriveEffect : ShipStatusEffect
{
	int blueGainAdded = 65;
	ShipModel activeOnShip;

	protected override void InitializeValues()
	{

		name = "Overdrive";
		icon = SpriteDB.Instance.overdriveEffectSprite;
		description = "Until next engagement: overdrives the ship's generator, causing the figures to drop more rapidly, for double the blue energy gain";
		color = Color.red;
	}

	protected override void CastExtenderActivation(ShipModel activateOnShip)
	{
		//FigureController.accelerated = true;
		//blueGainAdded = activateOnShip.blueEnergyGain;
		activeOnShip = activateOnShip;
		//activeOnShip.energyUser.blueEnergyGain += blueGainAdded;
	}

	protected override void ExtenderDeactivation()
	{
		//FigureController.accelerated = false;
		//activeOnShip.energyUser.blueEnergyGain -= blueGainAdded;
		activeOnShip = null;
	}
}

public class CoolantEffect : ShipStatusEffect
{
	//int blueGainAdded = 0;
	//ShipModel activeOnShip;
	int greenGainAdded = 12;
	ShipModel activeOnShip;

	protected override void InitializeValues()
	{

		name = "Coolant";
		icon = SpriteDB.Instance.coolantEffectSprite;
		description = "Until next engagement: Adds green energy gain";
		color = Color.cyan;
	}

	protected override void CastExtenderActivation(ShipModel activateOnShip)
	{
		//FigureSpawner.coolantMode = true;
		activeOnShip = activateOnShip;
		//activeOnShip.energyUser.greenEnergyGain += greenGainAdded;
	}

	protected override void ExtenderDeactivation()
	{
		//FigureSpawner.coolantMode = false;
		//activeOnShip.energyUser.greenEnergyGain -= greenGainAdded;
	}
}

public class ReactiveArmorEffect : ShipStatusEffect
{
	//int blueGainAdded = 0;
	ShipModel activeOnShip;

	const float damageReductionPercentage = 0.5f;

	protected override void InitializeValues()
	{
		name = "Reactive Armor";
		icon = SpriteDB.Instance.reactiveArmorEffectSprite;
		description = string.Format("Until next engagement: next damage taken will be reduced by {0}%", (int)(damageReductionPercentage * 100));
		color = Color.green;
	}

	protected override void CastExtenderActivation(ShipModel activateOnShip)
	{
		activeOnShip = activateOnShip;
		//activeOnShip.healthManager.EActivateDefences += ReduceDamage;

	}

	int ReduceDamage(int damage)
	{
		int reducedDamage = Mathf.RoundToInt(damage * damageReductionPercentage);
		DeactivateEffect();
		return reducedDamage;
	}

	protected override void ExtenderDeactivation()
	{
		//activeOnShip.healthManager.EActivateDefences -= ReduceDamage;
		activeOnShip = null;
	}
}





