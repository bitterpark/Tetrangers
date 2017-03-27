using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentTypes {Weapon,Equipment,Skill}

public abstract class ShipEquipment
{
	public delegate void EquipmentCooldownDeleg(ShipEquipment equipment, int cooldown);
	public static event EquipmentCooldownDeleg EEquipmentCooldownChanged;

	public int blueEnergyCostToUse { get; set; }
	public int greenEnergyCostToUse { get; set; }
	public int generatorLevelDelta { get; set;}
	public string name { get; set; }

	public EquipmentTypes equipmentType { get; protected set; }

	public bool hasDescription { get { return _description != null; } }
	public string description { get { return _description; } protected set { _description = value; } }
	string _description = null;

	public int maxCooldownTime { get; protected set; }
	public int cooldownTimeRemaining 
	{
		get { return _cooldownTimeRemaining; }
		set 
		{ 
			_cooldownTimeRemaining = Mathf.Max(value, 0); 
			if (EEquipmentCooldownChanged != null) 
				EEquipmentCooldownChanged(this, cooldownTimeRemaining);
		} 
	}
	int _cooldownTimeRemaining = 0;

	public bool isUseable { get; protected set; }

	public ShipEquipment()
	{
		isUseable = true;
		blueEnergyCostToUse = 0;
		greenEnergyCostToUse = 0;
		generatorLevelDelta = 0;
		equipmentType = EquipmentTypes.Equipment;
		Initialize();
	}

	protected abstract void Initialize();

	public virtual void ActivateEquipment(ShipModel activatedOnShip)
	{
		SetCooldown();
		if (generatorLevelDelta!=0)
			TetrisManager.Instance.ChangeGeneratorLevel(generatorLevelDelta);
		ExtenderActivation(activatedOnShip);
	}

	protected void SetCooldown()
	{
		cooldownTimeRemaining = maxCooldownTime;
	}
	protected virtual void ExtenderActivation(ShipModel activateOnShip) { }


	public void ResetCooldown()
	{
		cooldownTimeRemaining = 0;
	}
}

public class Forcefield : ShipEquipment
{
	int healthGain;

	protected override void Initialize()
	{
		maxCooldownTime = 2;
		blueEnergyCostToUse = 0;
		greenEnergyCostToUse = 50;
		healthGain = greenEnergyCostToUse * 2;
		name = "Forcefield";
		description = string.Format("Restore {0} health", healthGain);
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		activateOnShip.GainHealth(healthGain);
	}
}

public class Interface : ShipEquipment
{
	int blueGain;

	protected override void Initialize()
	{
		maxCooldownTime = 4;
		//blueEnergyCostToUse = 200;
		greenEnergyCostToUse = 100;
		blueGain = greenEnergyCostToUse * 2;
		name = "Interface";
		description = string.Format("Gain {0} blue energy", blueGain);
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		activateOnShip.GainBlueEnergy(blueGain, true);
	}
}

public class BlockEjector :ShipEquipment
{
	const int numberOfRowsCleared = 2;

	protected override void Initialize()
	{
		maxCooldownTime = 1;
		blueEnergyCostToUse = 50;
		name = "Eject Blocks";
		description = string.Format("Clears {0} rows from the bottom", numberOfRowsCleared);
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		base.ExtenderActivation(activateOnShip);
		Grid.Instance.ClearBottomRows(numberOfRowsCleared);
	}
}
public class BlockEjectorTopic: ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 100;
		materialsRequired = 200;

		providesEquipment = new BlockEjector();
	}
}

public class CoolantInjector : ShipEquipment
{
	protected override void Initialize()
	{
		maxCooldownTime = 2;
		generatorLevelDelta = -1;
		blueEnergyCostToUse = 50;
		name = "Coolant Injector";
	}
}
public class CoolantInjectorTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 100;
		materialsRequired = 200;

		providesEquipment = new CoolantInjector();
	}
}

public class Afterburner : ShipEquipment
{
	const int lowerByMoves = 5;

	protected override void Initialize()
	{
		maxCooldownTime = 2;
		blueEnergyCostToUse = 50;
		name = "Afterburner";
		description = string.Format("Catch up to the enemy ship faster, lowering moves until next engagement by {0}", lowerByMoves);
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		base.ExtenderActivation(activateOnShip);
		BattleManager.Instance.ChangeTurnsForNextEngagement(-lowerByMoves);
	}
}
public class AfterburnerTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 100;
		materialsRequired = 200;

		providesEquipment = new Afterburner();
	}
}

public class ManeuveringJets : ShipEquipment
{
	const int higherByMoves = 5;

	protected override void Initialize()
	{
		maxCooldownTime = 2;
		greenEnergyCostToUse = 25;
		name = "Maneuvering Jets";
		description = string.Format("Avoid the enemy ship longer, increasing moves until next engagement by {0}", higherByMoves);
	}

	protected override void ExtenderActivation(ShipModel activateOnShip)
	{
		base.ExtenderActivation(activateOnShip);
		BattleManager.Instance.ChangeTurnsForNextEngagement(higherByMoves);
	}
}
public class ManeuveringJetsTopic:ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 100;
		materialsRequired = 200;

		providesEquipment = new ManeuveringJets();
	}
}



