using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TetrisManager : Singleton<TetrisManager> 
{
	//public delegate void EmptyDeleg();
	public static event UnityAction ETetrisStarted;
	public static event UnityAction ETetrisFinished;
	public static event UnityAction ECurrentPlayerMoveDone;
	public static event UnityAction ENextPlayerMoveStarted;

	public static bool paused = false;

	[SerializeField]
	CanvasGroup tetrisPanelGroup;

	public static float tetrisDeltaTime
	{
		get
		{
			if (!paused)
				return Time.deltaTime;
			else
				return 0;
		}
	}

	public void StartTetris()
	{
		UnpauseTetris();
		StartCoroutine(WaitForGridReadyRoutine());
	}

	IEnumerator WaitForGridReadyRoutine()
	{
		while (!Grid.gridReady)
			yield return null;
		if (ETetrisStarted != null)
			ETetrisStarted();
		yield break;
	}


	void HandleFinishedPlayerMove(Rect dummySettledBlockArg)
	{
		if (ECurrentPlayerMoveDone != null) ECurrentPlayerMoveDone();
		//Debug.Log("Assering pause status: "+paused);
		if (!paused)
			StartNextPlayerMove();
	}

	void PauseTetrisForEngagementMode()
	{
		PauseTetris();
		//Debug.Log("paused status: "+paused);
	}

	void UnpauseTetrisAfterEngagementMode()
	{
		UnpauseTetris();
		StartNextPlayerMove();
	}

	void StartNextPlayerMove()
	{
		if (ENextPlayerMoveStarted != null) ENextPlayerMoveStarted();
	}

	public void EndTetris()
	{
		if (ETetrisFinished != null) ETetrisFinished();
	}


	void PauseTetris()
	{
		//Debug.Log("Pausing tetris");
		paused = true;
		tetrisPanelGroup.alpha = 0;
	}

	void UnpauseTetris()
	{
		//Debug.Log("Unpausing tetris");
		paused = false;
		tetrisPanelGroup.alpha = 1;
	}

	void Awake()
	{
		BattleManager.EEngagementModeStarted += PauseTetrisForEngagementMode;
		BattleManager.EEngagementModeEnded += UnpauseTetrisAfterEngagementMode;
		BattleManager.EBattleStarted += StartTetris;
		BattleManager.EBattleFinished += EndTetris;
		Grid.ENewFigureSettled += HandleFinishedPlayerMove;
	}

	void Update()
	{
		//if (Input.GetKeyDown(KeyCode.P))
		//PauseTetris();
		//if (Input.GetKeyDown(KeyCode.U))
		//UnpauseTetris();
	}
}
