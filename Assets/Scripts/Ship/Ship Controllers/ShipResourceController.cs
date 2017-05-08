using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ShipResourceController
{
	protected ShipResourceView view;
	protected ShipResourceManager model;

	public ShipResourceController(ShipResourceView view, ShipResourceManager model)
	{
		this.view = view;
		this.model = model;

		model.EEnergyChanged += UpdateEnergy;
		model.EEnergyGainChanged += UpdateEnergyGain;
		model.EAmmoChanged += UpdateAmmo;
		model.EPartsChanged += UpdateParts;

		UpdateEnergy();
		UpdateEnergyGain();
		UpdateAmmo();
		UpdateParts();
	}

	public virtual void DisposeController()
	{
		model.EEnergyChanged -= UpdateEnergy;
		model.EEnergyGainChanged -= UpdateEnergyGain;

		view = null;
		model = null;

	}

	void UpdateEnergy()
	{
		view.SetShipEnergy(model.shipEnergy, model.shipEnergyMax);
	}

	void UpdateEnergyGain()
	{
		view.SetEnergyGain(model.shipEnergyGain);
	}

	void UpdateAmmo()
	{
		view.SetAmmo(model.ammo, model.ammoMax);
	}
	void UpdateParts()
	{
		view.SetParts(model.parts,model.partsMax);
	}
}

