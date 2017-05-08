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


	public event UnityAction EAmmoChanged
	{
		add { ammoModel.EResourceChanged += value; }
		remove { ammoModel.EResourceChanged -= value; }
	}
	public int ammo
	{
		get { return ammoModel.resourceCurrent; }
		set { ammoModel.resourceCurrent = value; }
	}
	public int ammoMax
	{
		get { return ammoModel.resourceMax; }
		set { ammoModel.resourceMax = value; }
	}

	public event UnityAction EPartsChanged
	{
		add { partsModel.EResourceChanged += value; }
		remove { partsModel.EResourceChanged -= value; }
	}
	public int parts
	{
		get { return partsModel.resourceCurrent; }
		set { partsModel.resourceCurrent = value; }
	}
	public int partsMax
	{
		get { return partsModel.resourceMax; }
		set { partsModel.resourceMax = value; }
	}

	public EnergyResourceModel shipEnergyModel { get; protected set; }
	public FiniteResourceModel ammoModel { get; protected set; }
	public FiniteResourceModel partsModel { get; protected set; }
	
	public ShipResourceManager(int shipResourceGain, int shipResourceMax, int shipAmmoMax, int shipPartsMax)
	{
		shipEnergyModel = new EnergyResourceModel(shipResourceGain, shipResourceMax);
		ammoModel = new FiniteResourceModel(shipAmmoMax);
		partsModel = new FiniteResourceModel(shipPartsMax);
	}

	public void Dispose()
	{
		shipEnergyModel.DisposeModel();
		ammoModel.DisposeModel();
		partsModel.DisposeModel();
	}

	public bool EnoughEnergyToUseEquipment(ShipEquipment equipment)
	{
		return (shipEnergy >= equipment.shipEnergyCostToUse && ammo >=equipment.ammoCostToUse && parts >= equipment.partsCostToUse);
	}

	public void SpendEnergyFromEquipmentUse(ShipEquipment equipment)
	{
		shipEnergy -= equipment.blueEnergyCostToUse;
		ammo -= equipment.ammoCostToUse;
		parts -= equipment.partsCostToUse;
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

