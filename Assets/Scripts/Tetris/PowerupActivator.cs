using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PowerupType { Freeze, Bomb, Change };

public class PowerupActivator : Singleton<PowerupActivator> 
{

	public delegate void PowerupActivateDeleg(PowerupType type);
	public static event PowerupActivateDeleg EPowerupActivated;
	public delegate void EmptyDeleg();
	public static event EmptyDeleg EDeactivateAllPowerupRoutines;

	[SerializeField]
	Text powerupTextPrefab;
	[SerializeField]
	Transform powerupTextGroup;

	void Awake()
	{
		TetrisManager.ETetrisFinished += ClearOnTetrisEnd;
	}

	void ClearOnTetrisEnd()
	{
		if (EDeactivateAllPowerupRoutines != null)
			EDeactivateAllPowerupRoutines();
		foreach (Text powerupText in powerupTextGroup.GetComponentsInChildren<Text>())
			GameObject.Destroy(powerupText.gameObject);
		StopAllCoroutines();
	}

	public void ActivatePowerup(PowerupType type)
	{
		if (EPowerupActivated != null)
			EPowerupActivated(type);
		IPowerup powerup = GetPowerupInstance(type);
		StartCoroutine(PerformPowerupEffectRoutine(powerup.GetPowerupRoutine,type));
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
		Text newPowerupText = Instantiate(powerupTextPrefab,powerupTextGroup);
		newPowerupText.text = powerupType.ToString()+" active!";
		yield return StartCoroutine(routineFunc());
		GameObject.Destroy(newPowerupText.gameObject);
		yield break;
	}

}
