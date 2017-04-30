using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SectorView : MonoBehaviour
{
	public EquipmentListView equipmentListView;
	public SectorEnergyView energyView;
	public StatusEffectDisplayer statusEffectDisplayer;
	public HealthView healthView;
	public int sectorIndex;

	public void ClearView()
	{
		if (equipmentListView!=null)
			equipmentListView.ClearView();
	}

}
