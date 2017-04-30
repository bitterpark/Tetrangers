using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

public class ShipEnergyManager : ICanSpendEnergy
{
	public ShipEnergyModel shipEnergyModel { get; private set; }

	public event UnityAction EEnergyChanged
	{
		add
		{
			shipEnergyModel.EResourceChanged += value;
		}
		remove
		{
			shipEnergyModel.EResourceChanged -= value;
		}
	}
	public event UnityAction<int> EEnergyGained
	{
		add { shipEnergyModel.EResourceGained += value; }
		remove { shipEnergyModel.EResourceGained -= value; }
	}

	public event UnityAction EEnergyGainChanged
	{
		add
		{
			shipEnergyModel.EEnergyGainChanged += value;
		}
		remove
		{
			shipEnergyModel.EEnergyGainChanged -= value;
		}
	}

	public int shipEnergy
	{
		get { return shipEnergyModel.resourceCurrent; }
		set { shipEnergyModel.resourceCurrent = value; }
	}
	public int shipEnergyGain
	{
		get { return shipEnergyModel.energyGain; }
		set { shipEnergyModel.energyGain = value; }
	}
	public int shipEnergyMax
	{
		get { return shipEnergyModel.resourceMax; }
		set { shipEnergyModel.resourceMax = value; }
	}

	public ShipEnergyManager(int shipEnergyGain, int shipEnergyMax)
	{
		shipEnergyModel = new ShipEnergyModel(shipEnergyGain,shipEnergyMax);
		
	}	

	public void Dispose()
	{
		shipEnergyModel.DisposeModel();
	}

	public bool EnoughEnergyToUseEquipment(ShipEquipment equipment)
	{
		if (shipEnergy >= equipment.shipEnergyCostToUse)
			return true;
		else
			return false;
	}

	public void SpendEnergyFromEquipmentUse(ShipEquipment equipment)
	{
		shipEnergy -= equipment.shipEnergyCostToUse;
	}
}

