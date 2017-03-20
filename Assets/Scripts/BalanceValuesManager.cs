﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceValuesManager : Singleton<BalanceValuesManager> {

	[Header("Game stats")]
	public int movesPerEngagement;
	public float powerupSpawnChancePerMove;

	public int bluePointsWorthPerGreenPoint;

	[Header("Player stats")]
	public int playerShipHealth;
	//public int playerGainPerRow;
	//public float playerGainPerSavedSecond;
	//public int playerMaxAmountOfSavedSeconds;
	public int playerBlueGain;
	public int playerGreenGain;
	public int playerBlueGainPerSecondSaved;
	public int playerGreenMax;
	public int playerBlueMax;


	[Header("General enemy stats")]
	public int enemyBlueGainPerMove;
	public int enemyGreenGainPerEngagement;
	public int enemyBlueGainPerHoverSecond;

	[Header("Assault enemy stats")]
	public int assaultHealth;
	public int assaultMaxBlue;
	public int assaultMaxGreen;
	[Header("Heavy enemy stats")]
	public int heavyHealth;
	public int heavyMaxBlue;
	public int heavyMaxGreen;


	public int GetTotalPointsValue(int blueEnergy, int greenEnergy)
	{
		int total = blueEnergy;
		total += greenEnergy * bluePointsWorthPerGreenPoint;

		return total;
	}
}