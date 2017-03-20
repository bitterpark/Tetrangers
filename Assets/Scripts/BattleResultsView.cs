using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultsView : Singleton<BattleResultsView> {

	public static event UnityEngine.Events.UnityAction EMissionContinueTriggered;
	public static event UnityEngine.Events.UnityAction EMissionFailureTriggered;

	[SerializeField]
	Text battleResultsText;
	[SerializeField]
	Button continueMissionButton;

	void Awake()
	{
		
	}

	public void DisplayBattleResults(int xpGained, int creditsGained)
	{
		gameObject.SetActive(true);

		battleResultsText.text = "";
		battleResultsText.text += "Credits: "+creditsGained;
		battleResultsText.text += "\nXP: " + xpGained;
		continueMissionButton.onClick.RemoveAllListeners();
		continueMissionButton.onClick.AddListener(ContinueToMissionProgress);
	}

	void ContinueToMissionProgress()
	{
		gameObject.SetActive(false);
		if (EMissionContinueTriggered != null)
			EMissionContinueTriggered();
	}

	public void DisplayBattleLoss()
	{
		gameObject.SetActive(true);

		continueMissionButton.onClick.RemoveAllListeners();
		continueMissionButton.onClick.AddListener(GoToMissionFailure);

		battleResultsText.text = "Mission Failed";
	}

	void GoToMissionFailure()
	{
		gameObject.SetActive(false);
		if (EMissionFailureTriggered != null)
			EMissionFailureTriggered();
	}

}
