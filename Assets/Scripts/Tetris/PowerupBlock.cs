using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PowerupBlock : StaticBlock 
{
	[SerializeField]
	Image powerupImage;

	[SerializeField]
	Sprite freezeSprite;
	[SerializeField]
	Sprite bombSprite;
	[SerializeField]
	Sprite changeSprite;

	PowerupType myPowerupType;

	public void AssignPowerupType(PowerupType type)
	{
		myPowerupType = type;
		
		if (myPowerupType == PowerupType.Freeze)
			powerupImage.sprite = freezeSprite;
		if (myPowerupType == PowerupType.Bomb)
			powerupImage.sprite = bombSprite;
		if (myPowerupType == PowerupType.Change)
			powerupImage.sprite = changeSprite;
	}

	public void DisposePowerup()
	{
		GameObject.Destroy(this.gameObject);
	}

	public void TogglePowerup ()
	{
		PowerupActivator.Instance.ActivatePowerup(myPowerupType);
		GameObject.Destroy(this.gameObject);
	}
}
