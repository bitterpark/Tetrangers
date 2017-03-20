using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MissionManager : Singleton<MissionManager> 
{
	[SerializeField]
	Image startMissionPanel;
	[SerializeField]
	Button startMissionButton;
	[SerializeField]
	Image progressMissionPanel;
	[SerializeField]
	Text progressMissionText;
	[SerializeField]
	Button continueMissionButton;

	[SerializeField]
	Image missionFailedPanel;
	[SerializeField]
	Button missionFailedButton;
	[SerializeField]
	Image missionWonPanel;
	[SerializeField]
	Button missionWonButton;

	[SerializeField]
	Sprite tempShipSprite;

	[SerializeField]
	BattleManager battleManager;

	public PlayerShipModel playerShipTempLocation { get; private set;}

	public static event UnityAction EMissionWon;
	public static event UnityAction EMissionFailed;
	public static event UnityAction EMissionStarted;

	struct Mission
	{
		public EnemyShipModel[] enemyShips;
		public string description;

		public int missionLength { get { return enemyShips.Length; } }

		public Mission(string description, int enemyShipCount)
		{
			enemyShips = new EnemyShipModel[enemyShipCount];
			for (int i = 0; i < enemyShipCount; i++)
				enemyShips[i] = EnemyShipModel.GetEnemyShipModelInstance();
			this.description = description;
		}

		public Mission(string description, params EnemyShipModel[] enemyShips)
		{
			this.enemyShips = enemyShips;
			this.description = description;
		}
	}

	Mission currentMission;
	int shipsDefeatedInCurrentMission;

	void Awake()
	{
		BattleResultsView.EMissionContinueTriggered += ProgressCurrentMission;
		BattleResultsView.EMissionFailureTriggered += FailCurrentMission;
		startMissionButton.onClick.AddListener(StartNewMission);
		continueMissionButton.onClick.AddListener(ContinueMissionButtonPressed);
		missionWonButton.onClick.AddListener(MissionWinButtonPressed);
		missionFailedButton.onClick.AddListener(MissionFailButtonPressed);

		playerShipTempLocation = PlayerShipModel.GetPlayerShipModelInstance();
	}

	public void InitializeMissionManager()
	{
		startMissionPanel.gameObject.SetActive(true);
	}

	void StartNewMission()
	{
		currentMission = new Mission("Defeat all enemy ships", 3);
		shipsDefeatedInCurrentMission = 0;
		startMissionPanel.gameObject.SetActive(false);
		ShowMissionProgressPanel();
		if (EMissionStarted != null) EMissionStarted();
		//battleManager.StartNewBattle(playerShipTempLocation, currentMission.enemyShips[shipsDefeatedInCurrentMission]);
	}

	void ProgressCurrentMission()
	{
		shipsDefeatedInCurrentMission++;
		ShowMissionProgressPanel();
	}

	void ShowMissionProgressPanel()
	{
		progressMissionPanel.gameObject.SetActive(true);

		if (shipsDefeatedInCurrentMission < currentMission.missionLength)
		{
			progressMissionText.text = string.Format("Defeated:{0}/{1}\nNext enemy:{2}"
			, shipsDefeatedInCurrentMission, currentMission.missionLength, currentMission.enemyShips[shipsDefeatedInCurrentMission].shipName);
			continueMissionButton.GetComponentInChildren<Text>().text = "Attack";
		}
		else
		{
			progressMissionText.text = string.Format("Mission complete!");
			continueMissionButton.GetComponentInChildren<Text>().text = "Continue";
		}
	}

	void ContinueMissionButtonPressed()
	{
		progressMissionPanel.gameObject.SetActive(false);
		if (shipsDefeatedInCurrentMission < currentMission.missionLength)
			battleManager.StartNewBattle(playerShipTempLocation, currentMission.enemyShips[shipsDefeatedInCurrentMission]);
		else
			WinCurrentMission();	
	}

	void WinCurrentMission()
	{
		if (EMissionWon != null) EMissionWon();
		missionWonPanel.gameObject.SetActive(true);
	}
	void MissionWinButtonPressed()
	{
		missionWonPanel.gameObject.SetActive(false);
		InitializeMissionManager();
	}

	void FailCurrentMission()
	{
		if (EMissionFailed != null) EMissionFailed();
		missionFailedPanel.gameObject.SetActive(true);
	}
	void MissionFailButtonPressed()
	{
		missionFailedPanel.gameObject.SetActive(false);
		InitializeMissionManager();
	}

}
