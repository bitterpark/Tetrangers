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
		Rect figureDimensions = FillInSettledFigure(figureBlocks);

		//Might be some doubling here if a row gets cleared and then gets checked for matches anyway
		List<int> figureRows = GetSettledFiguresRows(figureBlocks);

		HandleFilledUpRows(figureRows);
		HandleHorizontalMatches(figureRows);
		if (ENewFigureSettled != null) ENewFigureSettled(figureDimensions);
	}

	Rect FillInSettledFigure(List<SettledBlock> settledBlocks)
	{
		Rect settledFigureDimensions = new Rect();
		settledFigureDimensions.min = new Vector2(100,100);
		settledFigureDimensions.max = new Vector2(-1,-1);
		//bool overStacked = false;
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

			cells[xCoord, yCoord].FillCell(block, false);
		}
		return settledFigureDimensions;
	}

	List<int> GetSettledFiguresRows(List<SettledBlock> settledBlocks)
	{
		List<int> rowNumbers = new List<int>();
		foreach (SettledBlock block in settledBlocks)
		{
			int rowNum = block.currentY;
			if (!rowNumbers.Contains(rowNum))
				rowNumbers.Add(rowNum);
		}
		return rowNumbers;
	}

	void HandleHorizontalMatches(List<int> checkedRowNumbers)
	{
		ClearHorizontalMatches(FindHorizontalMatches(checkedRowNumbers, minMatchSize));
	}

	//OPTIMIZE THIS A BIT LATER??
	List<Cell> FindHorizontalMatches(List<int> checkedRowNumbers, int minimalMatchCount)
	{
		//checkedRowNumbers.Sort();
		//checkedRowNumbers.Reverse();
		List<Cell> matchingBlockCells = new List<Cell>();
		foreach (int rowNum in checkedRowNumbers)
		{
			int matchingSoFar = 0;
			BlockType cursorBlockType = BlockType.Blue;
			for (int i = 0; i < gridHorSize; i++)
			{
				Cell exploredCell = cells[i, rowNum];

				if (matchingSoFar == 0 || exploredCell.isUnoccupied || exploredCell.settledBlockInCell.blockType != cursorBlockType)
				{
					if (matchingSoFar >= minimalMatchCount)
						for (int j = 1; j <= matchingSoFar; j++)
							matchingBlockCells.Add(cells[i - j,rowNum]);

					if (exploredCell.isUnoccupied)
					{
						matchingSoFar = 0;
						cursorBlockType = BlockType.Blue;
					}
					else
					{
						cursorBlockType = exploredCell.settledBlockInCell.blockType;
						matchingSoFar = 1;
					}
				}
				else
					matchingSoFar++;
			}
			if (matchingSoFar >= minimalMatchCount)
				for (int j = 1; j <= matchingSoFar; j++)
					matchingBlockCells.Add(cells[gridHorSize - j, rowNum]);
		}

		return matchingBlockCells;
	}

	void ClearHorizontalMatches(List<Cell> matchingBlockCells)
	{
		if (matchingBlockCells.Count > 0)
		{
			ClearCells(matchingBlockCells);
			//foreach (Cell matchingBlockCell in matchingBlockCells)
				//ClearCell(matchingBlockCell);
		}
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
		
		List<int> result = new List<int>();

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
			/*
			filledUpRows.Sort();

			int highestClearedRowNum = -1;
			foreach (int filledUpRowNum in filledUpRows)
			{
				if (filledUpRowNum > highestClearedRowNum)
					highestClearedRowNum = filledUpRowNum;
				ClearRow(filledUpRowNum);
			}*/
			/*
			UnityEngine.Events.UnityAction<int> createFloatingTextHandler = null;
			createFloatingTextHandler = (int energyGain) =>
			{
				//GetCellWorldPosition(maxX / 2, highestClearedRowNum);
				CreateGreenEnergyGainText(energyGain);
				PlayerShipModel.EPlayerGainedGreenEnergy -= createFloatingTextHandler;
				//Debug.Log("Floating text handler fired!");
			};*/

			//PlayerShipModel.EPlayerGainedGreenEnergy += CreateGreenEnergyGainText;

			if (ERowsCleared != null) ERowsCleared(filledUpRows.Count);
			/*
			if (highestClearedRowNum != -1)
			{
				for (int i = highestClearedRowNum + 1; i <= maxY; i++)
					LowerRow(i, filledUpRows.Count);
			}*/
		}
	}

	void CreateGreenEnergyGainText(int gain)
	{
		PlayerShipModel.EPlayerGainedGreenEnergy -= CreateGreenEnergyGainText;
		FloatingText.CreateFloatingText(gain.ToString(), Color.green, 40, 1.5f, gridGroup);
	}

	public void ClearBottomRows(int numberOfRows)
	{
		List<int> rowIndeces = new List<int>();

		for (int i = 0; i < numberOfRows; i++)
			rowIndeces.Add(i);

		ClearRows(rowIndeces);
		//for (int i = numberOfRows-1; i >=0 ; i--)
			//ClearArea(0, i, maxX, i);
		//ClearRow(i);
		//for (int i = numberOfRows; i <= maxY; i++)
		//LowerRow(i, numberOfRows);
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
		//ClearRow(i);

	}
	/*
	void ClearRow(int rowIndex)
	{
		ClearArea(0,rowIndex,maxX,rowIndex);
	}*/

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

		/*
	{
		Cell cell = cells[j, i];
		cell.ClearCell();
		for (int k = cell.yCoord + 1; k < this.maxY; k++)
			if (TryLowerCell(cell.xCoord, k, 1))
				if (!rowsWithNewlyLoweredCells.Contains(k - 1))
					rowsWithNewlyLoweredCells.Add(k - 1);
	}*/
		ClearCells(clearedCells);
		//HandleHorizontalMatches(rowsWithNewlyLoweredCells);
	}

	void ClearCells(params Cell[] clearedCells)
	{
		ClearCells(new List<Cell>(clearedCells));
	}

	void ClearCells(List<Cell> clearedCells)
	{
		//This should ensure that the cells at the top always go first
		System.Comparison<Cell> cellSortComparer = (Cell cell1, Cell cell2) => 
		{
			if (cell1.yCoord > cell2.yCoord)
				return -1;
			if (cell1.yCoord < cell2.yCoord)
				return 1;
			return 0;	
		};
		clearedCells.Sort(cellSortComparer);

		List<int> rowsWithNewlyLoweredCells = new List<int>();
		foreach (Cell cell in clearedCells)
		{
			//Cell cell = cells[j, i];
			cell.ClearCell();
			for (int k = cell.yCoord + 1; k < maxY; k++)
				if (TryLowerCell(cell.xCoord, k, 1))
					if (!rowsWithNewlyLoweredCells.Contains(k - 1))
						rowsWithNewlyLoweredCells.Add(k - 1);
		}
		HandleHorizontalMatches(rowsWithNewlyLoweredCells);
	}

	/*
	void ClearCell(int cellX, int cellY)
	{
		ClearCell(cells[cellX,cellY]);
	}

	void ClearCell(Cell cell)
	{
		cell.ClearCell();
		for (int i = cell.yCoord + 1; i < maxY; i++)
			TryLowerCell(cell.xCoord, i, 1);
	}*/

	/*
	void LowerRow(int rowIndex, int lowerByN)
	{
		for (int i = 0; i <= maxX; i++)
		{
			LowerCell(i,rowIndex,lowerByN);

		}
	}*/

	bool TryLowerCell(int cellX, int cellY, int lowerByN)
	{
		SettledBlock block = cells[cellX, cellY].ExtractBlockFromCell();
		if (block != null)
		{
			block.MoveToGridCell(cellX, cellY - lowerByN);
			cells[cellX, cellY - lowerByN].FillCell(block, true);
			return true;
		}
		else
			return false;
	}

	Grid() { }

}
