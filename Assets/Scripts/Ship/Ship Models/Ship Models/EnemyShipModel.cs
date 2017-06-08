using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyShipModel : ShipModel
{

	public static event UnityAction EEnemyDied;

	public static EnemyShipModel currentlyActive = null;

	//public EnemyShipSectorModel sectorModel { get; private set; }
	public HealthAndShieldsManager healthManager { get; private set; }
	public SectorEnergyManager energyManager { get; private set; }

	BattleAI myAI;

	public static EnemyShipModel GetEnemyShipModelInstance(bool lightShip)
	{
		/*
		float randomValue = UnityEngine.Random.value;

		EnemyShipModel result = null;

		System.Type[] allShipTypes = {typeof(HeavyShip), typeof(AssaultShip) };
		//result = (EnemyShipModel)System.Activator.CreateInstance(allShipTypes[Random.Range(0,allShipTypes.Length)]);
		result = new HeavyShip();*/
		if (lightShip) return new LightShip();
		else return new HeavyShip();
	}

	public EnemyShipModel()
	{
		
	}
	//This makes sure the sector model is initialized before equipment is assigned to it

	protected override void SetStartingStats(int healthMax, int shieldsMax, int shieldsGain, int blueEnergyMax, int greenEnergyMax, Sprite sprite, string name)
	{		
		energyManager = SetupEnergyManager(blueEnergyMax, greenEnergyMax);
		healthManager = SetupHealthManager(healthMax, shieldsMax);
		healthManager.EHealthDepleted += DoDeathEvent;
		base.SetStartingStats(healthMax, shieldsMax, shieldsGain, blueEnergyMax, greenEnergyMax, sprite, name);
	}

	SectorEnergyManager SetupEnergyManager(int blueMax, int greenMax)
	{
		return new SectorEnergyManager(
			BalanceValuesManager.Instance.enemyBlueGainPerMove
			, blueMax
			, BalanceValuesManager.Instance.enemyGreenGainPerEngagement
			, greenMax);
	}

	HealthAndShieldsManager SetupHealthManager(int healthMax, int shieldsMax)
	{
		return new HealthAndShieldsManager(healthMax, shieldsMax, BalanceValuesManager.Instance.enemyShieldGain);
	}

	protected override ICanSpendEnergy CreateOrGetAppropriateEnergyUser(int blueMax, int greenMax)
	{
		return energyManager;
	}

	protected override EquipmentUser CreateAppropriateEquipmentUser()
	{
		return new EnemyEquipmentUser(this, this);
	}

	protected override void InitializeForBattle()
	{
		currentlyActive = this;
		myAI = new BattleAI(this);
		base.InitializeForBattle();

		PlayerEquipmentUser.EPlayerAppliedStatusEffectToEnemy += statusEffectManager.AddNewStatusEffect;
		TetrisManager.ECurrentPlayerMoveDone += GainEnergyOnPlayerMove;
		BattleManager.EEngagementModeEnded += GainEnergyOnNewRound;
		//sectorModel.InitializeForBattle();
		PlayerEquipmentUser.EPlayerWeaponFired += GetHitByWeapon;
	}

	public override void DisposeModel()
	{
		energyManager.Dispose();
		healthManager.Dispose();

		PlayerEquipmentUser.EPlayerAppliedStatusEffectToEnemy -= statusEffectManager.AddNewStatusEffect;
		TetrisManager.ECurrentPlayerMoveDone -= GainEnergyOnPlayerMove;
		BattleManager.EEngagementModeEnded -= GainEnergyOnNewRound;

		PlayerEquipmentUser.EPlayerWeaponFired -= GetHitByWeapon;
		//sectorModel.Dispose();
		myAI.Dispose();
		myAI = null;
		currentlyActive = null;
		base.DisposeModel();

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

	protected override void GetHitByWeapon(AttackInfo attack)
	{
		healthManager.TakeDamage(attack);
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

		shipEquipment.AddEquipment(new HeavyLaser(), new Railgun());
		//sectorModel.sectorEquipment.AddEquipment(new PlasmaCannon(), new ReactiveArmor(), new Forcefield());
		//shipEquipment.AddEquipment(new ReactiveArmor(), new Forcefield());
		//shipEquipment.AddEquipment(new Siphon());

		//shipShieldsGain = 10;
		//SetShieldsGain(10);
		//AddOtherEquipment(new BlueAmp());
	}
}

public class LightShip : EnemyShipModel
{
	protected override void InitializeClassStats()
	{
		SetStartingStats(180
			, 180
			, BalanceValuesManager.Instance.enemyShieldGain
			, BalanceValuesManager.Instance.heavyMaxBlue
			, BalanceValuesManager.Instance.heavyMaxGreen
			, SpriteDB.Instance.shipsprite, "Light Ship");

		if (Random.value<0.33f)
			shipEquipment.AddEquipment(new HeavyLaser());
		else
			if (Random.value<0.66f)
			shipEquipment.AddEquipment(new DualRailgun());
		else
			shipEquipment.AddEquipment(new TorpedoLauncher());
		//sectorModel.sectorEquipment.AddEquipment(new PlasmaCannon(), new ReactiveArmor(), new Forcefield());
		//shipEquipment.AddEquipment(new ReactiveArmor(), new Forcefield());
		if (Random.value<0.5f)
			shipEquipment.AddEquipment(new Siphon());

		//shipShieldsGain = 10;
		//SetShieldsGain(10);
		//AddOtherEquipment(new BlueAmp());
	}
}