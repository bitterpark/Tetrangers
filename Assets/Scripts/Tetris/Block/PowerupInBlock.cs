using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PowerupInBlock:MonoBehaviour
{
	public UnityEngine.Events.UnityAction EBlockDespawning;

	[SerializeField]
	Image powerupImage;

	

	PowerupType myPowerupType;

	SettledBlock parentBlock = null;

	int lifetimeInEngagements = 2;

	public void AssignPowerupType(PowerupType type)
	{
		myPowerupType = type;
		powerupImage.sprite = Powerup.GetPowerupSprite(type);
	}

	public void Initialize(SettledBlock parentBlock)
	{
		this.parentBlock = parentBlock;
		parentBlock.EThisBlockCleared += TogglePowerup;
		//BattleManager.EEngagementModeStarted += HandleLifetimeDecrease;
	}

	void HandleLifetimeDecrease()
	{
		lifetimeInEngagements--;
		if (lifetimeInEngagements == 1)
			powerupImage.color = new Color(powerupImage.color.r,powerupImage.color.g,powerupImage.color.b,0.5f);
		if (lifetimeInEngagements == 0)
		{
			//This is necessary to ensure the powerups don't activate
			SettledBlock parentBlock = transform.GetComponentInParent<SettledBlock>();
			if (parentBlock != null)
				parentBlock.EThisBlockCleared -= TogglePowerup;

			if (EBlockDespawning != null)
				EBlockDespawning();

			DisposePowerup();
		}
	}

	//Consider making these non-public
	public void DisposePowerup()
	{
		EBlockDespawning = null;
		//BattleManager.EEngagementModeStarted -= HandleLifetimeDecrease;

		//PowerupSpawner.Instance.FreeUpPowerupType(myPowerupType);
		GameObject.Destroy(this.gameObject);
	}

	public void TogglePowerup()
	{
		Powerup.PreparePowerup(myPowerupType, parentBlock.currentX, parentBlock.currentY);
		//PowerupActivator.ActivatePowerup(myPowerupType);
		DisposePowerup();
	}
}


