using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MissionProgressScreen: MissionSubscreen
{

	[SerializeField]
	Text missionProgressText;

	public void OpenSubscreen(int shipsDefeatedInMission, int totalShipsInMission, string nextEnemyShipName)
	{
		base.OpenSubscreen();
		missionProgressText.text = string.Format("Defeated:{0}/{1}\nNext enemy:{2}", shipsDefeatedInMission, totalShipsInMission, nextEnemyShipName);
	}

}

