using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipmentListModel
{
	List<ShipEquipment> GetStoredEquipment();
	void RemoveEquipment(params ShipEquipment[] equipment);
	void AddEquipment(params ShipEquipment[] equipment);
}

