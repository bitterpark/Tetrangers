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

		UpdateEnergy();
		UpdateEnergyGain();
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
}

