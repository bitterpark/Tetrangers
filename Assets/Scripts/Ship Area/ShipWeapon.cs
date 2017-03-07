using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipWeapon 
{
	public int damage { get; set; }
	public int energyCostToFire { get; set; }
	public string name { get; set; }
	public int cooldownTime { get; protected set; }
	public int cooldownTimeRemaining 
	{
		get { return _cooldownTimeRemaining; }
		set { _cooldownTimeRemaining = Mathf.Max(value, 0); } 
	}
	int _cooldownTimeRemaining = 0;

	public ShipWeapon()
	{
		Initialize();
	}

	protected abstract void Initialize();

	public void SetCooldown()
	{
		cooldownTimeRemaining = cooldownTime;
	}
}

public class LaserGun : ShipWeapon
{
	protected override void Initialize()
	{
		cooldownTime = 1;
		damage = 10;
		energyCostToFire = 100;
		name = "Laser Gun";
	}
}

public class PlasmaCannon : ShipWeapon
{
	protected override void Initialize()
	{
		cooldownTime = 2;
		damage = 20;
		energyCostToFire = 200;
		name = "Plasma Cannon";
	}
}