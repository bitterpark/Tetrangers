using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

public class EnemyEquipmentUser: EquipmentUser
{
	public static event UnityAction<int> EEnemyWeaponFired;
	public static event UnityAction<StatusEffect> EEnemyAppliedStatusEffectToPlayer;

	public EnemyEquipmentUser(ShipModel parentShip, IHasEnergy energyUser) : base(parentShip, energyUser)
	{
	}

	protected override void ApplyStatusEffectToOpponent(StatusEffect effect)
	{
		if (EEnemyAppliedStatusEffectToPlayer != null)
			EEnemyAppliedStatusEffectToPlayer(effect);
	}

	protected override void DoWeaponFireEvent(int weaponDamage)
	{
		if (EEnemyWeaponFired != null)
			EEnemyWeaponFired(weaponDamage);
	}

	public override void InitializeForBattle()
	{
		PlayerEquipmentUser.EPlayerWeaponFired += healthManager.TakeDamage;
		PlayerEquipmentUser.EPlayerAppliedStatusEffectToEnemy += statusEffectManager.AddNewStatusEffect;
	}

	public override void Dispose()
	{
		PlayerEquipmentUser.EPlayerWeaponFired -= healthManager.TakeDamage;
		PlayerEquipmentUser.EPlayerAppliedStatusEffectToEnemy -= statusEffectManager.AddNewStatusEffect;
	}
}

