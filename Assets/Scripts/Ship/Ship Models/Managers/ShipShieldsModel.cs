using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ShipShieldsModel : ShipEnergyModel
{
	public event UnityEngine.Events.UnityAction EShieldsDamaged;

	int normalShieldsGain;

	public ShipShieldsModel(int shieldsGain, int shieldsMax) : base(shieldsGain, shieldsMax)
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
		int oldShields = resourceCurrent;
		resourceCurrent -= damage;
		energyGain = 0;

		if (damage > 0 && EShieldsDamaged != null) EShieldsDamaged();

		return Mathf.Min(0, oldShields - resourceCurrent);
	}

	public void Regen()
	{
		energyGain = normalShieldsGain;
	}
}

