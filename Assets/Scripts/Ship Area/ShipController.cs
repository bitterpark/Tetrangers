using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController {


	ShipView view;
	ShipModel model;

	public ShipController(ShipModel model, ShipView view)
	{
		this.view = view;
		this.model = model;

		model.ETookDamage += UpdateHealth;
		model.EEnergyChanged += UpdateEnergy;

		view.SetNameAndSprite(model.shipName,model.shipSprite);

		UpdateHealth();
		UpdateEnergy();
	}

	void UpdateHealth()
	{
		view.SetHealth(model.shipHealth,model.shipHealthMax);
	}
	void UpdateEnergy()
	{
		view.SetEnergy(model.shipEnergy);
	}

}
