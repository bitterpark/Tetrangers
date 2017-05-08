﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WeaponDamage
{
	public int minDamage;
	public int maxDamage;

	public AttackType damageType;

	public WeaponDamage(int minDamage, int maxDamage)
	{
		this.minDamage = minDamage;
		this.maxDamage = maxDamage;
		damageType = AttackType.Regular;
	}

	public WeaponDamage(int minDamage, int maxDamage, AttackType damageType)
	{
		this.minDamage = minDamage;
		this.maxDamage = maxDamage;
		this.damageType = damageType;
	}
}

public enum AttackType {Regular, Antishield, Antihull }

public struct WeaponAttack
{
	public int damage;
	public AttackType type;

	public WeaponAttack(WeaponDamage attackDamageInfo)
	{
		damage = Random.Range(attackDamageInfo.minDamage, attackDamageInfo.maxDamage + 1);
		type = attackDamageInfo.damageType;
	}
}

public abstract class ShipWeapon: ShipEquipment
{

	public delegate void WeaponLockonDeleg(ShipEquipment weapon, int time);
	public static event WeaponLockonDeleg EWeaponLockonTimeChanged;

	//public int damage { get; protected set; }
	public WeaponDamage damageInfo { get; protected set; }
	public bool lockingOn { get; protected set; }
	public bool lockedOn
	{
		get { return lockOnTimeRemaining == 0; }
	}
	public int lockOnTimeRemaining
	{
		get { return _lockOnTimeRemaining; }
		set
		{
			_lockOnTimeRemaining = Mathf.Clamp(value, 0, lockOnTimeRequired);
			if (EWeaponLockonTimeChanged != null) EWeaponLockonTimeChanged(this,lockOnTimeRemaining);
		}
	}
	int _lockOnTimeRemaining;

	protected int lockOnTimeRequired = 0;

	public ShipWeapon()
	{
		equipmentType = EquipmentTypes.Weapon;
		equipmentGoal = Goal.Attack;
		//maxCooldownTime = 0;
		lockOnTimeRemaining = lockOnTimeRequired;
	}

	public WeaponAttack ActivateWeapon()
	{
		base.ActivateEquipment();

		WeaponAttack attack = new WeaponAttack(damageInfo);

		return attack;
	}

	/*
	public bool TryActivateWeapon(ShipModel firedByShip, out int damage)
	{
		damage = 0;
		
		if (lockedOn)
		{
			lockingOn = false;
			damage = this.damage;
			lockOnTimeRemaining = lockOnTimeRequired;
			base.ActivateEquipment(firedByShip);
			return true;
		}
		else
		{
			lockingOn = true;
			return false;
		}
	}*/

	public void TryChangeLockonTime(int delta)
	{
		if (lockedOn)
		{
			lockOnTimeRemaining = lockOnTimeRequired;
			lockingOn = false;
		}

		if (lockingOn)
			lockOnTimeRemaining = Mathf.Clamp(lockOnTimeRemaining + delta, 0, lockOnTimeRequired);
	}
	/*
	public override bool IsUsableByShip(ShipModel ship)
	{
		return (lockedOn || !lockingOn);
	}*/
	/*
	public override void ResetEquipment()
	{
		lockingOn = false;
		lockOnTimeRemaining = lockOnTimeRequired;
	}*/

}

public class LaserGun : ShipWeapon
{
	protected override void Initialize()
	{
		//lockOnTimeRequired = 1;
		maxCooldownTime = 1;
		damageInfo = new WeaponDamage(60, 70);
		//generatorLevelDelta = 1;

		//onSelfEffect = new ReactiveArmorEffect();
		//onOpponentEffect = new EnergySiphonEffect();

		blueEnergyCostToUse = 60;
		name = "Laser Gun";
	}
}

public class PlasmaCannon : ShipWeapon
{
	protected override void Initialize()
	{
		maxCooldownTime = 2;
		damageInfo = new WeaponDamage(120, 130, AttackType.Antishield);
		blueEnergyCostToUse = 90;
		name = "Plasma Cannon";
	}
}
public class PlasmaCannonTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 100;
		materialsRequired = 200;

		providesEquipment = new PlasmaCannon();
	}
}

public class HeavyLaser : ShipWeapon
{
	protected override void Initialize()
	{
		maxCooldownTime = 3;
		damageInfo = new WeaponDamage(180, 190);
		blueEnergyCostToUse = 120;
		//generatorLevelDelta = 1;
		name = "Heavy Laser";
	}
}
public class HeavyLaserTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 100;
		materialsRequired = 200;

		providesEquipment = new HeavyLaser();
		unlocksTopics.Add(new TurboLaserTopic());
	}
}

public class TurboLaser : ShipWeapon
{
	protected override void Initialize()
	{
		maxCooldownTime = 3;
		damageInfo = new WeaponDamage(300, 310);
		blueEnergyCostToUse = 150;
		//generatorLevelDelta = 2;
		name = "Turbo Laser";
	}
}
public class TurboLaserTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 200;
		materialsRequired = 400;

		providesEquipment = new TurboLaser();
	}
}

public class NukeLauncher: ShipWeapon
{
	protected override void Initialize()
	{
		maxCooldownTime = 5;
		damageInfo = new WeaponDamage(360, 370);
		ammoCostToUse = 2;
		//generatorLevelDelta = 2;
		name = "Nuke Launcher";
	}
}
/*
public class MissileLauncher : ShipWeapon
{
	protected override void Initialize()
	{
		maxCooldownTime = 1;
		damage = 100;
		greenEnergyCostToUse = 24;
		name = "Missile Launcher";
	}
}*/
/*
public class MissileLauncherTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 100;
		materialsRequired = 200;

		providesEquipment = new MissileLauncher();
		unlocksTopics.Add(new MineLayerTopic());
	}
}*/
/*
public class MineLayer: ShipWeapon
{
	protected override void Initialize()
	{
		maxCooldownTime = 2;
		damage = 150;
		greenEnergyCostToUse = 48;
		name = "Mine Layer";
	}
}
public class MineLayerTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 200;
		materialsRequired = 400;

		providesEquipment = new MineLayer();
		unlocksTopics.Add(new TorpedoLauncherTopic());
	}
}

public class TorpedoLauncher : ShipWeapon
{
	protected override void Initialize()
	{
		maxCooldownTime = 3;
		damage = 250;
		greenEnergyCostToUse = 96;
		name = "Torpedo Launcher";
	}
}
public class TorpedoLauncherTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 400;
		materialsRequired = 800;

		providesEquipment = new TorpedoLauncher();
	}
}*/
