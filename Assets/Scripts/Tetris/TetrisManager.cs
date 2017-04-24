using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TetrisManager : Singleton<TetrisManager> 
{
	//public delegate void EmptyDeleg();
	public static event UnityAction ETetrisStarted;
	public static event UnityAction ETetrisLost;
	public static event UnityAction ETetrisEndClear;
	public static event UnityAction ECurrentPlayerMoveDone;
	public static event UnityAction ETransitionFromCurrentToNextMove;
	public static event UnityAction ENextPlayerMoveStarted;

	public static bool paused = false;

	const int generatorMaxLevel = 10;
	public int generatorLevel
	{
		get { return _generatorLevel; }
		set
		{
			_generatorLevel = value;
		}
	}
	int _generatorLevel;
	/*
	const float speedMultChangePerGenLevel = 1;
	public float currentSpeedMultiplierModifier
	{
		get { return generatorLevel * speedMultChangePerGenLevel; }
	}*/

	[SerializeField]
	CanvasGroup tetrisPanelGroup;

	[SerializeField]
	Text generatorLevelText;

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

	public void RaiseGeneratorLevel()
	{
		RaiseGeneratorLevel(1);
	}

	public void RaiseGeneratorLevel(int delta)
	{
		SetGeneratorLevel(generatorLevel+delta);
	}

	public void LowerGeneratorLevel()
	{
		LowerGeneratorLevel(1);
	}

	public void LowerGeneratorLevel(int delta)
	{
		SetGeneratorLevel(generatorLevel - delta);
	}

	public void ChangeGeneratorLevel(int delta)
	{
		SetGeneratorLevel(generatorLevel + delta);
	}

	public void ResetGeneratorLevel()
	{
		SetGeneratorLevel(0);
	}

	void SetGeneratorLevel(int newGeneratorLevel)
	{
		generatorLevel = Mathf.Clamp(newGeneratorLevel,0, generatorMaxLevel);
		generatorLevelText.text = generatorLevel.ToString();
	}

	public void StartTetris()
	{
		UnpauseTetris();
		ResetGeneratorLevel();
		StartCoroutine(WaitForGridReadyRoutine());
	}

	IEnumerator WaitForGridReadyRoutine()
	{
		while (!Grid.gridReady)
			yield return null;
		if (ETetrisStarted != null)
			ETetrisStarted();
		//ContinueTetris();
		yield break;
	}


	void HandleGridOverstack()
	{
		PauseTetris();
		if (ETetrisLost != null) ETetrisLost();
		//ETetrisLost.Invoke();

	}

	void HandleFinishedPlayerMove()
	{
		if (ECurrentPlayerMoveDone != null) ECurrentPlayerMoveDone();
		if (ETransitionFromCurrentToNextMove != null) ETransitionFromCurrentToNextMove();
		if (!paused)
			StartNextPlayerMove();
	}

	void ContinueTetris()
	{
		UnpauseTetris();
		StartNextPlayerMove();
	}

	void StartNextPlayerMove()
	{
		if (ENextPlayerMoveStarted != null) ENextPlayerMoveStarted();
	}

	public void EndAndClearTetris()
	{
		//Debug.Log("Calling tetris end clear! Disposing all FigureControllers!");
		if (ETetrisEndClear != null) ETetrisEndClear();
	}


	void PauseTetris()
	{
		//Debug.Log("Pausing tetris");
		paused = true;
		tetrisPanelGroup.alpha = 0.5f;
	}

	void UnpauseTetris()
	{
		//Debug.Log("Unpausing tetris");
		paused = false;
		tetrisPanelGroup.alpha = 1f;
	}

	void Awake()
	{
		BattleManager.EEngagementModeStarted += PauseTetris;
		BattleManager.EEngagementModeEnded += ContinueTetris;
		BattleManager.EBattleManagerDeactivated += EndAndClearTetris;
		BattleManager.EBattleStarted += StartTetris;
		BattleManager.EBattleFinished += PauseTetris;
		FigureSettler.ENewFigureSettled += HandleFinishedPlayerMove;
		FigureSpawner.ENoRoomToDropFigure += HandleGridOverstack;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
			RaiseGeneratorLevel();
		if (Input.GetKeyDown(KeyCode.L))
			LowerGeneratorLevel();
		//if (Input.GetKeyDown(KeyCode.P))
		//PauseTetris();
		//if (Input.GetKeyDown(KeyCode.U))
		//UnpauseTetris();
	}
}
