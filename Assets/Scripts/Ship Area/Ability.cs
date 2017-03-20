using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Ability : StatusEffectEquipment
{
	public static event UnityEngine.Events.UnityAction EAbilityUsed;

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		base.ExtenderActivation(activateOnShip);
		if (EAbilityUsed != null) EAbilityUsed();
	}

	public Ability()
	{
		EAbilityUsed += DisableAbility;
		equipmentType = EquipmentTypes.Skill;
	}

	void DisableAbility()
	{
		isUseable = false;
		//WATCH OUT FOR MEMORY LEAKS HERE
		EAbilityUsed -= DisableAbility;
		BattleManager.EEngagementModeEnded += EnableAbility;
	}
	void EnableAbility()
	{
		isUseable = true;
		//WATCH OUT FOR MEMORY LEAKS HERE
		EAbilityUsed += DisableAbility;
		BattleManager.EEngagementModeEnded -= EnableAbility;
	}

}

public class Overdrive : Ability
{
	protected override void Initialize()
	{
		maxCooldownTime = 4;
		name = "Overdrive";
		appliedStatusEffect = new OverdriveEffect();
	}
}

public class Coolant : Ability
{
	protected override void Initialize()
	{
		maxCooldownTime = 4;
		name = "Coolant";
		appliedStatusEffect = new CoolantEffect();
	}
}

public class GeneratorUp : Ability
{
	protected override void Initialize()
	{
		maxCooldownTime = 2;
		generatorLevelDelta = 1;
		name = "Generator Up";
		appliedStatusEffect = new GeneratorUpEffect();
	}
}

