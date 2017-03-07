using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipController {

	protected ShipView view;
	protected ShipModel model;

	public ShipController(ShipModel model, ShipView view)
	{
		this.view = view;
		this.model = model;

		model.ETookDamage += TakeDamage;
		model.EEnergyChanged += UpdateEnergy;
		model.ECooldownsChanged += UpdateCooldownTimes;

		List<ShipWeapon> modelWeapons = model.shipWeapons;
		Debug.Assert(view != null, "view is null!");
		ShipWeaponView[] weaponViews = view.CreateWeaponViews(modelWeapons.Count);

		for (int i = 0; i < modelWeapons.Count; i++)
		{
			ShipWeapon weapon = modelWeapons[i];
			ShipWeaponView weaponView = weaponViews[i];
			weaponView.SetDisplayValues(weapon.damage, weapon.energyCostToFire, weapon.name);
			weaponView.SetButtonInteractable(false);
			weaponView.EWeaponButtonPressed += HandleWeaponButtonPress;
			weaponView.EWeaponButtonAnimationFinished += HandleWeaponButtonAnimationFinish;
		}

		view.SetNameAndSprite(model.shipName,model.shipSprite);

		UpdateHealth();
		UpdateEnergy();
		UpdateCooldownTimes();
	}

	protected abstract void HandleWeaponButtonPress(int buttonIndex);
	protected abstract void HandleWeaponButtonAnimationFinish ();

	public virtual void DisposeController(bool disposeModel)
	{
		model.ETookDamage -= TakeDamage;
		model.EEnergyChanged -= UpdateEnergy;

		foreach (ShipWeaponView weaponView in view.GetWeaponViews())
		{
			weaponView.EWeaponButtonPressed -= HandleWeaponButtonPress;
			weaponView.EWeaponButtonAnimationFinished -= HandleWeaponButtonAnimationFinish;
		}
		view.ClearShipView();
		if (disposeModel)
			model.DisposeModel();
	}

	void TakeDamage()
	{
		UpdateHealth();
		view.PlayGotHitFX();
	}

	void UpdateHealth()
	{
		view.SetHealth(model.shipHealth,model.shipHealthMax);
	}
	void UpdateEnergy()
	{
		view.SetEnergy(model.shipEnergy,model.shipEnergyMax);
	}

	void UpdateCooldownTimes()
	{
		ShipWeaponView[] shipWeaponViews = view.GetWeaponViews();
		List<int> shipWeaponCooldowns = model.GetWeaponCooldownTimes();
		for (int i=0; i<shipWeaponViews.Length; i++)
			shipWeaponViews[i].SetCooldownTime(shipWeaponCooldowns[i]);
	}
	

}
