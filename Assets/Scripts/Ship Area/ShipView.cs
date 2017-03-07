using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipView : MonoBehaviour {

	[SerializeField]
	Text shipName;
	[SerializeField]
	Image shipImage;

	[SerializeField]
	RectTransform healthBar;
	[SerializeField]
	Text healthText;
	[SerializeField]
	RectTransform energyBar;
	[SerializeField]
	Text energyText;

	[SerializeField]
	Transform weaponsArea;

	[SerializeField]
	ShipWeaponView weaponViewPrefab;

	public void SetNameAndSprite(string name, Sprite sprite)
	{
		shipName.text = name;
		shipImage.sprite = sprite;
	}

	public void SetHealth(int newHealth, int maxHealth)
	{
		healthText.text = newHealth.ToString() + "/" + maxHealth.ToString();
		healthBar.anchorMax = new Vector2((float)newHealth / (float)maxHealth, healthBar.anchorMax.y);
	}

	public void SetEnergy(int newEnergy, int maxEnergy)
	{
		energyText.text = newEnergy.ToString() + "/" + maxEnergy.ToString();
		energyBar.anchorMax = new Vector2((float)newEnergy / (float)maxEnergy, energyBar.anchorMax.y);
	}

	public void PlayGotHitFX()
	{
		ParticleController gotHitParticles = Instantiate(ParticleDB.Instance.shipGotHitParticles);
		Vector3 particlesPosition = shipImage.transform.position;
		particlesPosition.z = -2;
		gotHitParticles.transform.position = particlesPosition;
		gotHitParticles.EnableParticleSystemOnce();
	}

	public void ClearShipView()
	{
		ClearWeaponViews();
	}

	public ShipWeaponView[] CreateWeaponViews(int count)
	{
		for (int i = 0; i < count; i++)
		{
			CreateWeaponView().Initialize(i);
		}
		return GetWeaponViews();
	}

	ShipWeaponView CreateWeaponView()
	{
		ShipWeaponView newView = Instantiate(weaponViewPrefab);
		newView.transform.SetParent(weaponsArea, false);
		return newView;
	}

	public void ClearWeaponViews()
	{
		foreach (ShipWeaponView view in weaponsArea.GetComponentsInChildren<ShipWeaponView>())
			GameObject.Destroy(view.gameObject);
	}

	public ShipWeaponView[] GetWeaponViews()
	{
		return weaponsArea.GetComponentsInChildren<ShipWeaponView>();
	}
}
