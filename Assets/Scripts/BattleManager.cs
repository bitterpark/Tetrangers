using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattleManager : Singleton<BattleManager> {

	
	public static event UnityAction EBattleStarted;
	public static event UnityAction EBattleFinished;
	public static event UnityAction EEngagementModeStarted;
	public static event UnityAction EEngagementModeEnded;
	public static event UnityAction EBattleManagerDeactivated;
	public static event UnityAction EBattleManagerActivated;

	[SerializeField]
	Button engagementButton;
	[SerializeField]
	Text engagementStatusText;

	[SerializeField]
	ShipView playerShipView;
	[SerializeField]
	ShipView enemyShipView;

	[SerializeField]
	BattleResultsView battleResultsView;

	EnemyShipController enemyShipController;
	PlayerShipController playerShipController;

	int turnsPerEngagement;
	int turnsRemaining
	{
		get { return _turnsRemaining; }
		set
		{
			_turnsRemaining = value;
			engagementStatusText.text = _turnsRemaining.ToString();
			if (value == 1)
				engagementButton.GetComponent<Animator>().SetTrigger("Start_Alert");
		}
	}
	int _turnsRemaining;

	//Only called once, happens before start
	void Awake()
	{
		turnsPerEngagement = BalanceValuesManager.Instance.movesPerEngagement;
		TetrisManager.ETransitionFromCurrentToNextMove += AdvanceTurn;
		MissionManager.EMissionWon += DeactivateBattleManager;
		MissionManager.EMissionFailed += DeactivateBattleManager;
	}

	void Update()
	{
		//if (Input.GetKeyDown(KeyCode.B))
			//EndBattle();
	}

	public void StartNewBattle(PlayerShipModel playerShip, EnemyShipModel enemyShip)
	{
		if (!gameObject.activeSelf)
			ActivateBattleManager();

		enemyShip.ActivateModel();
		//if (playerShipController==null)
			DisplayPlayerShip(playerShip);
		//if (enemyShipController==null)
			DisplayEnemyShip(enemyShip);
		turnsRemaining = turnsPerEngagement;

		PlayerShipModel.EPlayerDied += LoseBattle;
		TetrisManager.ETetrisLost += LoseBattle;
		EnemyShipModel.EEnemyDied += WinBattle;

		if (EBattleStarted != null)
			EBattleStarted();
	}

	void ActivateBattleManager()
	{
		gameObject.SetActive(true);
		engagementButton.onClick.RemoveAllListeners();
		engagementButton.onClick.AddListener(EndEngagementMode);
		if (EBattleManagerActivated != null) EBattleManagerActivated();
	}

	void DisplayPlayerShip(ShipModel playerShipModel)
	{
		//ShipModel playerShipModel = new PlayerShipModel(100, 100, 250, tempShipSprite, "Player Ship");
		playerShipController = new PlayerShipController(playerShipModel, playerShipView);
	}
	void ClearPlayerShip(bool clearModel)
	{
		playerShipController.DisposeController(clearModel);
		playerShipController = null;
	}

	void DisplayEnemyShip(ShipModel enemyShipModel)
	{
		//ShipModel enemyShipModel = new EnemyShipModel(50, 200, 250, tempShipSprite, "Enemy Ship");
		enemyShipController = new EnemyShipController(enemyShipModel, enemyShipView);
	}
	void ClearEnemyShip(bool clearModel)
	{
		enemyShipController.DisposeController(clearModel);
		enemyShipController = null;
	}

	void AdvanceTurn()
	{
		turnsRemaining--;
		if (turnsRemaining == 0)
			StartEngagementMode();
	}

	void StartEngagementMode()
	{
		engagementButton.GetComponent<Animator>().SetTrigger("Stop_Alert");
		engagementButton.interactable = true;
		engagementStatusText.text = "Disengage";

		//Debug.Log("Engagement mode started");

		if (EEngagementModeStarted != null)
			EEngagementModeStarted();
	}

	void RevertEngagementMode()
	{
		engagementButton.interactable = false;
		turnsRemaining = turnsPerEngagement;
	}

	void EndEngagementMode()
	{
		RevertEngagementMode();
		if (EEngagementModeEnded != null)
			EEngagementModeEnded();
	}

	void WinBattle()
	{
		ClearEnemyShip(true);
		ClearPlayerShip(false);
		EndBattle();
		battleResultsView.DisplayBattleResults(100, 5000);
	}

	void LoseBattle()
	{
		ClearEnemyShip(true);
		ClearPlayerShip(false);
		EndBattle();
		battleResultsView.DisplayBattleLoss();
	}

	void EndBattle()
	{
		PlayerShipModel.EPlayerDied -= LoseBattle;
		TetrisManager.ETetrisLost -= LoseBattle;
		EnemyShipModel.EEnemyDied -= WinBattle;

		engagementButton.GetComponent<Animator>().SetTrigger("Stop_Alert");
		RevertEngagementMode();

		if (EBattleFinished != null)
			EBattleFinished();
	}

	void DeactivateBattleManager()
	{
		gameObject.SetActive(false);
		if (EBattleManagerDeactivated != null) 
			EBattleManagerDeactivated();
	}

}
