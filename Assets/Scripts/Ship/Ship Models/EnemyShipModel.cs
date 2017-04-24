
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyShipModel : ShipModel {

	public static event UnityAction EEnemyDied;

	public static EnemyShipModel currentlyActive = null;

	public bool energyGainForFigureHoverEnabled = false;
	BattleAI myAI;
	//readonly int greenEnergyGainPerPlayerMove;
	//readonly int blueEnergyGainPerSecondOfHover;

	public static EnemyShipModel GetEnemyShipModelInstance()
	{
		float randomValue = UnityEngine.Random.value;

		EnemyShipModel result = null;

		System.Type[] allShipTypes = {typeof(HeavyShip), typeof(AssaultShip) };
		//result = (EnemyShipModel)System.Activator.CreateInstance(allShipTypes[Random.Range(0,allShipTypes.Length)]);
		result = new HeavyShip();
		return result;
	}

	public EnemyShipModel()
	{
		
	}

	protected override ShipEnergyManager SetupEnergyManager(int blueMax, int greenMax)
	{
		return new ShipEnergyManager(
			BalanceValuesManager.Instance.enemyBlueGainPerMove
			, blueMax
			, BalanceValuesManager.Instance.enemyGreenGainPerEngagement
			, greenMax);
	}

	protected override ShipHealthManager SetupHealthManager(int healthMax, int shieldsMax)
	{
		return new ShipHealthManager(healthMax, shieldsMax, BalanceValuesManager.Instance.enemyShieldGain);
	}

	protected override EquipmentUser SetupEquipmentUser()
	{
		return new EnemyEquipmentUser(this, this);
	}

	protected override void InitializeForBattle()
	{
		currentlyActive = this;
		myAI = new BattleAI(this);
		base.InitializeForBattle();
		TetrisManager.ECurrentPlayerMoveDone += GainEnergyOnPlayerMove;
		BattleManager.EEngagementModeEnded += GainEnergyOnNewRound;
		PlayerEquipmentUser.EPlayerWeaponFired += healthManager.TakeDamage;
		PlayerEquipmentUser.EPlayerAppliedStatusEffectToEnemy += statusEffectManager.AddNewStatusEffect;
	}

	public override void DisposeModel()
	{
		
		base.DisposeModel();
		TetrisManager.ECurrentPlayerMoveDone -= GainEnergyOnPlayerMove;
		BattleManager.EEngagementModeEnded -= GainEnergyOnNewRound;
		PlayerEquipmentUser.EPlayerWeaponFired -= healthManager.TakeDamage;
		PlayerEquipmentUser.EPlayerAppliedStatusEffectToEnemy -= statusEffectManager.AddNewStatusEffect;

		myAI.Dispose();
		myAI = null;
		currentlyActive = null;
		//EEnemyWeaponFired = null;
		//EEnemyDied = null;
		//EEnemyAppliedStatusEffectToPlayer = null;
	}

	protected override void TryRegenShields()
	{
		healthManager.IncreaseShieldsByGain();
		base.TryRegenShields();
	}

	void GainEnergyOnPlayerMove()
	{
		//GainBlueEnergy();
		//ChangeGreenEnergy(greenEnergyGainPerPlayerMove);
		
	}

	void GainEnergyOnNewRound()
	{
		energyManager.IncreaseBlueByGain();
		energyManager.IncreaseGreenByGain();
	}

	protected override void DoDeathEvent()
	{
		if (EEnemyDied != null)
			EEnemyDied();
	}
}
/*
public class BlueFocusShip : EnemyShipModel
{
	protected override void InitializeClassStats()
	{
		SetStartingStats(BalanceValuesManager.Instance.assaultHealth
			, 400
			, BalanceValuesManager.Instance.enemyShieldGain
			,BalanceValuesManager.Instance.assaultMaxBlue
			,BalanceValuesManager.Instance.assaultMaxGreen
			, SpriteDB.Instance.shipsprite,"Blue Focus Ship");
	

		AddWeapons(new LaserGun());
		AddWeapons(new PlasmaCannon());
		if (Random.value < 0.5f)
			AddOtherEquipment(new Siphon());
		else
			AddOtherEquipment(new BlueAmp());
		//AddOtherEquipment(new BlueAmp());
	}
}*/

public class AssaultShip: EnemyShipModel
{
	protected override void InitializeClassStats()
	{
		const int health = 600;

		SetStartingStats(health
			, BalanceValuesManager.Instance.assaultShields
			, BalanceValuesManager.Instance.enemyShieldGain
			, BalanceValuesManager.Instance.heavyMaxBlue
			, BalanceValuesManager.Instance.heavyMaxGreen
			, SpriteDB.Instance.shipsprite, "Assault Ship");

		//shipShieldsGain = ;
		//SetShieldsGain(0);

		shipEquipment.AddEquipment(new HeavyLaser(), new LaserGun());
		if (Random.value<0.5f)
			shipEquipment.AddEquipment(new BlueAmp());
		else
			shipEquipment.AddEquipment(new Siphon());
		//AddOtherEquipment(new BlueAmp());
	}
}

public class HeavyShip : EnemyShipModel
{
	protected override void InitializeClassStats()
	{
		SetStartingStats(BalanceValuesManager.Instance.heavyHealth
			, BalanceValuesManager.Instance.heavyShields
			, BalanceValuesManager.Instance.enemyShieldGain
			, BalanceValuesManager.Instance.heavyMaxBlue
			, BalanceValuesManager.Instance.heavyMaxGreen
			, SpriteDB.Instance.shipsprite, "Heavy Ship");

		//shipEquipment.AddEquipment(new PlasmaCannon());
		//shipEquipment.AddEquipment(new ReactiveArmor(), new Forcefield());
		shipEquipment.AddEquipment(new Siphon(), new Forcefield());

		//shipShieldsGain = 10;
		//SetShieldsGain(10);
		//AddOtherEquipment(new BlueAmp());
	}
}