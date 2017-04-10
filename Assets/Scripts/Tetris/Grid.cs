using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Grid : Singleton<Grid> 
{
	//public static event UnityAction EOverstacked;
	public static event UnityAction<Rect> ENewFigureSettled;
	public static event UnityAction<int> ERowsCleared;
	public delegate PlayerShipModel.TotalEnergyGain BlocksClearDeleg(int blueBlocks, int greenBlocks, int shieldBlocks);
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

	[SerializeField]
	int gridVertSize = 20;
	[SerializeField]
	int gridHorSize = 20;

	public int maxX
	{
		get { return gridHorSize - 1;}
	}
	public int maxY
	{
		get { return gridVertSize - 1; }
	}

	public int maxYAllowedForSettling
	{
		get { return maxY; }
	}

	const int minMatchSize = 3;

	//bool[,] unoccupiedCells;
	Cell[,] cells;

	void Awake()
	{
		TetrisManager.ETetrisEndClear += ClearGrid;
		FigureController.EFigureSettled += NewSettledFigureHandler;
		SettledBlock.EBlockDespawnedFromCell += ClearCellAtCoords;
		StartCoroutine(WaitForCanvasToSetupRoutine());
	}

	IEnumerator WaitForCanvasToSetupRoutine()
	{
		yield return new WaitForFixedUpdate();
		CreateGrid();
	}

	void CreateGrid()
	{
		cellSize = _gridGroup.GetComponent<RectTransform>().rect.width / gridHorSize;

		cells = new Cell[gridHorSize, gridVertSize];

		for (int i = 0; i < gridVertSize; i++)
			for (int j = 0; j < gridHorSize; j++)
			{
				cells[j, i] = new Cell(j,i);
				Image newCellImage = Instantiate(gridCellImage);
				newCellImage.transform.SetParent(_gridGroup, false);
                RectTransform newCellImageTransform = newCellImage.GetComponent<RectTransform>();
                newCellImageTransform.sizeDelta = new Vector2(cellSize,cellSize);
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
		if (cellX < 0 | cellY < 0 | cellX >= gridHorSize | cellY >= gridVertSize)
			Debug.LogFormat("Getting cell ({0},{1}) which does not exist!",cellX,cellY);

		return cells[cellX, cellY];
	}

    public bool CellExistsIsUnoccupied(Vector2 cellCoords)
    {
        return CellExistsIsUnoccupied((int)cellCoords.x, (int)cellCoords.y);
    }

	public bool CellExistsIsUnoccupied(int cellX, int cellY)
	{
		//print("Checking cell "+cellX+","+cellY);
		if (cellX >= 0 && cellX < gridHorSize)
			if (cellY >= 0 && cellY < gridVertSize)
				return cells[cellX, cellY].isUnoccupied;

		return false;
	}

	Vector3 GetCellWorldPosition(int cellX, int cellY)
	{
		return gridGroup.position + (Vector3)GetCellPositionInGrid(cellX, cellY);
	}

	public Vector2 GetCellPositionInGrid(int cellX, int cellY)
	{
		if (cellX < 0 | cellY < 0 | cellX >= gridHorSize | cellY >= gridVertSize)
			Debug.LogFormat("Getting world position of cell ({0},{1}) which does not exist!", cellX, cellY);

		return new Vector2(cellX*cellSize, cellY*cellSize);
	}

	

	void NewSettledFigureHandler(List<SettledBlock> figureBlocks)
	{
		//Consider consolidating figureDimensions and GetRowsAndCols
		SettledFigureInfo figureInfo = FillInSettledFigure(figureBlocks);

		//Might be some doubling here if a row gets cleared and then gets checked for matches anyway
		List<int> figureRows;
		List<int> figureCols;
		GetSettledFigureRowsAndCols(figureBlocks, out figureRows, out figureCols);

		HandleFilledUpRows(figureRows);
		HandleMatches(figureRows,figureCols);
		if (ENewFigureSettled != null) ENewFigureSettled(figureInfo.dimensions);

		//Lower any remaining parts of the figure floating in the air
		List<Cell> settledFigureCells = figureInfo.settledBlockCells;

		System.Comparison<Cell> bottomCellsFirstOrderer = (Cell cell1, Cell cell2) =>
		{
			if (cell1.yCoord < cell2.yCoord)
				return -1;
			if (cell1.yCoord > cell2.yCoord)
				return 1;

			return 0;
		};

		settledFigureCells.Sort(bottomCellsFirstOrderer);
		foreach (Cell cell in figureInfo.settledBlockCells)
			TryLowerBlockInCell(cell.xCoord,cell.yCoord,true);
	}

	struct SettledFigureInfo
	{
		public Rect dimensions;
		public List<Cell> settledBlockCells;

		public SettledFigureInfo(Rect dimensions, List<Cell> settledBlockCells)
		{
			this.dimensions = dimensions;
			this.settledBlockCells = settledBlockCells;
		}
	}

	SettledFigureInfo FillInSettledFigure(List<SettledBlock> settledBlocks)
	{
		Rect settledFigureDimensions = new Rect();
		settledFigureDimensions.min = new Vector2(100,100);
		settledFigureDimensions.max = new Vector2(-1,-1);
		//bool overStacked = false;
		List<Cell> filledCells=new List<Cell>();
		foreach (SettledBlock block in settledBlocks)
		{
			int xCoord = block.currentX;
			int yCoord = block.currentY;
			
			if (xCoord < 0 | yCoord < 0 | xCoord >= gridHorSize | yCoord >= gridVertSize)
				Debug.LogErrorFormat(this, "Filling in cell ({0}, {1}) which does not exist!",xCoord,yCoord);

			settledFigureDimensions.xMin = Mathf.Min(settledFigureDimensions.xMin, xCoord);
			settledFigureDimensions.yMin = Mathf.Min(settledFigureDimensions.yMin, yCoord);
			settledFigureDimensions.xMax = Mathf.Max(settledFigureDimensions.xMax, xCoord);
			settledFigureDimensions.yMax = Mathf.Max(settledFigureDimensions.yMax, yCoord);

			Cell filledCell = cells[xCoord, yCoord];
			filledCell.FillCell(block, false);
			filledCells.Add(filledCell);

		}
		

		return new SettledFigureInfo(settledFigureDimensions,filledCells);
	}

	void GetSettledFigureRowsAndCols(List<SettledBlock> settledBlocks, out List<int> rowNumbers, out List<int> colNumbers)
	{
		rowNumbers = new List<int>();
		colNumbers = new List<int>();
		foreach (SettledBlock block in settledBlocks)
		{
			int rowNum = block.currentY;
			int colNum = block.currentX;
			if (!rowNumbers.Contains(rowNum))
				rowNumbers.Add(rowNum);
			if (!colNumbers.Contains(colNum))
				colNumbers.Add(colNum);
		}
	}

	void HandleMatches(List<int> checkedRows, List<int> checkedCols)
	{
		List<Cell> allMatches = FindHorizontalMatches(checkedRows, minMatchSize);
		allMatches.AddRange(FindVerticalMatches(checkedCols,minMatchSize));

		ClearMatches(allMatches);
	}


	//OPTIMIZE THIS A BIT LATER??
	List<Cell> FindHorizontalMatches(List<int> checkedRowNumbers, int minimalMatchCount)
	{
		//checkedRowNumbers.Sort();
		//checkedRowNumbers.Reverse();
		List<Cell> matchingBlockCells = new List<Cell>();
		foreach (int rowNum in checkedRowNumbers)
		{
			BlockType cursorBlockType = BlockType.Blue;
			List<Cell> matchingCellsFoundSoFar = new List<Cell>();
			for (int i = 0; i < gridHorSize; i++)
			{
				Cell exploredCell = cells[i, rowNum];
				
				if (exploredCell.isUnoccupied)
				{
					if (matchingCellsFoundSoFar.Count >= minimalMatchCount)
					{
						if (matchingCellsFoundSoFar.Count >= minimalMatchCount)
							matchingBlockCells.AddRange(matchingCellsFoundSoFar.ToArray());
					}
					matchingCellsFoundSoFar.Clear();
					cursorBlockType = BlockType.Blue;
				}
				else
				{
					if (exploredCell.settledBlockInCell.blockType != cursorBlockType && exploredCell.settledBlockInCell.blockType != BlockType.Powerup)
					{
						if (matchingCellsFoundSoFar.Count >= minimalMatchCount)
								matchingBlockCells.AddRange(matchingCellsFoundSoFar.ToArray());

						Cell previousCell = i > 0 ? cells[i-1, rowNum] : null;

						matchingCellsFoundSoFar.Clear();
						if (previousCell != null && !previousCell.isUnoccupied && previousCell.settledBlockInCell.blockType == BlockType.Powerup)
							matchingCellsFoundSoFar.Add(previousCell);
					}

					if (exploredCell.settledBlockInCell.blockType != BlockType.Powerup)
						cursorBlockType = exploredCell.settledBlockInCell.blockType;

					matchingCellsFoundSoFar.Add(exploredCell);
				}
			}
			if (matchingCellsFoundSoFar.Count >= minimalMatchCount)
				matchingBlockCells.AddRange(matchingCellsFoundSoFar.ToArray());
		}

		return matchingBlockCells;
	}

	void ClearMatches(List<Cell> matchingBlockCells)
	{
		if (matchingBlockCells.Count > 0)
		{
			ClearCells(matchingBlockCells);
			//foreach (Cell matchingBlockCell in matchingBlockCells)
				//ClearCell(matchingBlockCell);
		}
	}

	List<Cell> FindVerticalMatches(List<int> checkedColNumbers, int minimalMatchCount)
	{
		//checkedRowNumbers.Sort();
		//checkedRowNumbers.Reverse();
		List<Cell> matchingBlockCells = new List<Cell>();
		foreach (int colNum in checkedColNumbers)
		{
			BlockType cursorBlockType = BlockType.Blue;
			List<Cell> matchingCellsFoundSoFar = new List<Cell>();
			//Watch out, this might require to go top-down and not bottom-up
			for (int i = 0; i < gridVertSize; i++)
			{
				Cell exploredCell = cells[colNum, i];

				if (exploredCell.isUnoccupied)
				{
					if (matchingCellsFoundSoFar.Count >= minimalMatchCount)
					{
						if (matchingCellsFoundSoFar.Count >= minimalMatchCount)
							matchingBlockCells.AddRange(matchingCellsFoundSoFar.ToArray());
					}
					matchingCellsFoundSoFar.Clear();
					cursorBlockType = BlockType.Blue;
				}
				else
				{
					if (exploredCell.settledBlockInCell.blockType != cursorBlockType && exploredCell.settledBlockInCell.blockType != BlockType.Powerup)
					{
						if (matchingCellsFoundSoFar.Count >= minimalMatchCount)
							matchingBlockCells.AddRange(matchingCellsFoundSoFar.ToArray());

						Cell previousCell = i > 0 ? cells[colNum, i-1] : null;

						matchingCellsFoundSoFar.Clear();
						if (previousCell != null && !previousCell.isUnoccupied && previousCell.settledBlockInCell.blockType == BlockType.Powerup)
							matchingCellsFoundSoFar.Add(previousCell);
					}

					if (exploredCell.settledBlockInCell.blockType != BlockType.Powerup)
						cursorBlockType = exploredCell.settledBlockInCell.blockType;

					matchingCellsFoundSoFar.Add(exploredCell);
				}
			}
			if (matchingCellsFoundSoFar.Count >= minimalMatchCount)
				matchingBlockCells.AddRange(matchingCellsFoundSoFar.ToArray());
		}

		return matchingBlockCells;
	}

	void HandleFilledUpRows(List<int> checkedRowNumbers)
	{
		ClearFilledUpRows(CheckForFilledUpRows(checkedRowNumbers));
	}

	List<int> CheckForFilledUpRows(List<int> checkedRowNumbers)
	{
		/*
		List<int> checkedRowNumbers = new List<int>();
		foreach (SettledBlock block in settledBlocks)
		{
			int rowNum = block.currentY;
			if (!checkedRowNumbers.Contains(rowNum))
				checkedRowNumbers.Add(rowNum);
		}*/

		List <int> result = new List<int>();

		foreach (int rowNum in checkedRowNumbers)
		{
			bool rowFilledUp = true;
			for (int i = 0; i < gridHorSize; i++)
				if (cells[i, rowNum].isUnoccupied)
				{
					rowFilledUp = false;
					break;
				}
			if (rowFilledUp)
				result.Add(rowNum);
		}

		return result;
	}

	void ClearFilledUpRows(List<int> filledUpRows)
	{
		if (filledUpRows.Count > 0)
		{
			ClearRows(filledUpRows);

			if (ERowsCleared != null) ERowsCleared(filledUpRows.Count);
		}
	}



	public void ClearBottomRows(int numberOfRows)
	{
		List<int> rowIndeces = new List<int>();

		for (int i = 0; i < numberOfRows; i++)
			rowIndeces.Add(i);

		ClearRows(rowIndeces);
	}

	void ClearRows(params int[] rows)
	{
		ClearRows(new List<int>(rows));
	}

	void ClearRows(List<int> rows)
	{
		rows.Sort();

		for (int i = rows.Count - 1; i >= 0; i--)
			ClearArea(0, rows[i], maxX, rows[i]);

	}

	public void ClearArea(int leftX, int bottomY, int rightX, int topY)
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

		ClearCells(clearedCells);
	}

	public void ClearCellAtCoords(int xCoord, int yCoord)
	{
		ClearCells(GetCell(xCoord,yCoord));
	}

	void ClearCells(params Cell[] clearedCells)
	{
		ClearCells(new List<Cell>(clearedCells));
	}

	void ClearCells(List<Cell> clearedCells)
	{
		//This should ensure that the cells at the top always go first
		System.Comparison<Cell> topLeftCellsFirstOrderer = (Cell cell1, Cell cell2) => 
		{
			if (cell1.yCoord > cell2.yCoord)
				return -1;
			if (cell1.yCoord < cell2.yCoord)
				return 1;
			if (cell1.yCoord==cell2.yCoord)
			{
				if (cell1.xCoord < cell2.xCoord)
					return -1;
				if (cell1.xCoord > cell2.xCoord)
					return 1;
			}
			return 0;	
		};
		clearedCells.Sort(topLeftCellsFirstOrderer);

		
		int greenBlocksCleared = 0;
		int blueBlocksCleared = 0;
		int shieldBlocksCleared = 0;
		List<int> newlyLoweredCellRows = new List<int>();
		List<int> newlyLoweredCellCols = new List<int>();
		foreach (Cell cell in clearedCells)
		{
			if (!cell.isUnoccupied)
			{
				SettledBlock blockInCell = cell.settledBlockInCell;
				if (blockInCell.blockType == BlockType.Blue)
					blueBlocksCleared++;
				else if (blockInCell.blockType == BlockType.Green)
					greenBlocksCleared++;
				else if (blockInCell.blockType == BlockType.Shield)
					shieldBlocksCleared++;

				cell.ClearCell();
				for (int k = cell.yCoord + 1; k < maxY; k++)
					if (TryLowerBlockInCell(cell.xCoord, k, true))
					{
						if (!newlyLoweredCellRows.Contains(k - 1))
							newlyLoweredCellRows.Add(k - 1);
						if (!newlyLoweredCellCols.Contains(cell.xCoord))
							newlyLoweredCellCols.Add(cell.xCoord);
					}
			}
		}

		Cell middleCell = clearedCells[Mathf.Clamp(clearedCells.Count/2,0,clearedCells.Count-1)];
		Vector3 middleCellPos = GetCellWorldPosition(middleCell.xCoord, middleCell.yCoord);

		if (EBlocksCleared != null)
		{
			PlayerShipModel.TotalEnergyGain totalGain = EBlocksCleared(blueBlocksCleared, greenBlocksCleared, shieldBlocksCleared);
			TryCreateEnergyGainText(totalGain.blueGain, Color.cyan, middleCellPos);
			TryCreateEnergyGainText(totalGain.greenGain, Color.green, middleCellPos);
			TryCreateEnergyGainText(totalGain.shieldGain, Color.blue, middleCellPos);
		}

		HandleMatches(newlyLoweredCellRows, newlyLoweredCellCols);
	}


	/*System.Comparison<Cell> cellSortComparer = (Cell cell1, Cell cell2) => 
		{
			if (cell1.yCoord > cell2.yCoord)
				return -1;
			if (cell1.yCoord < cell2.yCoord)
				return 1;
			if (cell1.yCoord==cell2.yCoord)
			{
				if (cell1.xCoord < cell2.xCoord)
					return -1;
				if (cell1.xCoord > cell2.xCoord)
					return 1;
			}
			return 0;	
		};*/
	/*
	bool TryFallDownBlockInCell(int cellX, int cellY)
	{
		Cell fallingCell = cells[cellX, cellY];
		if (fallingCell.isUnoccupied) return false;

		bool cellFell = false;
		while (TryLowerBlockInCell(fallingCell.xCoord, fallingCell.yCoord, 1)) { cellFell = true; }
		return cellFell;
	}*/

	bool TryLowerBlockInCell(int cellX, int cellY, bool lowerAllTheWay)
	{
		if (cellY - 1 < 0) return false;

		Cell startCell = cells[cellX, cellY];
		Cell endCell = cells[cellX, cellY - 1];
		if (!startCell.isUnoccupied && endCell.isUnoccupied)
		{
			SettledBlock block = startCell.ExtractBlockFromCell();
			block.MoveToGridCell(endCell.xCoord, endCell.yCoord);
			endCell.FillCell(block, true);
			if (lowerAllTheWay)
				TryLowerBlockInCell(endCell.xCoord,endCell.yCoord,true);

			return true;
		}
		else
			return false;
	}

	void TryCreateEnergyGainText(int gain, Color color, Vector3 worldPosition)
	{
		//Debug.Log("Creating energy gain text");
		worldPosition.x += Random.Range(-20, 21);
		if (gain>0)
			FloatingText.CreateFloatingText(gain.ToString(), color, 40, 1.5f, gridGroup, worldPosition);
	}

	/*
	void CreateGreenEnergyGainText(int gain)
	{
		PlayerShipModel.EPlayerGainedGreenEnergy -= CreateGreenEnergyGainText;
		FloatingText.CreateFloatingText(gain.ToString(), Color.green, 40, 1.5f, gridGroup);
	}*/

	Grid() { }

}
