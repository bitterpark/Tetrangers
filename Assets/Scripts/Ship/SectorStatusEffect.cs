using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class SectorStatusEffect: StatusEffect
{
	protected override void SubclassActivation(object activateOnObject)
	{
		BattleManager.EEngagementModeStarted += DeactivateEffect;
	}

	//void Funknuts() { Debug.Log(); }

	protected override void SubclassDeactivation()
	{
		BattleManager.EEngagementModeStarted -= DeactivateEffect;
	}

	protected override void ExtenderActivation(object activateOnObject)
	{
		ShipSectorModel activateOnSector = activateOnObject as ShipSectorModel;
		Debug.Assert(activateOnSector != null, "Trying to activate player ship effect on non-player ship!");
		CastExtenderActivation(activateOnSector);
	}

	protected abstract void CastExtenderActivation(ShipSectorModel useOnPlayerShipModel);
}

public class EnergySiphonEffect : SectorStatusEffect
{
	EnemyShipModel activeOnShip;
	ShipSectorModel siphoningFromSector;

	protected override void InitializeValues()
	{
		name = "Energy Siphon";
		icon = SpriteDB.Instance.siphonEffectSprite;
		description = "Until next engagement: gain blue energy every time the opponent does";
		color = Color.blue;
	}

	protected override void CastExtenderActivation(ShipSectorModel activateInSector)
	{
		activeOnShip = EnemyShipModel.currentlyActive;

		siphoningFromSector = activateInSector;
		siphoningFromSector.energyManager.EBlueEnergyGained += GainEnergy;
	}

	void GainEnergy(int gain)
	{
		activeOnShip.energyManager.blueEnergy += gain;
	}

	protected override void ExtenderDeactivation()
	{
		siphoningFromSector.energyManager.EBlueEnergyGained -= GainEnergy;
		siphoningFromSector = null;
		activeOnShip = null;
	}
}

public class DisableEffect : SectorStatusEffect
{

	int mySectorIndex;

	protected override void InitializeValues()
	{
		name = "Disabled";
		icon = SpriteDB.Instance.damageSprite;
		description = "Until next engagement: segment is overloaded";
		color = Color.gray;
	}

	protected override void CastExtenderActivation(ShipSectorModel activateInSector)
	{
		mySectorIndex = activateInSector.index;
		Grid.Instance.GridSegments[mySectorIndex].isUsable = false;
	}


	protected override void ExtenderDeactivation()
	{
		Grid.Instance.GridSegments[mySectorIndex].isUsable = true;
	}
}

