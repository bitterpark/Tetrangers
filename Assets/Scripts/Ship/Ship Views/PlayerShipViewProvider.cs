using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IShipViewProvider
{
	ShipView shipView { get; }
}

public class PlayerShipViewProvider : MonoBehaviour, IShipViewProvider
{
	public List<SectorView> sectorViews;

	public ShipView shipView { get { return providedShipView; } }
	[SerializeField]
	ShipView providedShipView;

	public ShipResourceView resourceView;

}
