using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattleManager : Singleton<BattleManager> {

	public static event UnityAction EBattleWon;
	public static event UnityAction EBattleLost;
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
	PlayerShipViewProvider playerShipView;
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

		enemyShip.TryInitializeForBattle();
		playerShip.TryInitializeForBattle();
		//if (playerShipController==null)
			DisplayPlayerShip(playerShip);
		//if (enemyShipController==null)
			DisplayEnemyShip(enemyShip);
		turnsRemaining = turnsPerEngagement;

		PlayerShipModel.EPlayerDied += DisplayBattleLoss;
		TetrisManager.ETetrisLost += DisplayBattleLoss;
		EnemyShipModel.EEnemyDied += DisplayBattleWin;

		if (EBattleStarted != null)
			EBattleStarted();

		StartEngagementMode();
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

		//IShipViewProvider fuckShitBalls = playerShipView;
		//PlayerShipViewProvider recast = fuckShitBalls as PlayerShipViewProvider;
		//Debug.Assert(recast.sectorEquipmentLists.Count > 0, "Fail fuck!");

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
		turnsRemaining = turnsPerEngagement;
		engagementButton.GetComponent<Animator>().SetTrigger("Stop_Alert");
		engagementButton.interactable = true;
		engagementStatusText.text = string.Format("Disengage ({0})",turnsRemaining);

		//Debug.Log("Engagement mode started");

		if (EEngagementModeStarted != null)
			EEngagementModeStarted();
	}

	public void ChangeTurnsForNextEngagement(int delta)
	{
		SetTurnsForNextEngagement(turnsRemaining + delta);
	}

	void SetTurnsForNextEngagement(int turns)
	{
		_turnsRemaining = turns;
		engagementStatusText.text = string.Format("Disengage ({0})", turns);
	}

	void RevertEngagementMode()
	{
		engagementButton.interactable = false;
		
	}

	void EndEngagementMode()
	{
		RevertEngagementMode();
		if (EEngagementModeEnded != null)
			EEngagementModeEnded();
	}

	void DisplayBattleWin()
	{
		EndBattle();
		battleResultsView.OpenSubscreen(true);
		BattleResultsView.EBattleResultsViewClosed += WinBattle;
	}

	void WinBattle()
	{
		BattleResultsView.EBattleResultsViewClosed -= WinBattle;
		if (EBattleWon != null) EBattleWon();
	}

	void DisplayBattleLoss()
	{	
		EndBattle();
		battleResultsView.OpenSubscreen(false);
		BattleResultsView.EBattleResultsViewClosed += LoseBattle;
	}

	void LoseBattle()
	{
		BattleResultsView.EBattleResultsViewClosed -= LoseBattle;
		if (EBattleLost != null) EBattleLost();
	}

	void EndBattle()
	{
		ClearEnemyShip(true);
		ClearPlayerShip(false);

		PlayerShipModel.EPlayerDied -= DisplayBattleLoss;
		TetrisManager.ETetrisLost -= DisplayBattleLoss;
		EnemyShipModel.EEnemyDied -= DisplayBattleWin;

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
