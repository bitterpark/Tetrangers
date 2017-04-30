using System;
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
	public int shipEnergyCostToUse { get; set; }
	public int generatorLevelDelta { get; set;}
	public string name { get; set; }

	public EquipmentTypes equipmentType { get; protected set; }
	public Goal equipmentGoal { get; protected set; }

	public bool hasDescription { get { return _description != null || effectOnSelfDescription != null || effectOnSectorDescription != null; } }
	public string description
	{	get
		{
			string fullDesc="";
			if (_description != null)
				fullDesc += _description;
			if (effectOnSelfDescription!=null)
			{
				if (_description != null)
					fullDesc += "\n";
				fullDesc += effectOnSelfDescription;
			}
			if (effectOnSectorDescription != null)
			{
				if (_description != null || effectOnSelfDescription != null)
					fullDesc += "\n";
				fullDesc += effectOnSectorDescription;
			}
			if (effectOnOpponentDescription!=null)
			{
				if (_description != null || effectOnSelfDescription != null || effectOnSectorDescription!=null)
					fullDesc += "\n";
				fullDesc += effectOnOpponentDescription;
			}

			return fullDesc;
		}
		protected set { _description = value+"\n"; }
	}
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

	//public bool isUseable { get; protected set; }

	public StatusEffect onSelfEffect
	{
		get { return _onSelfEffect; }
		protected set
		{
			_onSelfEffect = value;
			if (value != null)
				effectOnSelfDescription = "Self:" + _onSelfEffect.description;
		}
	}
	StatusEffect _onSelfEffect;

	public StatusEffect onSectorEffect
	{
		get { return _onSectorEffect; }
		protected set
		{
			_onSectorEffect = value;
			if (value != null)
				effectOnSectorDescription = "Sector:" + _onSectorEffect.description;
		}
	}
	StatusEffect _onSectorEffect;

	public StatusEffect onOpponentEffect
	{
		get { return _onOpponentEffect; }
		protected set
		{
			_onOpponentEffect = value;
			if (value != null)
				effectOnOpponentDescription = "Opponent:"+ _onOpponentEffect.description;
		}
	}
	StatusEffect _onOpponentEffect;

	string effectOnSelfDescription = null;
	string effectOnSectorDescription = null;
	string effectOnOpponentDescription = null;

	protected ShipModel installedOnShip;
	protected ShipSectorModel installedInSector;

	public ShipEquipment()
	{
		//isUseable = true;
		blueEnergyCostToUse = 0;
		greenEnergyCostToUse = 0;
		shipEnergyCostToUse = 0;
		generatorLevelDelta = 0;
		equipmentType = EquipmentTypes.Equipment;
		equipmentGoal = Goal.Defence;
		Initialize();
	}

	protected abstract void Initialize();

	public virtual bool IsUsableByShip(ShipModel ship)
	{
		if (cooldownTimeRemaining == 0)
			return true;
		else
			return false;
	}

	public virtual void SetOwner(object ownerObject)
	{
		installedOnShip = ownerObject as ShipModel;
		if (installedOnShip == null)
		{
			installedInSector = ownerObject as ShipSectorModel;
			Debug.Assert(installedInSector != null, "Cannot cast passed ownerObject as ShipSectorModel!");
			if (installedInSector.GetType() == typeof(EnemyShipSectorModel))
				installedOnShip = EnemyShipModel.currentlyActive;
			else
				installedOnShip = PlayerShipModel.main;
			//installedOnShip = installedInSector.pa;
		}
		
	}

	public virtual void ActivateEquipment()
	{
		if (generatorLevelDelta != 0)
			installedOnShip.ChangeGeneratorLevel(generatorLevelDelta);
			//TetrisManager.Instance.ChangeGeneratorLevel(generatorLevelDelta);
		ExtenderActivation();
		SetCooldown();
	}

	protected void SetCooldown()
	{
		cooldownTimeRemaining = maxCooldownTime;
	}
	protected virtual void ExtenderActivation() { }


	public virtual void ResetEquipment()
	{
		cooldownTimeRemaining = 0;
	}

	public virtual void Dispose() { }

}

public class Forcefield : ShipEquipment
{
	int healthGain;

	protected override void Initialize()
	{
		maxCooldownTime = 3;
		//blueEnergyCostToUse = 0;
		greenEnergyCostToUse = 60;
		healthGain = greenEnergyCostToUse * 2;
		name = "Forcefield";
		description = string.Format("Restore {0} shields", healthGain);
	}

	protected override void ExtenderActivation()
	{
		//installedOnShip.healthManager.shields += healthGain;
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

	protected override void ExtenderActivation()
	{
		//installedOnShip.energyUser.blueEnergy += blueGain;
	}
}

public class BlockEjector :ShipEquipment
{
	const int numberOfRowsCleared = 2;

	protected override void Initialize()
	{
		maxCooldownTime = 4;
		greenEnergyCostToUse = 60;
		name = "Eject Blocks";
		description = string.Format("Clears {0} rows from the bottom", numberOfRowsCleared);
	}

