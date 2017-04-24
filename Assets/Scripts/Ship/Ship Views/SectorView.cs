using UnityEngine;
using System.Collections;

public class SectorView : MonoBehaviour
{
	public SectorEquipmentListView equipmentListView;
	public ShipEnergyView energyView;
	public StatusEffectDisplayer statusEffectDisplayer;

	public void ClearView()
	{
		equipmentListView.ClearView();
	}
}
