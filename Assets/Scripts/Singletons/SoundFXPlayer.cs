using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXPlayer : Singleton<SoundFXPlayer> {

	[SerializeField]
	AudioSource playerOne;
	[SerializeField]
	AudioSource playerTwo;

	[SerializeField]
	AudioClip blockClearSound;
	[SerializeField]
	AudioClip isolatedBlockExpireSound;
	[SerializeField]
	AudioClip weaponFireSound;
	[SerializeField]
	AudioClip equipmentActivationSound;
	[SerializeField]
	AudioClip takeDamageSound;


	public void PlayBlockClearSound()
	{
		PlaySound(blockClearSound);
	}

	public void PlayWeaponFireSound()
	{
		PlaySound(weaponFireSound);
	}

	public void PlayEquipmentActivationSound()
	{
		PlaySound(equipmentActivationSound);
	}

	public void PlayIsolatedBlockExpireSound()
	{
		PlaySound(isolatedBlockExpireSound);
	}

	public void PlayTookDamageSound()
	{
		PlaySound(takeDamageSound);
	}

	void PlaySound(AudioClip sound)
	{
		AudioSource usedPlayer = playerOne;
		if (playerOne.isPlaying && !playerTwo.isPlaying)
			usedPlayer = playerTwo;

		usedPlayer.clip = sound;
		usedPlayer.Play();
	}


}