	protected override void ExtenderActivation()
	{
		base.ExtenderActivation();
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
		blueEnergyCostToUse = 500;
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
		blueEnergyCostToUse = 500;
		name = "Afterburner";
		description = string.Format("Catch up to the enemy ship faster, lowering moves until next engagement by {0}", lowerByMoves);
	}

	protected override void ExtenderActivation()
	{
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
	const int higherByMoves = 4;

	protected override void Initialize()
	{
		maxCooldownTime = 3;
		greenEnergyCostToUse = 30;
		name = "Maneuvering Jets";
		description = string.Format("Avoid the enemy ship longer, increasing moves until next engagement by {0}", higherByMoves);
	}

	protected override void ExtenderActivation()
	{
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

public class ReactiveArmor : ShipEquipment
{
	protected override void Initialize()
	{
		maxCooldownTime = 2;
		greenEnergyCostToUse = 30;

		name = "Reactive Armor";
		onSelfEffect = new ReactiveArmorEffect();
	}
}
public class ReactiveArmorTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 100;
		materialsRequired = 200;

		providesEquipment = new ReactiveArmor();
	}
}

public class MeltdownTrigger : ShipEquipment
{
	protected override void Initialize()
	{
		maxCooldownTime = 2;
		generatorLevelDelta = 1;
		name = "Meltdown Trigger";
		onSelfEffect = new MeltdownEffect();
	}
}
public class MeltdownTriggerTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 100;
		materialsRequired = 200;

		providesEquipment = new MeltdownTrigger();
	}
}


public class GreenAmp : Ability
{
	protected override void Initialize()
	{
		maxCooldownTime = 2;
		blueEnergyCostToUse = 40;
		//blueEnergyCostToUse = 50;
		//greenEnergyCostToUse = 50;

		name = "Green Amp";
		onSelfEffect = new GreenAmplificationEffect();
		onOpponentEffect = new GreenAmplificationEffect();
	}
}
public class GreenAmpTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 100;
		materialsRequired = 200;

		providesEquipment = new GreenAmp();
	}
}

public class BlueAmp : Ability
{
	protected override void Initialize()
	{
		maxCooldownTime = 2;
		greenEnergyCostToUse = 20;
		//greenEnergyCostToUse = 25;
		//greenEnergyCostToUse = 50;

		name = "Blue Amp";
		onSelfEffect = new BlueAmplificationEffect();
		onOpponentEffect = new BlueAmplificationEffect();
	}
}
public class BlueAmpTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 100;
		materialsRequired = 200;

		providesEquipment = new BlueAmp();
	}
}

public class Stabilizer : ShipEquipment
{
	const int lowerByRows = 2;

	protected override void Initialize()
	{
		maxCooldownTime = 5;
		shipEnergyCostToUse = 00;//500;
		name = "Stabilizer";
		description = string.Format("Lowers the grid by {0} rows", lowerByRows);
	}

	protected override void ExtenderActivation()
	{
		Grid.Instance.ClearBottomRows(lowerByRows);
	}
}

public class Heatsink : ShipEquipment
{
	const int lowerCooldownsBy = 1;

	protected override void Initialize()
	{
		maxCooldownTime = 4;
		shipEnergyCostToUse = 10;//300;
		name = "Heatsink";
		description = string.Format("Lowers all cooldowns by {0}", lowerCooldownsBy);
	}

	protected override void ExtenderActivation()
	{
		installedOnShip.shipEquipment.LowerAllCooldowns(lowerCooldownsBy);

		foreach (PlayerShipSectorModel sector in PlayerShipModel.main.shipSectors)
			sector.sectorEquipment.LowerAllCooldowns(lowerCooldownsBy);
	}
}

public class TestEquipment : ShipEquipment, IRequiresPlayerTargetSelect
{

	int targetedShipSectorIndex;

	protected override void Initialize()
	{
		maxCooldownTime = 3;
		name = "TestEquipment";
		description = string.Format("Clear all blocks in a selected ship sector");
	}

	protected override void ExtenderActivation()
	{
		GridSegment targetedSegment = Grid.Instance.GridSegments[targetedShipSectorIndex];
		Grid.Instance.ClearArea(targetedSegment.minX,targetedSegment.minY,targetedSegment.maxX,targetedSegment.maxY);
	}

	public void CallTargetSelectManager()
	{
		PlayerTargetSelectManager.Instance.InitiateSelectingPlayerShipSector(this);
	}

	public void SetTarget(object selectedTarget)
	{
		SectorView targetedView = selectedTarget as SectorView;
		Debug.Assert(targetedView != null, "Could not find targeted view!");
		targetedShipSectorIndex = targetedView.sectorIndex;
	}
}

//ENEMIES ONLY

public class Siphon : ShipEquipment
{
	protected override void Initialize()
	{
		maxCooldownTime = 2;
		//blueEnergyCostToUse = 500;
		greenEnergyCostToUse = 20;

		name = "Siphon";
		onSectorEffect = new DisableEffect();//EnergySiphonEffect();
	}
}




