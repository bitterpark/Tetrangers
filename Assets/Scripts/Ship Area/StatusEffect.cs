using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StatusEffects;
using UnityEngine.Events;

public abstract class StatusEffect : IDisplayableStatusEffect
{
	public Color color
	{
		get; protected set;
	}

	public Sprite icon
	{
		get; protected set;
	}

	public string description
	{
		get; protected set;
	}

	public string name
	{
		get; protected set;
	}

	public event UnityAction<StatusEffect> EStatusEffectEnded;

	public StatusEffect ()
	{
		InitializeValues();
	}

	protected abstract void InitializeValues();

	public void ActivateEffect(ShipModel activateOnShip)
	{
		ExtenderActivation(activateOnShip);
		BattleManager.EBattleFinished += DeactivateEffect;
		BattleManager.EEngagementModeStarted += DeactivateEffect;
	}

	protected abstract void ExtenderActivation(ShipModel activateOnShip);

	protected void DeactivateEffect()
	{
		ExtenderDeactivation();
		if (EStatusEffectEnded != null) EStatusEffectEnded(this);
		EStatusEffectEnded = null;
		
		BattleManager.EBattleFinished -= DeactivateEffect;
		BattleManager.EEngagementModeStarted -= DeactivateEffect;
	}
	protected virtual void ExtenderDeactivation() { }


}

public class GeneratorUpEffect : StatusEffect
{
	int blueGainAdded = 0;
	//PlayerShipModel activeOnPlayerShip;

	protected override void InitializeValues()
	{

		name = "Generator Up";
		icon = SpriteDB.Instance.overdriveEffectSprite;
		description = "Until next engagement: Overloads dropped figures, providing bonus energy for settling figures quicker";
		color = Color.yellow;
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		Debug.Assert(activateOnShip.GetType() == typeof(PlayerShipModel), "Trying to activate player ship ability on an enemy ship!");
		PlayerShipModel.energyGainPerSecondsSavedEnabled = true;
	}

	protected override void ExtenderDeactivation()
	{
		PlayerShipModel.energyGainPerSecondsSavedEnabled = false;
	}
}

public class OverdriveEffect: StatusEffect
{
	int blueGainAdded = 0;
	ShipModel activeOnShip;

	protected override void InitializeValues()
	{
		
		name = "Overdrive";
		icon = SpriteDB.Instance.overdriveEffectSprite;
		description = "Until next engagement: overdrives the ship's generator, causing the figures to drop more rapidly, for double the blue energy gain";
		color = Color.red;
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		FigureController.accelerated = true;
		blueGainAdded = activateOnShip.blueEnergyGain;
		activateOnShip.blueEnergyGain += blueGainAdded;
		activeOnShip = activateOnShip;
		//Debug.Log("Overdrive activated!");
	}

	protected override void ExtenderDeactivation()
	{
		FigureController.accelerated = false;
		activeOnShip.blueEnergyGain -= blueGainAdded;
		activeOnShip = null;
	}
}

public class CoolantEffect : StatusEffect
{
	//int blueGainAdded = 0;
	//ShipModel activeOnShip;

	protected override void InitializeValues()
	{

		name = "Coolant";
		icon = SpriteDB.Instance.coolantEffectSprite;
		description = "Until next engagement: cools the generator, stopping new figures from falling";
		color = Color.cyan;
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		FigureSpawner.coolantMode = true;
	}

	protected override void ExtenderDeactivation()
	{
		FigureSpawner.coolantMode = false;
	}
}



public class EnergySiphonEffect : StatusEffect
{
	//int blueGainAdded = 0;
	EnemyShipModel activeOnEnemyShip;

	protected override void InitializeValues()
	{

		name = "Energy Siphon";
		icon = SpriteDB.Instance.siphonEffectSprite;
		description = "Until next engagement: gain blue energy for every second of figure placement";
		color = Color.blue;
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		Debug.Assert(activateOnShip.GetType().BaseType == typeof(EnemyShipModel), "Activating Siphon effect for player ship!");
		activeOnEnemyShip = activateOnShip as EnemyShipModel;
		activeOnEnemyShip.energyGainForFigureHoverEnabled = true;

	}

	protected override void ExtenderDeactivation()
	{
		activeOnEnemyShip.energyGainForFigureHoverEnabled = false;
		activeOnEnemyShip = null;
	}
}

public class ReactiveArmorEffect : StatusEffect
{
	//int blueGainAdded = 0;
	ShipModel activeOnShip;

	const float damageReductionPercentage=0.5f;

	protected override void InitializeValues()
	{
		name = "Energy Siphon";
		icon = SpriteDB.Instance.reactiveArmorEffectSprite;
		description = string.Format("Until next engagement: next damage taken will be reduced by {0}%",(int)(damageReductionPercentage*100));
		color = Color.green;
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		activeOnShip = activateOnShip;
		activeOnShip.EActivateDefences += ReduceDamage;

	}

	int ReduceDamage(int damage)
	{
		int reducedDamage = Mathf.RoundToInt(damage*damageReductionPercentage);
		DeactivateEffect();
		return reducedDamage;
	}

	protected override void ExtenderDeactivation()
	{
		activeOnShip.EActivateDefences -= ReduceDamage;
		activeOnShip = null;
	}
}