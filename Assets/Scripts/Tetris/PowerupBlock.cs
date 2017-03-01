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

	public override void Initialize(int startingGridX, int startingGridY)
	{
		base.Initialize(startingGridX, startingGridY);
		PickRandomPowerupType();
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
	
	void PickRandomPowerupType()
	{
		System.Array types = System.Enum.GetValues(typeof(PowerupType));
		myPowerupType = (PowerupType)types.GetValue(Random.Range(0, types.Length));
		//testing
		myPowerupType = PowerupType.Bomb;

		if (myPowerupType == PowerupType.Freeze)
			powerupImage.sprite = freezeSprite;
		if (myPowerupType == PowerupType.Bomb)
			powerupImage.sprite = bombSprite;
		if (myPowerupType == PowerupType.Change)
			powerupImage.sprite = changeSprite;

	}
}
