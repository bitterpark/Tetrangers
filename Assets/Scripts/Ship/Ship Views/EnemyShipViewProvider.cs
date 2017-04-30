using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyShipViewProvider : MonoBehaviour, IShipViewProvider
{
	public SectorView sectorView;

	public ShipView shipView { get { return providedShipView; } }
	[SerializeField]
	ShipView providedShipView;

	public SectorEnergyView energyView;
	public HealthView healthView;


}