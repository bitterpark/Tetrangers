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

	protected override void SubclassDeactivation()
	{
		BattleManager.EEngagementModeStarted -= DeactivateEffect;
	}

	protected override void ExtenderActivation(object activateOnObject)
	{
		PlayerShipSectorModel activateOnSector = activateOnObject as PlayerShipSectorModel;
		Debug.Assert(activateOnSector != null, "Trying to activate player ship effect on non-player ship!");
		CastExtenderActivation(activateOnSector);
	}

	protected abstract void CastExtenderActivation(PlayerShipSectorModel useOnPlayerShipModel);
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

	protected override void CastExtenderActivation(PlayerShipSectorModel activateInSector)
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

	protected override void CastExtenderActivation(PlayerShipSectorModel activateInSector)
	{
		mySectorIndex = activateInSector.index;
		Grid.Instance.GridSegments[mySectorIndex].isUsable = false;
	}


	protected override void ExtenderDeactivation()
	{
		Grid.Instance.GridSegments[mySectorIndex].isUsable = true;
	}
}

public class GreenAmplificationEffect : SectorStatusEffect
{
	ShipModel activeOnShip;
	PlayerShipSectorModel activatedOnSector;

	int blueGainDecrease;
	int greenGainIncrease;

	protected override void InitializeValues()
	{
		name = "Green Amplification Field";
		icon = SpriteDB.Instance.siphonEffectSprite;
		description = string.Format("Until next engagement: removes all blue energy gain, doubles green energy gain");//, BalanceValuesManager.Instance.bluePointsWorthPerGreenPoint);
		color = Color.green;
	}

	protected override void CastExtenderActivation(PlayerShipSectorModel activateOnSector)
	{
		activatedOnSector = activateOnSector;

		blueGainDecrease = activatedOnSector.energyManager.blueEnergyGain;
		activatedOnSector.energyManager.blueEnergyGain -= blueGainDecrease;
		greenGainIncrease = activatedOnSector.energyManager.greenEnergyGain;
		activatedOnSector.energyManager.greenEnergyGain += greenGainIncrease;

	}

	protected override void ExtenderDeactivation()
	{
		activatedOnSector.energyManager.blueEnergyGain += blueGainDecrease;
		activatedOnSector.energyManager.greenEnergyGain -= greenGainIncrease;
	}
}

public class BlueAmplificationEffect : SectorStatusEffect
{
	//ShipModel activeOnShip;
	PlayerShipSectorModel activatedOnSector;

	int blueGainIncrease;
	int greenGainDecrease;

	protected override void InitializeValues()
	{
		name = "Blue Amplification Field";
		icon = SpriteDB.Instance.siphonEffectSprite;
		description = string.Format("Until next engagement: removes all green energy gain, doubles blue energy gain");
		color = Color.cyan;
	}

	protected override void CastExtenderActivation(PlayerShipSectorModel activateOnSector)
	{
		activatedOnSector = activateOnSector;

		greenGainDecrease = activatedOnSector.energyManager.greenEnergyGain;
		blueGainIncrease = activatedOnSector.energyManager.blueEnergyGain;
		activatedOnSector.energyManager.blueEnergyGain += blueGainIncrease;
		activatedOnSector.energyManager.greenEnergyGain -= greenGainDecrease;

	}

	protected override void ExtenderDeactivation()
	{
		activatedOnSector.energyManager.blueEnergyGain -= blueGainIncrease;
		activatedOnSector.energyManager.greenEnergyGain += greenGainDecrease;
	}
}

