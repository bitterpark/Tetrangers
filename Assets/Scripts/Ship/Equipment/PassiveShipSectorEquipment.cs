using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class PassiveShipSectorEquipment: PassiveShipEquipment
{
	
}

public class BlueEnergyTransmitter : PassiveShipSectorEquipment
{
	List<ShipSectorModel> adjacentSectors = new List<ShipSectorModel>();

	public override void SetOwner(object ownerObject)
	{
		base.SetOwner(ownerObject);
	}

	protected override void ActivatePassiveEffect()
	{
		//GridSegment mySegment = Grid.Instance.GridSegments[installedInSector.index];
		PlayerShipSectorModel playerSectorModel = installedInSector as PlayerShipSectorModel;
		Debug.Assert(playerSectorModel != null, "Cannot cast installedInSector as PlayerShipSectorModel!");
		int myIndex = playerSectorModel.index;

		int previousSectorIndex = Mathf.Clamp(myIndex - 1, 0, Grid.segmentCount - 1);
		int nextSectorIndex = Mathf.Clamp(myIndex + 1, 0, Grid.segmentCount - 1);

		if (previousSectorIndex != myIndex)
			adjacentSectors.Add(PlayerShipModel.main.shipSectors[previousSectorIndex]);
		if (nextSectorIndex != myIndex)
			adjacentSectors.Add(PlayerShipModel.main.shipSectors[nextSectorIndex]);

		foreach (ShipSectorModel sector in adjacentSectors)
			sector.energyManager.EBlueEnergyGained += TriggerEnergyGain;
	}

	void TriggerEnergyGain(int adjacentSectorEnergyGain)
	{
		installedInSector.energyManager.blueEnergy += adjacentSectorEnergyGain;
	}

	protected override void DeactivatePassiveEffect()
	{
		foreach (ShipSectorModel sector in adjacentSectors)
			sector.energyManager.EBlueEnergyGained -= TriggerEnergyGain;
	}

	protected override void Initialize()
	{
		name = "Blue Transmitter";
		description = "Gains blue energy whenever blue energy is gained by adjacent sectors";
	}
}

