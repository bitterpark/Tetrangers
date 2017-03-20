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

	public Vector2 GetCellPositionInGrid(int cellX, int cellY)
	{
		if (cellX < 0 | cellY < 0 | cellX >= gridHorSize | cellY >= gridVertSize)
			Debug.LogFormat("Getting world position of cell ({0},{1}) which does not exist!", cellX, cellY);

		return new Vector2(cellX*cellSize, cellY*cellSize);
	}

	void NewSettledFigureHandler(List<SettledBlock> figureBlocks)
	{
		Rect figureDimensions = FillInSettledFigure(figureBlocks);

		HandleFilledUpRows(CheckForFilledUpRows(figureBlocks));

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

			/*
			if (yCoord > maxYAllowedForSettling)
				overStacked = true;
			else
				cells[xCoord, yCoord].FillCell(block, false);*/
			cells[xCoord, yCoord].FillCell(block, false);
		}
		//if (overStacked && EOverstacked != null)
			//EOverstacked();

		return settledFigureDimensions;
	}

	List<int> CheckForFilledUpRows(List<SettledBlock> settledBlocks)
	{
		List<int> checkedRowNumbers = new List<int>();
		foreach (SettledBlock block in settledBlocks)
		{
			int rowNum = block.currentY;
			if (!checkedRowNumbers.Contains(rowNum))
				checkedRowNumbers.Add(rowNum);
		}
		
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

	void HandleFilledUpRows(List<int> filledUpRows)
	{
		if (filledUpRows.Count > 0)
		{
			int highestClearedRowNum = -1;
			foreach (int filledUpRowNum in filledUpRows)
			{
				if (filledUpRowNum > highestClearedRowNum)
					highestClearedRowNum = filledUpRowNum;
				ClearRow(filledUpRowNum);
			}
			if (ERowsCleared != null) ERowsCleared(filledUpRows.Count);

			if (highestClearedRowNum != -1)
			{
				for (int i = highestClearedRowNum + 1; i <= maxY; i++)
					LowerRow(i, filledUpRows.Count);
			}
		}
	}

	void ClearRow(int rowIndex)
	{
		ClearArea(0,rowIndex,maxX,rowIndex);
	}

	public void ClearArea(int startX, int startY, int endX, int endY)
	{
		Debug.AssertFormat(startX>=0 && startX<=maxX 
			&& startY>=0 && startY<=maxY
			&& endX>=0 && endX<=maxX
			&& endY>=0 && endY<=maxY
			,"Area clear out of grid bounds at ({0},{1} - {2},{3})",startX,startY,endX,endY);

		//Goes bottom left to top right
		for (int i = startY; i <= endY; i++)
			for (int j = startX; j <= endX; j++)
				ClearCell(j,i);

	}

	void ClearCell(int cellX, int cellY)
	{
		cells[cellX, cellY].ClearCell();
	}

	void LowerRow(int rowIndex, int lowerByN)
	{
		for (int i = 0; i <= maxX; i++)
		{
			SettledBlock block = cells[i, rowIndex].ExtractBlockFromCell();
			if (block!=null)
			{
				block.MoveToGridCell(i, rowIndex - lowerByN);
				cells[i, rowIndex-lowerByN].FillCell(block,true);
			}
		}
	}

	Grid() { }

}
