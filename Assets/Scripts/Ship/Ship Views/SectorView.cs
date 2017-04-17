using UnityEngine;
using System.Collections;

public class SectorView : MonoBehaviour
{
	public SectorEquipmentListView equipmentListView;
	public ShipEnergyView energyView;	

	public void ClearView()
	{
		equipmentListView.ClearView();
	}
}
