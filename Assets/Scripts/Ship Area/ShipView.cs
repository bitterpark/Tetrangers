using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using StatusEffects;


public class ShipView : EquipmentListView {


	[SerializeField]
	Text shipName;
	[SerializeField]
	Image shipImage;
	
	[SerializeField]
	Transform healthBarObject;
	Text healthText;
	RectTransform healthBar;

	[SerializeField]
	Transform blueEnergyBarObject;
	Text blueEnergyText;
	RectTransform blueEnergyBar;

	[SerializeField]
	Transform greenEnergyBarObject;
	Text greenEnergyText;
	RectTransform greenEnergyBar;

	[SerializeField]
	StatusEffectView statusEffectsView;

	

	public void SetNameAndSprite(string name, Sprite sprite)
	{
		shipName.text = name;
		shipImage.sprite = sprite;
	}

	public void SetHealth(int newHealth, int maxHealth)
	{
		healthText.text = newHealth.ToString() + "/" + maxHealth.ToString();

		float barPercentage;
		if (maxHealth > 0)
			barPercentage = (float)newHealth / (float)maxHealth;
		else
			barPercentage = 1;

		healthBar.anchorMax = new Vector2(barPercentage, healthBar.anchorMax.y);
	}

	public void SetBlueEnergy(int newEnergy, int maxEnergy)
	{
		blueEnergyText.text = newEnergy.ToString() + "/" + maxEnergy.ToString();
		float barPercentage;
		if (maxEnergy > 0)
			barPercentage = (float)newEnergy / (float)maxEnergy;
		else
			barPercentage = 1;
		blueEnergyBar.anchorMax = new Vector2(barPercentage, blueEnergyBar.anchorMax.y);
	}

	public void SetGreenEnergy(int newEnergy, int maxEnergy)
	{
		greenEnergyText.text = newEnergy.ToString() + "/" + maxEnergy.ToString();
		
		float barPercentage;
		if (maxEnergy > 0)
			barPercentage = (float)newEnergy / (float)maxEnergy;
		else
			barPercentage = 1;

		greenEnergyBar.anchorMax = new Vector2(barPercentage, greenEnergyBar.anchorMax.y);
	}

	public void PlayGotHitFX()
	{
		ParticleController gotHitParticles = Instantiate(ParticleDB.Instance.shipGotHitParticles);
		Vector3 particlesPosition = shipImage.transform.position;
		particlesPosition.z = -2;
		gotHitParticles.transform.position = particlesPosition;
		gotHitParticles.EnableParticleSystemOnce();
	}

	public void ShowStatusEffect(IDisplayableStatusEffect effect)
	{
		statusEffectsView.AddStatusEffectIcon(effect);
	}

	protected override void Initialize()
	{
		base.Initialize();
		healthText = healthBarObject.transform.FindChild("Value").GetComponent<Text>();
		healthBar = healthBarObject.transform.FindChild("Underbar").FindChild("Bar").GetComponent<RectTransform>();

		blueEnergyText = blueEnergyBarObject.transform.FindChild("Value").GetComponent<Text>();
		blueEnergyBar = blueEnergyBarObject.transform.FindChild("Underbar").FindChild("Bar").GetComponent<RectTransform>();

		greenEnergyText = greenEnergyBarObject.transform.FindChild("Value").GetComponent<Text>();
		greenEnergyBar = greenEnergyBarObject.transform.FindChild("Underbar").FindChild("Bar").GetComponent<RectTransform>();

		
	}
}
