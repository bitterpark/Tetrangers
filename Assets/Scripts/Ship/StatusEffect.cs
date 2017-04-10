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

	public int durationRemaining { get; protected set; }

	public event UnityAction<StatusEffect> EStatusEffectEnded;

	public StatusEffect ()
	{
		InitializeValues();
	}

	protected abstract void InitializeValues();

	public void ActivateEffect(ShipModel activateOnShip)
	{
		ExtenderActivation(activateOnShip);

		if (activateOnShip.GetType()==typeof(PlayerShipController))
			BattleAI.EAITurnFinished += DeactivateEffect;
		else
			BattleManager.EEngagementModeStarted += DeactivateEffect;


		BattleManager.EBattleFinished += DeactivateEffect;
		
	}

	protected abstract void ExtenderActivation(ShipModel activateOnShip);

	protected void DeactivateEffect()
	{
		ExtenderDeactivation();
		if (EStatusEffectEnded != null) EStatusEffectEnded(this);
		EStatusEffectEnded = null;
		
		BattleManager.EBattleFinished -= DeactivateEffect;
		BattleAI.EAITurnFinished -= DeactivateEffect;
		BattleManager.EEngagementModeStarted -= DeactivateEffect;
	}
	protected virtual void ExtenderDeactivation() { }


}

public class MeltdownEffect : StatusEffect
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
	int blueGainAdded = 65;
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
		//FigureController.accelerated = true;
		//blueGainAdded = activateOnShip.blueEnergyGain;
		activeOnShip = activateOnShip;
		activeOnShip.ChangeBlueEnergyGain(blueGainAdded);
	}

	protected override void ExtenderDeactivation()
	{
		//FigureController.accelerated = false;
		activeOnShip.ChangeBlueEnergyGain(-blueGainAdded);
		activeOnShip = null;
	}
}

public class CoolantEffect : StatusEffect
{
	//int blueGainAdded = 0;
	//ShipModel activeOnShip;
	int greenGainAdded = 12;
	ShipModel activeOnShip;

	protected override void InitializeValues()
	{

		name = "Coolant";
		icon = SpriteDB.Instance.coolantEffectSprite;
		description = "Until next engagement: cools the generator, stopping new figures from falling";
		color = Color.cyan;
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		//FigureSpawner.coolantMode = true;
		activeOnShip = activateOnShip;
		activeOnShip.ChangeGreenEnergyGain(greenGainAdded);
	}

	protected override void ExtenderDeactivation()
	{
		//FigureSpawner.coolantMode = false;
		activeOnShip.ChangeGreenEnergyGain(-greenGainAdded);
	}
}



public class EnergySiphonEffect : StatusEffect
{
	//int blueGainAdded = 0;
	//EnemyShipModel activeOnEnemyShip;
	ShipModel activeOnShip;

	protected override void InitializeValues()
	{

		name = "Energy Siphon";
		icon = SpriteDB.Instance.siphonEffectSprite;
		description = "Until next engagement: gain blue energy every time the opponent does";
		color = Color.blue;
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		Debug.Assert(activateOnShip.GetType().BaseType == typeof(EnemyShipModel), "Activating Siphon effect for player ship!");
		activeOnShip = activateOnShip;
		PlayerShipModel.EPlayerGainedBlueEnergy += GainEnergy;
		//activeOnEnemyShip = activateOnShip as EnemyShipModel;
		//activeOnEnemyShip.energyGainForFigureHoverEnabled = true;

	}

	void GainEnergy(int gain)
	{
		activeOnShip.GainBlueEnergy(gain, true);
	}

	protected override void ExtenderDeactivation()
	{
		//activeOnEnemyShip.energyGainForFigureHoverEnabled = false;
		//activeOnEnemyShip = null;
		PlayerShipModel.EPlayerGainedBlueEnergy -= GainEnergy;
		activeOnShip = null;
	}
}

public class ReactiveArmorEffect : StatusEffect
{
	//int blueGainAdded = 0;
	ShipModel activeOnShip;

	const float damageReductionPercentage=0.5f;

	protected override void InitializeValues()
	{
		name = "Reactive Armor";
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

public class GreenAmplificationEffect : StatusEffect
{
	ShipModel activeOnShip;

	int blueGainDecrease;
	int greenGainIncrease;

	protected override void InitializeValues()
	{
		name = "Green Amplification Field";
		icon = SpriteDB.Instance.siphonEffectSprite;
		description = string.Format("Until next engagement: removes all blue energy gain, doubles green energy gain");//, BalanceValuesManager.Instance.bluePointsWorthPerGreenPoint);
		color = Color.green;
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		activeOnShip = activateOnShip;

		blueGainDecrease = activateOnShip.blueEnergyGain;
		greenGainIncrease = activeOnShip.greenEnergyGain;

		activeOnShip.ChangeEnergyGain(-blueGainDecrease,greenGainIncrease);
	}

	protected override void ExtenderDeactivation()
	{
		activeOnShip.ChangeEnergyGain(blueGainDecrease, -greenGainIncrease);
		activeOnShip = null;
	}
}

public class BlueAmplificationEffect : StatusEffect
{
	ShipModel activeOnShip;

	int blueGainIncrease;
	int greenGainDecrease;

	protected override void InitializeValues()
	{
		name = "Blue Amplification Field";
		icon = SpriteDB.Instance.siphonEffectSprite;
		description = string.Format("Until next engagement: removes all green energy gain, doubles blue energy gain");
		color = Color.cyan;
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		//Debug.Log("Activating blue amp");
		activeOnShip = activateOnShip;

		greenGainDecrease = activateOnShip.greenEnergyGain;
		blueGainIncrease = activeOnShip.blueEnergyGain;//greenGainDecrease*BalanceValuesManager.Instance.bluePointsWorthPerGreenPoint;

		activeOnShip.ChangeEnergyGain(blueGainIncrease, -greenGainDecrease);
	}

	protected override void ExtenderDeactivation()
	{
		//Debug.Log("Deactivating blue amp");
		activeOnShip.ChangeEnergyGain(-blueGainIncrease, greenGainDecrease);
		activeOnShip = null;
	}
}