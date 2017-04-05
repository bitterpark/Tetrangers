using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Ability : ShipEquipment
{
	//public static event UnityEngine.Events.UnityAction<ShipModel> EAbilityUsed;
	static List<ShipModel> disabledForShips = new List<ShipModel>();

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		base.ExtenderActivation(activateOnShip);
		DisableAbilitiesForShip(activateOnShip);
		//if (EAbilityUsed != null) EAbilityUsed(activateOnShip);
	}

	static void DisableAbilitiesForShip(ShipModel ship)
	{
		disabledForShips.Add(ship);
		BattleManager.EEngagementModeEnded += ReenableAllAbilities;
		BattleManager.EBattleFinished += ReenableAllAbilities;
	}

	static void ReenableAllAbilities()
	{
		disabledForShips.Clear();
		BattleManager.EEngagementModeEnded -= ReenableAllAbilities;
		BattleManager.EBattleFinished -= ReenableAllAbilities;
	}

	public Ability()
	{
		//EAbilityUsed += DisableAbility;
		equipmentType = EquipmentTypes.Skill;
	}

	public override bool IsUsableByShip(ShipModel ship)
	{
		return !disabledForShips.Contains(ship) && base.IsUsableByShip(ship);
	}
}

public class Overdrive : Ability
{
	protected override void Initialize()
	{
		maxCooldownTime = 4;
		name = "Overdrive";
		onSelfEffect = new BlueAmplificationEffect();//new OverdriveEffect();
	}
}

public class BlitzMode : Ability
{
	protected override void Initialize()
	{
		maxCooldownTime = 4;
		name = "Blitz Mode";
		description = "Reduce shields by half. Gain blue energy equal to lost shields.";
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		base.ExtenderActivation(activateOnShip);
		int shieldLoss = activateOnShip.shipShields/2;
		activateOnShip.ChangeShields(-shieldLoss);
		activateOnShip.GainBlueEnergy(shieldLoss, true);
	}
}

public class Coolant : Ability
{
	protected override void Initialize()
	{
		maxCooldownTime = 1;
		name = "Coolant";
		onSelfEffect = new CoolantEffect();
	}
}

