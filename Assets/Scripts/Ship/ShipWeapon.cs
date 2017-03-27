using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipWeapon: ShipEquipment
{

	public int damage { get; protected set; }

	public ShipWeapon()
	{
		equipmentType = EquipmentTypes.Weapon;
	}

	public int FireWeapon(ShipModel firedByShip)
	{
		base.ActivateEquipment(firedByShip);
		return damage;
	}
}

public class LaserGun : ShipWeapon
{
	protected override void Initialize()
	{
		maxCooldownTime = 1;
		damage = 50;
		blueEnergyCostToUse = 50;
		name = "Laser Gun";
	}
}

public class PlasmaCannon : ShipWeapon
{
	protected override void Initialize()
	{
		maxCooldownTime = 3;
		damage = 150;
		blueEnergyCostToUse = 100;
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
		maxCooldownTime = 2;
		damage = 200;
		blueEnergyCostToUse = 100;
		generatorLevelDelta = 1;
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
		damage = 250;
		blueEnergyCostToUse = 100;
		generatorLevelDelta = 2;
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

public class MissileLauncher : ShipWeapon
{
	protected override void Initialize()
	{
		maxCooldownTime = 2;
		damage = 150;
		greenEnergyCostToUse = 50;
		name = "Missile Launcher";
	}
}
public class MissileLauncherTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 100;
		materialsRequired = 200;

		providesEquipment = new MissileLauncher();
		unlocksTopics.Add(new TorpedoLauncherTopic());
	}
}

public class TorpedoLauncher : ShipWeapon
{
	protected override void Initialize()
	{
		maxCooldownTime = 3;
		damage = 250;
		greenEnergyCostToUse = 100;
		name = "Torpedo Launcher";
	}
}
public class TorpedoLauncherTopic : ResearchTopic
{
	protected override void InitializeValues()
	{
		intelRequired = 200;
		materialsRequired = 400;

		providesEquipment = new TorpedoLauncher();
	}
}
