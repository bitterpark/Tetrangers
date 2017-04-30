using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerEquipmentUser : EquipmentUser
{
	public static event UnityAction<int> EPlayerWeaponFired;
	public static event UnityAction<StatusEffect> EPlayerAppliedStatusEffectToEnemy;

	public PlayerEquipmentUser(ShipModel parentShip, IHasEnergy energyUser) : base(parentShip, energyUser)
	{
	}

	

	protected override void ApplyStatusEffectToOpponent(StatusEffect effect)
	{
		if (EPlayerAppliedStatusEffectToEnemy != null)
			EPlayerAppliedStatusEffectToEnemy(effect);
	}

	protected override void DoWeaponFireEvent(int weaponDamage)
	{
		if (EPlayerWeaponFired != null)
			EPlayerWeaponFired(weaponDamage);
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

