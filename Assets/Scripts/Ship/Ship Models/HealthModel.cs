using UnityEngine;
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
		TakeDamage(damage, false);
	}

	public void TakeDamage(int damage, bool lowerByHalf)
	{
		if (lowerByHalf)
			damage = Mathf.RoundToInt(damage * 0.5f);

		resourceCurrent -= damage;
		if (damage > 0 && EHealthDamaged != null) EHealthDamaged();
		if (resourceCurrent == 0 && EHealthRanOut != null) EHealthRanOut(); 
	}
}

