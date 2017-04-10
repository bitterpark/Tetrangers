
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyShipModel : ShipModel {

	public static event UnityAction<int> EEnemyWeaponFired;
	public static event UnityAction<StatusEffect> EEnemyAppliedStatusEffectToPlayer;
	public static event UnityAction EEnemyDied;

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
		greenEnergyGain = BalanceValuesManager.Instance.enemyGreenGainPerEngagement;
		blueEnergyGain = BalanceValuesManager.Instance.enemyBlueGainPerMove;
	}

	public void ActivateModel()
	{
		myAI = new BattleAI(this);
		InitializeEventSubscriptions();
	}

	protected override void InitializeEventSubscriptions()
	{
		base.InitializeEventSubscriptions();
		TetrisManager.ECurrentPlayerMoveDone += GainEnergyOnPlayerMove;
		PlayerShipModel.EPlayerWeaponFired += TakeDamage;
		PlayerShipModel.EPlayerAppliedStatusEffectToEnemy += AddNewStatusEffect;
		BattleManager.EEngagementModeEnded += GainEnergyOnNewRound;
		FigureController.EFigureHoveredForOneSecond += GainEnergyOnFigureHover;

	}

	public override void DisposeModel()
	{
		base.DisposeModel();
		TetrisManager.ECurrentPlayerMoveDone -= GainEnergyOnPlayerMove;
		PlayerShipModel.EPlayerWeaponFired -= TakeDamage;
		PlayerShipModel.EPlayerAppliedStatusEffectToEnemy -= AddNewStatusEffect;
		BattleManager.EEngagementModeEnded -= GainEnergyOnNewRound;
		FigureController.EFigureHoveredForOneSecond -= GainEnergyOnFigureHover;

		myAI.Dispose();
		myAI = null;
		//EEnemyWeaponFired = null;
		//EEnemyDied = null;
		//EEnemyAppliedStatusEffectToPlayer = null;
	}

	protected override void TryRegenShields()
	{
		if (shipShieldsCurrentGain>0)
			ChangeShields(shipShieldsCurrentGain);
		base.TryRegenShields();
	}

	void GainEnergyOnPlayerMove()
	{
		//GainBlueEnergy();
		//ChangeGreenEnergy(greenEnergyGainPerPlayerMove);
		
	}

	void GainEnergyOnNewRound()
	{
		GainGreenEnergy();
		GainBlueEnergy();
	}
	
	void GainEnergyOnFigureHover()
	{
		if (energyGainForFigureHoverEnabled)
			GainBlueEnergy(BalanceValuesManager.Instance.enemyBlueGainPerHoverSecond,true);
	}

	protected override void DoWeaponFireEvent(int weaponDamage)
	{
		if (EEnemyWeaponFired != null)
			EEnemyWeaponFired(weaponDamage);
	}

	protected override void ApplyStatusEffectToOpponent(StatusEffect effect)
	{
		if (EEnemyAppliedStatusEffectToPlayer != null)
			EEnemyAppliedStatusEffectToPlayer(effect);
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
		blueEnergyGain = BalanceValuesManager.Instance.enemyBlueGainPerMove;
		greenEnergyGain = BalanceValuesManager.Instance.enemyGreenGainPerEngagement;

		AddWeapons(new HeavyLaser(), new LaserGun());
		if (Random.value<0.5f)
			AddOtherEquipment(new BlueAmp());
		else
			AddOtherEquipment(new Siphon());
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

		AddWeapons(new PlasmaCannon());
		AddOtherEquipment(new ReactiveArmor(), new Forcefield());

		//shipShieldsGain = 10;
		//SetShieldsGain(10);
		blueEnergyGain = BalanceValuesManager.Instance.enemyBlueGainPerMove;
		greenEnergyGain = BalanceValuesManager.Instance.enemyGreenGainPerEngagement;
		//AddOtherEquipment(new BlueAmp());
	}
}