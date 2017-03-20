using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangarModel: IEquipmentListModel
{

	List<ShipEquipment> storedEquipment = new List<ShipEquipment>(); 
	
	public HangarModel()
	{
		AddStoredEquipment(new LaserGun(),new LaserGun(),new Siphon());
	}

	public void AddStoredEquipment(params ShipEquipment[] addedEquipment)
	{
		storedEquipment.AddRange(addedEquipment);
	}

	public void TakeOutStoredEquipment(params ShipEquipment[] removedEquipment)
	{
		foreach (ShipEquipment equipment in removedEquipment)
			storedEquipment.Remove(equipment);
	}

	public List<ShipEquipment> GetStoredEquipment()
	{
		return storedEquipment;
	}
}
