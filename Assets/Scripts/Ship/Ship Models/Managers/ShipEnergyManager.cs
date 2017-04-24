using System;
using System.Collections.Generic;
using UnityEngine.Events;

public interface IHasEnergy
{
	ShipEnergyManager energyManager { get;}
}

public class ShipEnergyManager
{
	public event UnityAction EEnergyChanged
	{
		add
		{
			blueEnergyModel.EResourceChanged += value;
			greenEnergyModel.EResourceChanged += value;
		}
		remove
		{
			blueEnergyModel.EResourceChanged -= value;
			greenEnergyModel.EResourceChanged -= value;
		}
	}
	public event UnityAction<int> EBlueEnergyGained
	{
		add { blueEnergyModel.EResourceGained += value; }
		remove { blueEnergyModel.EResourceGained -= value; }
	}
	public event UnityAction<int> EGreenEnergyGained
	{
		add { greenEnergyModel.EResourceGained += value; }
		remove { greenEnergyModel.EResourceGained -= value; }
	}

	public event UnityAction EEnergyGainChanged
	{
		add
		{
			blueEnergyModel.EEnergyGainChanged += value;
			greenEnergyModel.EEnergyGainChanged += value;
		}
		remove
		{
			blueEnergyModel.EEnergyGainChanged -= value;
			greenEnergyModel.EEnergyGainChanged -= value;
		}
	}

	public int blueEnergy
	{
		get { return blueEnergyModel.resourceCurrent; }
		set { blueEnergyModel.resourceCurrent = value; }
	}
	public int blueEnergyMax
	{
		get { return blueEnergyModel.resourceMax; }
		set { blueEnergyModel.resourceMax = value; }
	}
	public int greenEnergy
	{
		get { return greenEnergyModel.resourceCurrent; }
		set { greenEnergyModel.resourceCurrent = value; }
	}
	public int greenEnergyMax
	{
		get { return greenEnergyModel.resourceMax; }
		set { greenEnergyModel.resourceMax = value;}
	}

	public int blueEnergyGain
	{
		get { return GetBlueGainPropertyCalled(); }
		set { SetBlueGainPropertyCalled(value); }
	}
	public int greenEnergyGain
	{
		get { return GetGreenGainPropertyCalled(); }
		set { SetGreenGainPropertyCalled(value); }
	}

	public ShipEnergyModel blueEnergyModel { get; protected set; }
	public ShipEnergyModel greenEnergyModel { get; protected set; }

	public ShipEnergyManager(int blueGain, int blueMax, int greenGain, int greenMax)
	{
		blueEnergyModel = new ShipEnergyModel(blueGain, blueMax);
		greenEnergyModel = new ShipEnergyModel(greenGain, greenMax);
	}

	public void ResetToStartingStats()
	{
		blueEnergyModel.ResetToStartingStats();
		greenEnergyModel.ResetToStartingStats();
	}

	public void Dispose()
	{
		blueEnergyModel.DisposeModel();
		greenEnergyModel.DisposeModel();
	}

	public void IncreaseBlueByGain()
	{
		IncreaseBlueByGains(1);
	}

	public void IncreaseBlueByGains(int multiplier)
	{
		blueEnergyModel.IncreaseByGains(multiplier);
	}

	public int GetActualBlueIncrease()
	{
		return GetActualBlueDelta(1, false);
	}

	public int GetActualBlueDelta(int attemptedDelta)
	{
		return GetActualBlueDelta(attemptedDelta, true);
	}

	public int GetActualBlueDelta(int attemptedDelta, bool absolute)
	{
		return blueEnergyModel.GetActualDelta(attemptedDelta, absolute);
	}

	public void IncreaseGreenByGain()
	{
		IncreaseGreenByGains(1);
	}

	public void IncreaseGreenByGains(int multiplier)
	{
		greenEnergyModel.IncreaseByGains(multiplier);
	}

	public int GetActualGreenIncrease()
	{
		return GetActualGreenDelta(1, false);
	}

	public int GetActualGreenDelta(int attemptedDelta)
	{
		return GetActualGreenDelta(attemptedDelta, true);
	}

	public int GetActualGreenDelta(int attemptedDelta, bool absolute)
	{
		return greenEnergyModel.GetActualDelta(attemptedDelta, absolute);
	}

	protected virtual int GetBlueGainPropertyCalled()
	{
		return blueEnergyModel.energyGain;
	}
	protected virtual int GetGreenGainPropertyCalled()
	{
		return greenEnergyModel.energyGain;
	}
	protected virtual void SetBlueGainPropertyCalled(int value)
	{
		blueEnergyModel.energyGain = value;
	}
	protected virtual void SetGreenGainPropertyCalled(int value)
	{
		greenEnergyModel.energyGain = value;
	}

}

