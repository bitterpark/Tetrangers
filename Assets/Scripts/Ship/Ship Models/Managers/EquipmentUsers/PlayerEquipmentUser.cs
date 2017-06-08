using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerEquipmentUser : EquipmentUser
{
	public static event UnityAction<AttackInfo> EPlayerWeaponFired;
	public static event UnityAction<StatusEffect> EPlayerAppliedStatusEffectToEnemy;

	public PlayerEquipmentUser(ShipModel parentShip, IHasEnergy energyUser) : base(parentShip, energyUser)
	{
	}

	

	protected override void ApplyStatusEffectToOpponent(StatusEffect effect)
	{
		if (EPlayerAppliedStatusEffectToEnemy != null)
			EPlayerAppliedStatusEffectToEnemy(effect);
	}

	protected override void DoWeaponFireEvent(AttackInfo attack)
	{
		if (EPlayerWeaponFired != null)
			EPlayerWeaponFired(attack);
	}

	public override void InitializeForBattle()
	{
		//EnemyEquipmentUser.EEnemyWeaponFired += healthManager.TakeDamage;
		//EnemyEquipmentUser.EEnemyAppliedStatusEffectToPlayer += statusEffectManager.AddNewStatusEffect;
	}

	public override void Dispose()
	{
		//EnemyEquipmentUser.EEnemyWeaponFired -= healthManager.TakeDamage;
		//EnemyEquipmentUser.EEnemyAppliedStatusEffectToPlayer -= statusEffectManager.AddNewStatusEffect;
	}
}

