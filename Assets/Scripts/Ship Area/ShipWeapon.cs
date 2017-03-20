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

	public int FireWeapon()
	{
		SetCooldown();
		return damage;
	}
}

public class LaserGun : ShipWeapon
{
	protected override void Initialize()
	{
		maxCooldownTime = 1;
		damage = 100;
		blueEnergyCostToUse = 100;
		greenEnergyCostToUse = 0;
		name = "Laser Gun";
	}
}

public class PlasmaCannon : ShipWeapon
{
	protected override void Initialize()
	{
		maxCooldownTime = 3;
		damage = 300;
		blueEnergyCostToUse = 200;
		greenEnergyCostToUse = 0;
		name = "Plasma Cannon";
	}
}

public class TorpedoLauncher : ShipWeapon
{
	protected override void Initialize()
	{
		maxCooldownTime = 1;
		damage = 80;
		blueEnergyCostToUse = 0;
		greenEnergyCostToUse = 40;
		name = "Torpedo Launcher";
	}
}