using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ShieldsModel : EnergyResourceModel
{
	public event UnityEngine.Events.UnityAction EShieldsDamaged;

	int normalShieldsGain;

	public ShieldsModel(int shieldsGain, int shieldsMax) : base(shieldsGain, shieldsMax)
	{
		resourceCurrent = shieldsMax;
		normalShieldsGain = shieldsGain;
	}

	public override void ResetToStartingStats()
	{
		resourceCurrent = resourceMax;
		energyGain = normalShieldsGain;
	}

	public override void DisposeModel()
	{
		base.DisposeModel();
		EShieldsDamaged = null;
	}

	public int TakeDamage(int damage)
	{
		return TakeDamage(damage, false);
	}
	public int TakeDamage(int damage, bool lowerByHalf)
	{
		if (lowerByHalf)
			damage = Mathf.RoundToInt(damage * 0.5f);

		int overflowingDamage = Mathf.Max(damage - resourceCurrent, 0);
		resourceCurrent -= damage;
		if (damage > 0 && EShieldsDamaged != null) EShieldsDamaged();

		if (lowerByHalf)
			overflowingDamage *= 2;

		return overflowingDamage;
	}

	public void Regen()
	{
		energyGain = normalShieldsGain;
	}
}

