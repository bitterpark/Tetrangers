using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthView: MonoBehaviour
{
	[SerializeField]
	HorizontalBarView healthBar;
	[SerializeField]
	HorizontalEnergyBarView shieldsBar;

	public void SetHealth(int newHealth, int maxHealth)
	{
		healthBar.SetBarValue(newHealth, maxHealth);
	}

	public void SetShields(int newShields, int maxShields)
	{
		shieldsBar.SetBarValue(newShields, maxShields);
	}

	public void SetShieldsGain(int gain)
	{
		shieldsBar.SetGain(gain);
	}

	public void PlayGotHitFX()
	{
		ParticleDB.Instance.CreateShipGotHitParticles(healthBar.GetComponent<RectTransform>().position);//healthBarObject.position);
	}
}

