using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureSpawner : Singleton<FigureSpawner>
{

	public static event UnityEngine.Events.UnityAction EFigureDropped;
    public static event UnityEngine.Events.UnityAction ENoRoomToDropFigure;

	public static bool coolantMode = true;

	public Transform nextFigureDisplay;
	[SerializeField]
	List<FigureController> figurePrefabs = new List<FigureController>();
	
	int spawnedFiguresX;
	int spawnedFiguresY;

	void Awake()
	{
		TetrisManager.ETetrisStarted += StartSpawning;
		TetrisManager.ETetrisEndClear += ClearOnTetrisEnd;
		TetrisManager.ENextPlayerMoveStarted += DropInCurrentFigure;
	}

	public void StartSpawning()
	{
		spawnedFiguresX = Mathf.RoundToInt(Grid.Instance.maxX/2);
		//The 0,0 center of figures is not always at the top of the figure, this makes sure any blocks above the center still fit
		spawnedFiguresY = Grid.Instance.maxY-1;
		//DropInCurrentFigure();
	}

	void ClearOnTetrisEnd()
	{
		ClearNextFigure();
	}

	public void ChangeNextFigure()
	{
		TetrominoTypes currentNextFigureType = ClearNextFigure();
		SpawnNextFigure(true, currentNextFigureType);
	}

	TetrominoTypes ClearNextFigure()
	{
		FigureController nextFigure = nextFigureDisplay.GetComponentInChildren<FigureController>();
		if (nextFigure == null)
			Debug.LogError("Cannot clear next figure - no existing next figure found!");
		TetrominoTypes clearedNextFigureType = nextFigure.tetrominoType;
		GameObject.DestroyImmediate(nextFigure.gameObject);
		return clearedNextFigureType;
	}

	public void DropInCurrentFigure()
	{
		//Debug.Log("Dropping figure");
		FigureController currentFigure = nextFigureDisplay.GetComponentInChildren<FigureController>();
		if (currentFigure == null)
			currentFigure = CreateRandomFigure();

        bool roomToDropExists = true;
        foreach (Vector2 blockCoord in currentFigure.figureBlockOffsets)
            if (!Grid.Instance.CellExistsIsUnoccupied(blockCoord+new Vector2(spawnedFiguresX,spawnedFiguresY)))
            {
                roomToDropExists = false;
                break;
            }

        if (roomToDropExists)
        {
			if (coolantMode)
				FigureController.frozen = true;
			currentFigure.DropIntoPlay(spawnedFiguresX, spawnedFiguresY);
            if (EFigureDropped != null) EFigureDropped();
            SpawnNextFigure();
        }
        else
            if (ENoRoomToDropFigure!=null)	ENoRoomToDropFigure();
	}

	void SpawnNextFigure()
	{
		SpawnNextFigure(false, TetrominoTypes.L);
	}

	void SpawnNextFigure(bool excludeAType, TetrominoTypes excludedType)
	{
		FigureController nextFigure;
		if (excludeAType)
			nextFigure = CreateRandomFigure(excludedType);
		else
			nextFigure = CreateRandomFigure();

		nextFigure.DisplayAsNext();
	}

	FigureController CreateRandomFigure()
	{
		return CreateRandomFigure(figurePrefabs);
	}

	FigureController CreateRandomFigure(TetrominoTypes excludeType)
	{
		List<FigureController> allowedPrefabs = new List<FigureController>(figurePrefabs);
		foreach (FigureController prefab in allowedPrefabs)
		{
			if (prefab.tetrominoType == excludeType)
			{
				allowedPrefabs.Remove(prefab);
				break;
			}
		}

		return CreateRandomFigure(allowedPrefabs);
	}

	FigureController CreateRandomFigure(List<FigureController> prefabSelection)
	{
		FigureController randomPrefab = prefabSelection[Random.Range(0, prefabSelection.Count)];
		FigureController newFigure = Instantiate(randomPrefab);
		newFigure.Initialize();
		return newFigure;
	}

}
