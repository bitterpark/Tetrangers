using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureSpawner : Singleton<FigureSpawner>
{

	public delegate void EmptyDeleg();
	public static event EmptyDeleg EFigureDropped;

	public List<FigureController> figurePrefabs = new List<FigureController>();
	public Transform nextFigureDisplay;

	int spawnedFiguresX;
	int spawnedFiguresY;

	void Awake()
	{
		TetrisManager.ETetrisStarted += StartSpawning;
		
	}

	public void StartSpawning()
	{
		spawnedFiguresX = Mathf.RoundToInt(Grid.Instance.maxX/2);
		//The 0,0 center of figures is not always at the top of the figure, this makes sure any blocks above the center still fit
		spawnedFiguresY = Grid.Instance.maxY-1;
		DropInCurrentFigure();
		Grid.ENewPlayerMoveDone += DropInCurrentFigure;
	}

	public void ChangeNextFigure()
	{
		FigureController nextFigure = nextFigureDisplay.GetComponentInChildren<FigureController>();
		if (nextFigure == null)
			Debug.LogError("Cannot change next figure - no existing next figure found!");

		GameObject.DestroyImmediate(nextFigure.gameObject);
		SpawnNextFigure();
	}

	public void DropInCurrentFigure()
	{
		FigureController currentFigure = nextFigureDisplay.GetComponentInChildren<FigureController>();
		if (currentFigure == null)
			currentFigure = CreateRandomFigure();

		currentFigure.DropIntoPlay(spawnedFiguresX, spawnedFiguresY);
		if (EFigureDropped != null) EFigureDropped();
		SpawnNextFigure();
	}

	void SpawnNextFigure()
	{
		FigureController nextFigure = CreateRandomFigure();
		nextFigure.DisplayAsNext();
	}

	FigureController CreateRandomFigure()
	{
		FigureController randomPrefab = figurePrefabs[Random.Range(0, figurePrefabs.Count)];
		FigureController newFigure = Instantiate(randomPrefab);
		newFigure.Initialize();
		return newFigure;
	}

	public void TetrisFinished()
	{

	}

}
