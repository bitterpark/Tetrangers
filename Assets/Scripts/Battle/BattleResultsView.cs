using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultsView : Subscreen {


	public static event UnityEngine.Events.UnityAction EBattleResultsViewClosed;

	[SerializeField]
	Text battleResultsText;

	public void OpenSubscreen(bool battleWon)
	{
		base.OpenSubscreen();
		if (battleWon)
			battleResultsText.text = "Enemy Defeated";
		else
			battleResultsText.text = "Mission Failed";
	}

	protected override void SubscreenCloseEventCaller()
	{
		if (EBattleResultsViewClosed != null) EBattleResultsViewClosed();
	}
}
