using UnityEngine;


public class HealthController
{
	HealthView view;
	HealthAndShieldsManager healthModel;

	public HealthController(HealthView view, HealthAndShieldsManager healthModel)
	{
		this.view = view;
		this.healthModel = healthModel;

		healthModel.EHealthChanged += UpdateHealth;
		healthModel.EShieldsChanged += UpdateShields;
		healthModel.EHealthDamaged += DisplayHealthDamage;
		healthModel.EShieldsDamaged += DisplayShieldsDamage;
		healthModel.EShieldsGainChanged += UpdateShieldsGain;

		UpdateHealth();
		UpdateShields();
	}

	public void Dispose()
	{
		healthModel.EHealthChanged -= UpdateHealth;
		healthModel.EShieldsChanged -= UpdateShields;
		healthModel.EHealthDamaged -= DisplayHealthDamage;
		healthModel.EShieldsDamaged -= DisplayShieldsDamage;
		healthModel.EShieldsGainChanged -= UpdateShieldsGain;
	}

	protected void UpdateHealth()
	{
		view.SetHealth(healthModel.health, healthModel.healthMax);
	}

	protected void UpdateShields()
	{
		view.SetShields(healthModel.shields, healthModel.shieldsMax);
	}

	void UpdateShieldsGain()
	{
		view.SetShieldsGain(healthModel.shieldsCurrentGain);
	}

	void DisplayShieldsDamage()
	{
		UpdateShields();
		view.PlayGotHitFX();
	}

	void DisplayHealthDamage()
	{
		UpdateHealth();
		view.PlayGotHitFX();
	}
}

