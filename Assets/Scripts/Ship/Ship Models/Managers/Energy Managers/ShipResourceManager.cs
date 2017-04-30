using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

public class ShipResourceManager : ICanSpendEnergy
{
	public event UnityAction EEnergyChanged
	{
		add {shipEnergyModel.EResourceChanged += value;}
		remove {shipEnergyModel.EResourceChanged -= value;}
	}
	public event UnityAction<int> EShipEnergyGained
	{
		add { shipEnergyModel.EResourceGained += value; }
		remove { shipEnergyModel.EResourceGained -= value; }
	}

	public event UnityAction EEnergyGainChanged
	{
		add {shipEnergyModel.EEnergyGainChanged += value;}
		remove {shipEnergyModel.EEnergyGainChanged -= value;}
	}

	public int shipEnergy
	{
		get { return shipEnergyModel.resourceCurrent; }
		set { shipEnergyModel.resourceCurrent = value; }
	}
	public int shipEnergyMax
	{
		get { return shipEnergyModel.resourceMax; }
		set { shipEnergyModel.resourceMax = value; }
	}

	public int shipEnergyGain
	{
		get { return shipEnergyModel.energyGain; }
		set { shipEnergyModel.energyGain=value; }
	}

	public ShipEnergyModel shipEnergyModel { get; protected set; }

	public ShipResourceManager(int shipResourceGain, int shipResourceMax)
	{
		shipEnergyModel = new ShipEnergyModel(shipResourceGain, shipResourceMax);
	}

	public void Dispose()
	{
		shipEnergyModel.DisposeModel();
	}

	public bool EnoughEnergyToUseEquipment(ShipEquipment equipment)
	{
		return (shipEnergy >= equipment.shipEnergyCostToUse);
	}

	public void SpendEnergyFromEquipmentUse(ShipEquipment equipment)
	{
		shipEnergy -= equipment.blueEnergyCostToUse;
	}

	public void IncreaseShipEnergyByGain()
	{
		IncreaseShipEnergyByGains(1);
	}

	public void IncreaseShipEnergyByGains(int multiplier)
	{
		shipEnergyModel.IncreaseByGains(multiplier);
	}

	public int GetActualShipEnergyIncrease()
	{
		return GetActualShipEnergyDelta(1, false);
	}

	public int GetActualShipEnergyDelta(int attemptedDelta)
	{
		return GetActualShipEnergyDelta(attemptedDelta, true);
	}

	public int GetActualShipEnergyDelta(int attemptedDelta, bool absolute)
	{
		return shipEnergyModel.GetActualDelta(attemptedDelta, absolute);
	}
}

