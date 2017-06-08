using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class HealthAndShieldsManager
{
	public event UnityAction EHealthChanged
	{
		add { healthModel.EResourceChanged += value; }
		remove { healthModel.EResourceChanged -= value; }
	}
	public event UnityAction EHealthDamaged
	{
		add { healthModel.EHealthDamaged += value; }
		remove { healthModel.EHealthDamaged -= value; }
	}
	public event UnityAction EShieldsChanged
	{
		add { shieldsModel.EResourceChanged += value; }
		remove { shieldsModel.EResourceChanged -= value; }
	}
	public event UnityAction EShieldsGainChanged
	{
		add { shieldsModel.EEnergyGainChanged += value; }
		remove { shieldsModel.EEnergyGainChanged -= value; }
	}
	public event UnityAction EShieldsDamaged
	{
		add { shieldsModel.EShieldsDamaged += value; }
		remove { shieldsModel.EShieldsDamaged -= value; }
	}
	public event UnityAction EHealthDepleted
	{
		add { healthModel.EHealthRanOut += value; }
		remove { healthModel.EHealthRanOut -= value; }
	}

	public int health
	{
		get { return healthModel.resourceCurrent; }
		set { healthModel.resourceCurrent = value; }
	}
	public int healthMax
	{
		get { return healthModel.resourceMax; }
		set { healthModel.resourceMax = value; }
	}
	public int shields
	{
		get { return shieldsModel.resourceCurrent; }
		set { shieldsModel.resourceCurrent = value; }
	}
	public int shieldsMax
	{
		get { return shieldsModel.resourceMax; }
		set { shieldsModel.resourceMax = value; }
	}
	public int shieldsCurrentGain
	{
		get { return shieldsModel.energyGain; }
		set { shieldsModel.energyGain = value; }
	}

	public delegate int DefensesDeleg(int damage);
	public event DefensesDeleg EActivateDefences;

	ShieldsModel shieldsModel;
	HealthModel healthModel;

	public HealthAndShieldsManager(int healthMax, int shieldsMax, int shieldsGain)
	{
		shieldsModel = new ShieldsModel(shieldsGain, shieldsMax);
		healthModel = new HealthModel(healthMax);
	}

	public void Dispose()
	{
		shieldsModel.DisposeModel();
		healthModel.DisposeModel();
	}

	public void ResetToStartingShields()
	{
		shieldsModel.ResetToStartingStats();
	}
	public void ResetToStartingHealth()
	{
		healthModel.ResetToStartingStats();
	}

	public void TakeNonWeaponDamage(int damage)
	{
		AttackInfo nonWeaponAttack = new AttackInfo(damage);
		TakeDamage(nonWeaponAttack);
	}

	public void TakeDamage(AttackInfo attack)
	{
		int remainingDamage = attack.damage;
		if (EActivateDefences != null)
			remainingDamage = EActivateDefences(remainingDamage);

		bool tookDamage = false;

		if (remainingDamage > 0)
		{
			bool resistDamage = (attack.type == AttackType.Antihull) ? true : false;
			remainingDamage = shieldsModel.TakeDamage(remainingDamage, resistDamage);
			tookDamage = true;
		}
		if (remainingDamage > 0)
		{
			bool resistDamage = (attack.type == AttackType.Antishield) ? true : false;
			healthModel.TakeDamage(remainingDamage, resistDamage);
			tookDamage = true;
		}

		if (tookDamage)
			SoundFXPlayer.Instance.PlayTookDamageSound();

	}

	public void TakeDamage(int damage)
	{
		int remainingDamage = damage;

		if (remainingDamage > 0)
			remainingDamage = shieldsModel.TakeDamage(remainingDamage);

		if (remainingDamage > 0)
			healthModel.TakeDamage(remainingDamage); ;
	}

	public void RepairToPercentage(float percentage)
	{
		int repairedHealth = Mathf.Max(Mathf.RoundToInt(healthModel.resourceMax * percentage), 1);
		healthModel.resourceCurrent += repairedHealth;
	}

	public void RestoreShields(int restoreAmount)
	{
		shieldsModel.resourceCurrent += restoreAmount;
	}

	public void RegenShields()
	{
		shieldsModel.Regen();
	}

	public void IncreaseShieldsByGain()
	{
		IncreaseShieldsByGains(1);
	}

	public void IncreaseShieldsByGains(int multiplier)
	{
		shieldsModel.IncreaseByGains(multiplier);
	}

	public int GetActualShieldsIncrease()
	{
		return GetActualShieldsDelta(1, false);
	}

	public int GetActualShieldsDelta(int attemptedDelta)
	{
		return GetActualShieldsDelta(attemptedDelta, true);
	}

	public int GetActualShieldsDelta(int attemptedDelta, bool absolute)
	{
		return shieldsModel.GetActualDelta(attemptedDelta, absolute);
	}

}

