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
	public int ammoCostToUse { get; set; }
	public int partsCostToUse { get; set; }
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
		protected set { _description = value + "\n"; }
	}
	string _description = null;

	public int maxCooldownTime
	{
		get { return _adjustedMaxCooldownTime; }
		protected set { _adjustedMaxCooldownTime = value * 5; }
	}
	int _adjustedMaxCooldownTime = 0;
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

	public bool equipmentIsActive
	{
		get { return _isActive; }
		set
		{
			if (_isActive!=value)
				UpdateActiveStatus(value);
			_isActive = value;
		}
	}
	bool _isActive = true;

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

	protected UnityEngine.Events.UnityAction activationSoundAction;

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
		ammoCostToUse = 0;
		partsCostToUse = 0;
		generatorLevelDelta = 0;
		equipmentType = EquipmentTypes.Equipment;
		equipmentGoal = Goal.Defence;
		activationSoundAction = SoundFXPlayer.Instance.PlayEquipmentActivationSound;
		Initialize();
	}

	protected abstract void Initialize();

	protected virtual void UpdateActiveStatus(bool active)
	{

	}

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

	public void PlayActivationSound()
	{
		activationSoundAction.Invoke();
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
//Currently defunct
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
		//Grid.Instance.EmptyBottomRowsInSegment(numberOfRowsCleared);
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
//Defunct
/*
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
}*/


public class GreenAmp : ShipEquipment, IRequiresPlayerTargetSelect
{
	int targetSectorIndex;

	SectorStatusEffect effectApppliedToSector = new GreenAmplificationEffect();

	protected override void Initialize()
	{
		maxCooldownTime = 2;
		greenEnergyCostToUse = 20;
		//greenEnergyCostToUse = 25;
		//greenEnergyCostToUse = 50;

		name = "Green Amp";
		description = effectApppliedToSector.description;
		//onSelfEffect = new BlueAmplificationEffect();
		//onOpponentEffect = new BlueAmplificationEffect();
	}

	public void CallTargetSelectManager()
	{
		PlayerTargetSelectManager.Instance.InitiateSelectingPlayerShipSector(this);
	}

	public void SetTarget(object selectedTarget)
	{
		SectorView targetedView = selectedTarget as SectorView;
		Debug.Assert(targetedView != null, "Could not find targeted view!");
		targetSectorIndex = targetedView.sectorIndex;
		PlayerShipModel.main.shipSectors[targetSectorIndex].HandleSectorStatusEffectApplication(effectApppliedToSector);
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

public class BlueAmp : ShipEquipment, IRequiresPlayerTargetSelect
{
	int targetSectorIndex;

	SectorStatusEffect effectApppliedToSector = new BlueAmplificationEffect();

	protected override void Initialize()
	{
		maxCooldownTime = 2;
		greenEnergyCostToUse = 20;
		//greenEnergyCostToUse = 25;
		//greenEnergyCostToUse = 50;

		name = "Blue Amp";
		description = effectApppliedToSector.description;
		//onSelfEffect = new BlueAmplificationEffect();
		//onOpponentEffect = new BlueAmplificationEffect();
	}

	public void CallTargetSelectManager()
	{
		PlayerTargetSelectManager.Instance.InitiateSelectingPlayerShipSector(this);
	}

	public void SetTarget(object selectedTarget)
	{
		SectorView targetedView = selectedTarget as SectorView;
		Debug.Assert(targetedView != null, "Could not find targeted view!");
		targetSectorIndex = targetedView.sectorIndex;
		PlayerShipModel.main.shipSectors[targetSectorIndex].HandleSectorStatusEffectApplication(effectApppliedToSector);
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

public class Stabilizer : ShipEquipment, IRequiresPlayerTargetSelect
{
	const int lowerByRows = 2;
	int lowerInSegmentIndex = -1;

	protected override void Initialize()
	{
		maxCooldownTime = 4;
		blueEnergyCostToUse = 60;
		name = "Stabilizer";
		description = string.Format("Lowers a selected sector's grid by {0} rows", lowerByRows);
	}

	protected override void ExtenderActivation()
	{
		Debug.Assert(lowerInSegmentIndex != -1, "Target ship segment index not defined!");
		Clearer.Instance.EmptyBottomRowsInSegment(lowerByRows, lowerInSegmentIndex);
	}

	public void CallTargetSelectManager()
	{
		PlayerTargetSelectManager.Instance.InitiateSelectingPlayerShipSector(this);
	}

	public void SetTarget(object selectedTarget)
	{
		SectorView targetedView = selectedTarget as SectorView;
		Debug.Assert(targetedView != null, "Could not find targeted view!");
		lowerInSegmentIndex = targetedView.sectorIndex;
	}
}

public class Heatsink : ShipEquipment
{
	const int lowerCooldownsBy = 1;

	protected override void Initialize()
	{
		maxCooldownTime = 4;
		shipEnergyCostToUse = 300;
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

public class ShieldGenerator : ShipEquipment, IRequiresPlayerTargetSelect
{
	int shieldsGain;
	int bonusPerShieldBlock = 10;
	int sectorIndex;
	protected override void Initialize()
	{
		maxCooldownTime = 3;
		blueEnergyCostToUse = 30;
		shieldsGain = 40;
		name = "Forcefield";
		description = string.Format("Restore {0} shields, plus {1} per blue block, destroy all blue blocks", shieldsGain, bonusPerShieldBlock);
	}

	public void CallTargetSelectManager()
	{
		PlayerTargetSelectManager.Instance.InitiateSelectingPlayerShipSector(this);
	}

	public void SetTarget(object selectedTarget)
	{
		SectorView targetedView = selectedTarget as SectorView;
		Debug.Assert(targetedView != null, "Could not find targeted view!");
		sectorIndex = targetedView.sectorIndex;
		
	}

	protected override void ExtenderActivation()
	{
		List<Cell> shieldBlockCellsInSegment = new List<Cell>();
		foreach (SettledBlock block in Grid.Instance.GridSegments[sectorIndex].GetBlocksOfType(BlockType.Shield))
			shieldBlockCellsInSegment.Add(block.GetCell());
		Clearer.Instance.EmptyCells(shieldBlockCellsInSegment);

		int totalShieldPointsGained = shieldsGain;

		totalShieldPointsGained += bonusPerShieldBlock * shieldBlockCellsInSegment.Count;
		PlayerShipModel.main.shipSectors[sectorIndex].healthManager.RestoreShields(totalShieldPointsGained);
		
	}
}

public class RepairDrones : ShipEquipment, IRequiresPlayerTargetSelect
{
	const float repairPercentage = 0.33f;

	int targetedShipSectorIndex;

	protected override void Initialize()
	{
		maxCooldownTime = 3;
		blueEnergyCostToUse = 60;
		name = "Repair Drones";
		description = string.Format("Repairs a third of a chosen sector's health. Can restore damaged sectors");
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

	protected override void ExtenderActivation()
	{
		ShipSectorModel targetedSector = PlayerShipModel.main.shipSectors[targetedShipSectorIndex];
		targetedSector.healthManager.RepairToPercentage(repairPercentage);
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
		onSectorEffect = new EnergySiphonEffect();
	}
}




