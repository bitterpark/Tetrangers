using System;
using System.Collections.Generic;
using UnityEngine.Events;


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

	public void TakeDamage(int damage)
	{
		int totalDamage = damage;
		if (EActivateDefences != null)
			totalDamage = EActivateDefences(damage);

		if (totalDamage > 0)
		{
			totalDamage = shieldsModel.TakeDamage(totalDamage);
			//SetShieldsGain(0);
		}
		if (totalDamage > 0)
			healthModel.resourceCurrent-=totalDamage;
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

