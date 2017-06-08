using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Grid : Singleton<Grid>
{
	public static bool gridReady = false;

	public float cellSize;

	public Transform gridGroup
	{
		get { return _gridGroup; }
	}
	[SerializeField]
	Transform _gridGroup;

	//int _cellSize = 50;
	[SerializeField]
	Image gridCellImage;

	public int gridVertSize { get { return _gridVertSize; } }
	[SerializeField]
	int _gridVertSize = 20;

	public int gridHorSize { get { return _gridHorSize; } }
	[SerializeField]
	int _gridHorSize = 20;

	public int maxX
	{
		get { return _gridHorSize - 1; }
	}
	public int maxY
	{
		get { return _gridVertSize - 1; }
	}

	public int maxSegmentY
	{
		get { return _gridVertSize - 5; }
	}

	[SerializeField]
	Color oddSegmentCellColor;
	[SerializeField]
	Color evenSegmentCellColor;
	[SerializeField]
	Color nonSegmentCellColor;

	Cell[,] cells;

	//GridFX gridFX;

	public GridSegment[] GridSegments { get { return gridSegments; } }
	GridSegment[] gridSegments;

	public static readonly int segmentCount = 3;
	public static readonly int segmentWidth = 9;


	void Awake()
	{
		TetrisManager.ETetrisEndClear += ClearGrid;
		
		StartCoroutine(WaitForCanvasToSetupRoutine());

		gridSegments = new GridSegment[segmentCount];
		for (int i = 0; i < segmentCount; i++)
		{
			int segmentMinX = segmentWidth * i;
			int segmentMinY = 0;
			int segmentMaxX = segmentMinX + segmentWidth - 1;
			int segmentMaxY = maxSegmentY;
			gridSegments[i] = new GridSegment(segmentMinX, segmentMinY, segmentMaxX, segmentMaxY, i, gridGroup);
		}
	}

	IEnumerator WaitForCanvasToSetupRoutine()
	{
		yield return new WaitForFixedUpdate();
		CreateGrid();
	}

	void CreateGrid()
	{
		cellSize = _gridGroup.GetComponent<RectTransform>().rect.width / _gridHorSize;

		cells = new Cell[_gridHorSize, _gridVertSize];

		for (int i = 0; i < _gridVertSize; i++)
			for (int j = 0; j < _gridHorSize; j++)
			{
				cells[j, i] = new Cell(j, i);
				/*
				Image newCellImage = Instantiate(gridCellImage);
				newCellImage.transform.SetParent(_gridGroup, false);

				if (gridSegments[1].CellCoordsAreWithinSegment(j, i))
					newCellImage.color = oddSegmentCellColor;
				else
					if (gridSegments[0].CellCoordsAreWithinSegment(j, i) || gridSegments[2].CellCoordsAreWithinSegment(j, i))
					newCellImage.color = evenSegmentCellColor;
				else
					newCellImage.color = nonSegmentCellColor;

				RectTransform newCellImageTransform = newCellImage.GetComponent<RectTransform>();
				newCellImageTransform.sizeDelta = new Vector2(cellSize, cellSize);
				newCellImageTransform.GetComponent<RectTransform>().anchoredPosition = new Vector3(j * cellSize, i * cellSize);*/
			}
		gridReady = true;
	}

	void ClearGrid()
	{
		foreach (Cell cell in cells)
			cell.EmptyCell();
	}

	public Cell GetCell(int cellX, int cellY)
	{
		if (cellX < 0 | cellY < 0 | cellX >= _gridHorSize | cellY >= _gridVertSize)
			return null;
		else
			return cells[cellX, cellY];
	}

	public bool CellExistsIsUnoccupied(Vector2 cellCoords)
	{
		return CellExistsIsUnoccupied((int)cellCoords.x, (int)cellCoords.y);
	}

	public bool CellExistsIsUnoccupied(int cellX, int cellY)
	{
		//print("Checking cell "+cellX+","+cellY);
		if (cellX >= 0 && cellX < _gridHorSize)
			if (cellY >= 0 && cellY < _gridVertSize)
				return cells[cellX, cellY].isUnoccupied;

		return false;
	}

	public Vector3 GetCellWorldPosition(int cellX, int cellY)
	{
		return gridGroup.position + (Vector3)GetCellPositionInGrid(cellX, cellY);
	}

	public Vector2 GetCellPositionInGrid(int cellX, int cellY)
	{
		if (cellX < 0 | cellY < 0 | cellX >= _gridHorSize | cellY >= _gridVertSize)
			Debug.LogFormat("Getting world position of cell ({0},{1}) which does not exist!", cellX, cellY);

		return new Vector2(cellX * cellSize, cellY * cellSize);
	}

	public int GetSegmentIndexFromCoords(int x, int y)
	{
		GridSegment segment = GetSegmentFromCoords(x,y);

		return (segment != null) ? segment.segmentIndex: -1;
	}

	public GridSegment GetSegmentFromCoords(int x, int y)
	{
		foreach (GridSegment segment in gridSegments)
			if (segment.CellCoordsAreWithinSegment(x, y))
				return segment;

		return null;
	}

	Grid() { }

}
