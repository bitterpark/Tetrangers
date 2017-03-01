using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipModel
{

	public string shipName { get; private set;}

	public int shipHealth {get; private set;}
	public int shipHealthMax { get; private set; }

	public int shipEnergy { get; private set; }

	public Sprite shipSprite { get; private set; }

	public delegate void EmptyDeleg();

	public event EmptyDeleg ETookDamage;
	public event EmptyDeleg EKilled;
	public event EmptyDeleg EEnergyChanged;

	public ShipModel(int shipHealthMax, int shipEnergy, Sprite shipSprite, string shipName)
	{
		this.shipHealthMax = shipHealthMax;
		shipHealth = shipHealthMax;

		this.shipEnergy = shipEnergy;

		this.shipSprite = shipSprite;
		this.shipName = shipName;
	}

	public void TakeDamage(int damage)
	{
		shipHealth -= damage;
		if (shipHealth <= 0)
		{
			shipHealth = 0;
			if (EKilled != null) EKilled();
		}
		else
			if (ETookDamage != null) ETookDamage();
	}

	public void ChangeEnergy(int delta)
	{
		shipEnergy += delta;
		if (EEnergyChanged != null)
			EEnergyChanged();
	}
	
}
