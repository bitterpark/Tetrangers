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
	//public ShipEnergyView energyView;
	//public HealthView healthView;

	[SerializeField]
	Text shipName;
	[SerializeField]
	Image shipImage;
	

	[SerializeField]
	Text generatorLevelText;
	

	void Awake()
	{
		Initialize();
	}

	public void SetNameAndSprite(string name, Sprite sprite)
	{
		shipName.text = name;
		shipImage.sprite = sprite;
	}


	public void SetGenLevel(int level)
	{
		//generatorLevelText.text = "x"+level;
	}

	public void PlayGotHitFX()
	{
		ParticleDB.Instance.CreateShipGotHitParticles(shipImage.transform.position);
	}

	protected void Initialize()
	{

	}
}
