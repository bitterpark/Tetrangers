using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Grid : Singleton<Grid>
{

	public static event UnityAction<int> ERowsCleared;
	public delegate PlayerShipModel.TotalEnergyGain BlocksClearDeleg(int blueBlocks, int greenBlocks, int shieldBlocks, int shipBlocks);
	public static event BlocksClearDeleg EBlocksCleared;

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

	const int minMatchSize = 3;

	//bool[,] unoccupiedCells;
	Cell[,] cells;

	GridFX gridFX;

	public Matcher matcher { get; private set; }
	FigureSettler settler;

	public GridSegment[] GridSegments { get { return gridSegments; } }
	GridSegment[] gridSegments;

	public static readonly int segmentCount = 3;
	public static readonly int segmentWidth = 9;

	void Awake()
	{
		matcher = new Matcher();
		settler = new FigureSettler(matcher);
		gridFX = new GridFX(gridGroup);

		TetrisManager.ETetrisEndClear += ClearGrid;
		SettledBlock.EBlockDespawnedFromCell += ClearCellAtCoords;
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
				Image newCellImage = Instantiate(gridCellImage);
				newCellImage.transform.SetParent(_gridGroup, false);

				if (gridSegments[1].CellCoordsAreWithinSegment(j, i))
					newCellImage.color = Color.gray;

				RectTransform newCellImageTransform = newCellImage.GetComponent<RectTransform>();
				newCellImageTransform.sizeDelta = new Vector2(cellSize, cellSize);
				newCellImageTransform.GetComponent<RectTransform>().anchoredPosition = new Vector3(j * cellSize, i * cellSize);
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


	public IEnumerator ClearFilledUpRows(List<List<Cell>> filledUpRows)
	{
		if (filledUpRows.Count > 0)
		{
			int coroutineGroupIndex = CoroutineExtension.GetLatestAvailableIndex();
			foreach (List<Cell> cellsInRow in filledUpRows)
				ClearCells(cellsInRow).LaunchInParallelCoroutinesGroup(this, coroutineGroupIndex.ToString());

			while (CoroutineExtension.GroupIsProcessing(coroutineGroupIndex.ToString()))
				yield return null;

			if (ERowsCleared != null) ERowsCleared(filledUpRows.Count);
		}
		yield break;
	}



	public void ClearBottomRows(int numberOfRows)
	{
		for (int i = numberOfRows - 1; i >= 0; i--)
			ClearArea(0, i, maxX, i, false);

		List<int> possibleMatchesInRows = new List<int>();
		for (int i = 0; i <= maxX; i++)
			for (int j = numberOfRows; j <= maxSegmentY; j++)
			{
				if (CanLowerBlockInCell(i, j))
				{
					for (int tries = 0; tries < numberOfRows; tries++)
						StartCoroutine(LowerBlockInCell(i, j - tries, false));

					if (!possibleMatchesInRows.Contains(j - numberOfRows))
						possibleMatchesInRows.Add(j - numberOfRows);
				}


			}
		matcher.HandleMatches(possibleMatchesInRows,new List<int>());
	}

	public void ClearRows(params int[] rows)
	{
		ClearRows(new List<int>(rows));
	}

	public void ClearRows(List<int> rows)
	{
		rows.Sort();

		for (int i = rows.Count - 1; i >= 0; i--)
			ClearArea(0, rows[i], maxX, rows[i]);

	}

	//Update this with coroutines later?
	public void ClearArea(int leftX, int bottomY, int rightX, int topY)
	{
		ClearArea(leftX, bottomY, rightX, topY, false); 
	}
	public void ClearArea(int leftX, int bottomY, int rightX, int topY, bool lowerAboveCells)
	{
		Debug.AssertFormat(leftX >= 0 && leftX<= maxX 
			&& bottomY>=0 && bottomY<= maxY
			&& rightX>=0 && rightX<= maxX
			&& topY>=0 && topY<= maxY
			, "Area clear out of grid bounds at ({0},{1} - {2},{3})", leftX, bottomY, rightX, topY);

		//Goes top left to bottom right
		//List<int> rowsWithNewlyLoweredCells = new List<int>();
		List<Cell> clearedCells = new List<Cell>();

		int i = 0;
		int j = 0;

		for (i = topY; i >= bottomY; i--)
			for (j = leftX; j <= rightX; j++)
				clearedCells.Add(cells[j, i]);

		StartCoroutine(ClearCells(clearedCells, lowerAboveCells));
	}

	public void ClearCellAtCoords(int xCoord, int yCoord)
	{
		StartCoroutine(ClearCells(GetCell(xCoord,yCoord)));
	}

	public IEnumerator ClearCells(params Cell[] clearedCells)
	{
		return ClearCells(false, clearedCells);
	}
	public IEnumerator ClearCells(bool lower, params Cell[] clearedCells)
	{
		return ClearCells(new List<Cell>(clearedCells), lower);
	}
	public IEnumerator ClearCells(List<Cell> clearedCells)
	{
		return ClearCells(clearedCells, false);
	}

	public IEnumerator ClearCells(List<Cell> clearedCells, bool lower)
	{
		//This should ensure that the cells at the top always go first
		System.Comparison<Cell> topLeftCellsFirstOrderer = (Cell cell1, Cell cell2) => 
		{
			if (cell1.yCoord > cell2.yCoord)
				return -1;
			if (cell1.yCoord < cell2.yCoord)
				return 1;
			if (cell1.yCoord == cell2.yCoord)
			{
				if (cell1.xCoord < cell2.xCoord)
					return -1;
				if (cell1.xCoord > cell2.xCoord)
					return 1;
			}
			return 0;	
		};
		clearedCells.Sort(topLeftCellsFirstOrderer);

		Dictionary<GridSegment, GridSegment.ClearedCellsInfo> clearInfo = new Dictionary<GridSegment, GridSegment.ClearedCellsInfo>();
		foreach (GridSegment segment in gridSegments)
			clearInfo.Add(segment,new GridSegment.ClearedCellsInfo());

		GridSegment.ClearedCellsInfo globalClearInfo = new GridSegment.ClearedCellsInfo();

		List<SettledBlock> loweredBlocks = new List<SettledBlock>();
		int coroutineGroupIndex = CoroutineExtension.GetLatestAvailableIndex();

		foreach (Cell cell in clearedCells)
		{
			if (!cell.isUnoccupied)
			{
				SettledBlock blockInCell = cell.settledBlockInCell;

				globalClearInfo.clearedCellsBelongingToSegment.Add(cell);

				foreach (GridSegment segment in gridSegments)
					if (segment.CellCoordsAreWithinSegment(cell.xCoord,cell.yCoord))
					{
						if (segment.isUsable)
						{
							GridSegment.ClearedCellsInfo segmentInfo = clearInfo[segment];
							segmentInfo.clearedCellsBelongingToSegment.Add(cell);

							if (blockInCell.blockType == BlockType.Blue)
							{
								segmentInfo.blueBlocksCount++;
								globalClearInfo.blueBlocksCount++;
							}
							else if (blockInCell.blockType == BlockType.Green)
							{
								segmentInfo.greenBlocksCount++;
								globalClearInfo.greenBlocksCount++;
							}
							else if (blockInCell.blockType == BlockType.Shield)
							{
								segmentInfo.shieldBlocksCount++;
								globalClearInfo.shieldBlocksCount++;
							}
							else if (blockInCell.blockType == BlockType.ShipEnergy)
							{
								segmentInfo.shipBlocksCount++;
								globalClearInfo.shipBlocksCount++;
							}
						}
						break;
					}

				cell.ClearCell();
				if (lower)
				{
					for (int k = cell.yCoord + 1; k < maxY; k++)
						if (CanLowerBlockInCell(cell.xCoord, k))
						{
							loweredBlocks.Add(blockInCell);
							//StartCoroutine(LowerBlockInCell(cell.xCoord, k, false));
							LowerBlockInCell(cell.xCoord, k, false).LaunchInParallelCoroutinesGroup(this, coroutineGroupIndex.ToString());
						}
				}
			}
		}

		while (CoroutineExtension.GroupIsProcessing(coroutineGroupIndex.ToString()))
			yield return null;

		List<int> newlyLoweredCellRows = new List<int>();
		List<int> newlyLoweredCellCols = new List<int>();
		foreach (SettledBlock block in loweredBlocks)
		{
			if (!newlyLoweredCellRows.Contains(block.currentY))
				newlyLoweredCellRows.Add(block.currentY);
			if (!newlyLoweredCellCols.Contains(block.currentX))
				newlyLoweredCellCols.Add(block.currentX);
		}

		foreach (GridSegment segment in gridSegments)
			segment.BroadcastBlockClears(clearInfo[segment]);
		
		if (EBlocksCleared != null)
		{
			PlayerShipModel.TotalEnergyGain totalGain = EBlocksCleared
				(
				globalClearInfo.blueBlocksCount
				, globalClearInfo.greenBlocksCount
				, globalClearInfo.shieldBlocksCount
				,globalClearInfo.shipBlocksCount);
			gridFX.ShowEnergyGainFX(clearedCells, totalGain);
		}

		IEnumerator matchesRoutine = matcher.HandleMatches(newlyLoweredCellRows, newlyLoweredCellCols);
		if (matchesRoutine != null)
			yield return StartCoroutine(matchesRoutine);
		yield break;
	}

	bool CanLowerBlockInCell(int cellX, int cellY)
	{
		//debug, change this later
		//return false;


		if (cellY - 1 < 0) return false;
		Cell startCell = cells[cellX, cellY];
		Cell endCell = cells[cellX, cellY - 1];
		if (!startCell.isUnoccupied && endCell.isUnoccupied)
			return true;
		else
			return false;
	}

	
	IEnumerator LowerBlockInCell(int cellX, int cellY, bool lowerAllTheWay)
	{

		Cell startCell = cells[cellX, cellY];
		Cell endCell = cells[cellX, cellY - 1];
		SettledBlock block = startCell.ExtractBlockFromCell();

		block.MoveToGridCell(endCell.xCoord, endCell.yCoord);
		//yield return StartCoroutine(block.AnimateMoveToGridCell(endCell.xCoord, endCell.yCoord));
		endCell.FillCell(block, true);
		if (lowerAllTheWay && CanLowerBlockInCell(endCell.xCoord, endCell.yCoord))
			yield return StartCoroutine(LowerBlockInCell(endCell.xCoord, endCell.yCoord, true));

		yield break;
	}


	Grid() { }

}
