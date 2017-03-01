using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PowerupType { Freeze, Bomb, Change };

public class PowerupActivator : Singleton<PowerupActivator> 
{

	[SerializeField]
	Text powerupTextPrefab;
	[SerializeField]
	Transform powerupTextGroup;

	public void ActivatePowerup(PowerupType type)
	{
		IPowerup powerup = GetPowerupInstance(type);
		StartCoroutine(PerformPowerupEffectRoutine(powerup.ActivateEffect,type));
	}

	IPowerup GetPowerupInstance(PowerupType type)
	{
		if (type == PowerupType.Freeze)
			return new FreezeTime();
		if (type == PowerupType.Bomb)
			return new Bomb();
		if (type == PowerupType.Change)
			return new Change();

		Debug.LogErrorFormat("Could not get powerup instance from type {0}",type);
		return null;
	}

	IEnumerator PerformPowerupEffectRoutine(System.Func<IEnumerator> routineFunc, PowerupType powerupType)
	{
		//Debug.Log("Starting powerup effect!");
		Text newPowerupText = Instantiate(powerupTextPrefab,powerupTextGroup);
		newPowerupText.text = powerupType.ToString()+" active!";
		yield return StartCoroutine(routineFunc());
		GameObject.Destroy(newPowerupText.gameObject);
		yield break;
	}

}
