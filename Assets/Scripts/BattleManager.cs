using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : Singleton<BattleManager> {

	
	public delegate void EmptyDeleg();
	public static event EmptyDeleg EBattleStarted;
	public static event EmptyDeleg EBattleFinished;
	public static event EmptyDeleg EEngagementModeStarted;
	public static event EmptyDeleg EEngagementModeEnded;

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

	const int turnsPerEngagement = 1;
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
		TetrisManager.ECurrentPlayerMoveDone += AdvanceTurn;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
			EndBattle();
	}

	public void StartNewBattle(PlayerShipModel playerShip, EnemyShipModel enemyShip)
	{
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
			engagementButton.onClick.AddListener(EndEngagementMode);
		}

		enemyShip.ActivateModel();
		//if (playerShipController==null)
			DisplayPlayerShip(playerShip);
		//if (enemyShipController==null)
			DisplayEnemyShip(enemyShip);
		turnsRemaining = turnsPerEngagement;

		PlayerShipModel.EPlayerDied += LoseBattle;
		EnemyShipModel.EEnemyDied += WinBattle;

		if (EBattleStarted != null)
			EBattleStarted();
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
		EnemyShipModel.EEnemyDied -= WinBattle;

		engagementButton.GetComponent<Animator>().SetTrigger("Stop_Alert");
		
		if (EBattleFinished != null)
			EBattleFinished();

		gameObject.SetActive(false);
	}

	void AdvanceTurn()
	{
		turnsRemaining--;
		if (turnsRemaining == 0)
			StartEngagementMode();
	}

	void StartEngagementMode()
	{
		EBattleFinished += RevertEngagementModeAfterBattleEnd;
		
		engagementButton.GetComponent<Animator>().SetTrigger("Stop_Alert");
		engagementButton.interactable = true;
		engagementStatusText.text = "Disengage";

		if (EEngagementModeStarted != null)
			EEngagementModeStarted();
	}

	void RevertEngagementModeAfterBattleEnd()
	{
		EndEngagementMode(true);
	}

	void EndEngagementMode()
	{
		EndEngagementMode(false);
	}

	void EndEngagementMode(bool battleEnded)
	{
		EBattleFinished -= RevertEngagementModeAfterBattleEnd;

		engagementButton.interactable = false;
		turnsRemaining = turnsPerEngagement;

		if (!battleEnded && EEngagementModeEnded != null)
			EEngagementModeEnded();
	}

}
