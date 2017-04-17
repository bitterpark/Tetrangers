using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using StatusEffects;

[RequireComponent(typeof(StatusEffectDisplayer))]
public class ShipView : MonoBehaviour, ICanShowStatusEffects, IShipViewProvider {

	public StatusEffectDisplayer statusEffectDisplayer
	{
		get { return GetComponent<StatusEffectDisplayer>(); }
	}

	public ShipView shipView { get { return this; } }

	public TabbedEquipmentListView equipmentListView;
	public ShipEnergyView energyView;

	[SerializeField]
	Text shipName;
	[SerializeField]
	Image shipImage;
	
	[SerializeField]
	Transform healthBarObject;
	Text healthText;
	RectTransform healthBar;

	[SerializeField]
	Text generatorLevelText;

	[SerializeField]
	Transform shieldBarObject;
	Text shieldText;
	RectTransform shieldBar;
	[SerializeField]
	Text shieldsGainText;
	/*
	[SerializeField]
	Transform blueEnergyBarObject;
	Text blueEnergyText;
	RectTransform blueEnergyBar;
	[SerializeField]
	Text blueEnergyGainText;

	[SerializeField]
	Transform greenEnergyBarObject;
	Text greenEnergyText;
	RectTransform greenEnergyBar;
	[SerializeField]
	Text greenEnergyGainText;
	*/
	

	void Awake()
	{
		Initialize();
	}

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

	public void SetShields(int newShields, int maxShields)
	{
		shieldText.text = newShields.ToString() + "/" + maxShields.ToString();

		float barPercentage;
		if (maxShields > 0)
			barPercentage = (float)newShields / (float)maxShields;
		else
			barPercentage = 1;

		shieldBar.anchorMax = new Vector2(barPercentage, shieldBar.anchorMax.y);
	}

	public void SetShieldsGain(int gain)
	{
		shieldsGainText.text = gain.ToString();
	}
	/*
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
	*/
	public void SetGenLevel(int level)
	{
		generatorLevelText.text = "x"+level;
	}
	/*
	public void SetEnergyGainLevels(int blueEnergyGain, int greenEnergyGain)
	{
		SetBlueEnergyGain(blueEnergyGain);
		SetGreenEnergyGain(greenEnergyGain);
	}

	void SetBlueEnergyGain(int gain)
	{
		blueEnergyGainText.text = gain.ToString();
	}
	void SetGreenEnergyGain(int gain)
	{
		greenEnergyGainText.text = gain.ToString();
	}
	*/
	public void PlayGotHitFX()
	{
		ParticleDB.Instance.CreateShipGotHitParticles(shipImage.transform.position);
	}

	protected void Initialize()
	{
		healthText = healthBarObject.transform.FindChild("Value").GetComponent<Text>();
		healthBar = healthBarObject.transform.FindChild("Underbar").FindChild("Bar").GetComponent<RectTransform>();

		shieldText = shieldBarObject.transform.FindChild("Value").GetComponent<Text>();
		shieldBar = shieldBarObject.transform.FindChild("Underbar").FindChild("Bar").GetComponent<RectTransform>();

		//blueEnergyText = blueEnergyBarObject.transform.FindChild("Value").GetComponent<Text>();
		//blueEnergyBar = blueEnergyBarObject.transform.FindChild("Underbar").FindChild("Bar").GetComponent<RectTransform>();

		//greenEnergyText = greenEnergyBarObject.transform.FindChild("Value").GetComponent<Text>();
		//greenEnergyBar = greenEnergyBarObject.transform.FindChild("Underbar").FindChild("Bar").GetComponent<RectTransform>();
	}
}
