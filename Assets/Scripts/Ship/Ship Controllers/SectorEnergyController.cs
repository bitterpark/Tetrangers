﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class SectorEnergyController
{
	protected SectorEnergyView view;
	protected SectorEnergyManager model;

	public SectorEnergyController(SectorEnergyView view, SectorEnergyManager model)
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
		view.SetBlueEnergy(model.blueEnergy, model.blueEnergyMax);
		view.SetGreenEnergy(model.greenEnergy, model.greenEnergyMax);
	}

	void UpdateEnergyGain()
	{
		view.SetEnergyGainLevels(model.blueEnergyGain, model.greenEnergyGain);
	}
}

