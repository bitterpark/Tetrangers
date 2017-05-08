using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

public class HealthModel : ResourceModel
{
	public event UnityAction EHealthRanOut;
	public event UnityAction EHealthDamaged;

	public HealthModel(int resourceMax) : base(resourceMax)
	{
		resourceCurrent = resourceMax;
	}

	public override void DisposeModel()
	{
		EHealthRanOut = null;
		EHealthDamaged = null;
		base.DisposeModel();
	}

	public override void ResetToStartingStats()
	{
		resourceCurrent = resourceMax;
	}

	public void TakeDamage(int damage)
	{
		resourceCurrent -= damage;
		if (damage > 0 && EHealthDamaged != null) EHealthDamaged();
		if (resourceCurrent == 0 && EHealthRanOut != null) EHealthRanOut(); 
	}
}

