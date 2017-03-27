using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class StatusEffectEquipment: ShipEquipment
{

	public StatusEffect appliedStatusEffect
	{
		get { return _appliedStatusEffect; }
		protected set
		{
			_appliedStatusEffect = value;
			if (value!=null)
				description = _appliedStatusEffect.description;
		}
	}
	StatusEffect _appliedStatusEffect;
}

public class Siphon : StatusEffectEquipment
{
	protected override void Initialize()
	{
		maxCooldownTime = 2;
		blueEnergyCostToUse = 100;
		//greenEnergyCostToUse = 50;

		name = "Siphon";
		appliedStatusEffect = new EnergySiphonEffect();
	}
}



public class ReactiveArmor : StatusEffectEquipment
{
	protected override void Initialize()
	{
		maxCooldownTime = 2;
		greenEnergyCostToUse = 50;

		name = "Reactive Armor";
		appliedStatusEffect = new ReactiveArmorEffect();
	}
}
public class ReactiveArmorTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 100;
		materialsRequired = 200;

		providesEquipment = new ReactiveArmor();
	}
}

public class MeltdownTrigger : StatusEffectEquipment
{
	protected override void Initialize()
	{
		maxCooldownTime = 2;
		generatorLevelDelta = 1;
		name = "Meltdown Trigger";
		appliedStatusEffect = new MeltdownEffect();
	}
}
public class MeltdownTriggerTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 100;
		materialsRequired = 200;

		providesEquipment = new MeltdownTrigger();
	}
}