using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipEnergyModel : ShipResourceModel
{
	public event UnityAction EEnergyGainChanged;

	public int energyGain
	{
		get { return _energyGain; }
		set
		{
			int oldValue = _energyGain;
			_energyGain = Mathf.Max(0, value);
			if (_energyGain != oldValue && EEnergyGainChanged != null)
				EEnergyGainChanged();

		}
	}
	int _energyGain;

	public ShipEnergyModel(int energyGain, int energyMax)
		:base(energyMax)
	{
		this.energyGain = energyGain;
	}

	public override void ResetToStartingStats()
	{
		resourceCurrent = 0;
	}

	public override void DisposeModel()
	{
		EEnergyGainChanged = null;
		base.DisposeModel();
	}

	public void IncreaseByGains(int multiplier)
	{
		resourceCurrent += energyGain * multiplier;
	}

	public int GetActualDelta(int attemptedDelta, bool absolute)
	{
		if (!absolute)
			attemptedDelta *= energyGain;

		int oldValue = resourceCurrent;
		resourceCurrent += attemptedDelta;
		return resourceCurrent - oldValue;
	}

}

