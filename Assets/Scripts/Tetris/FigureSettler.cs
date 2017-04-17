using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FigureSettler
{
	public static event UnityAction<Rect> ENewFigureSettled;

	Matcher matcher;

	public FigureSettler(Matcher gridMatcher)
	{
		matcher = gridMatcher;
		FigureController.EFigureSettled += NewSettledFigureHandler;
	}

	void NewSettledFigureHandler(List<SettledBlock> figureBlocks)
	{
		Grid.Instance.StartCoroutine(NewSettledFigureRoutine(figureBlocks));
	}

	IEnumerator NewSettledFigureRoutine(List<SettledBlock> figureBlocks)
	{
		//Consider consolidating figureDimensions and GetRowsAndCols
		SettledFigureInfo figureInfo = FillInSettledFigure(figureBlocks);

		//Might be some doubling here if a row gets cleared and then gets checked for matches anyway
		List<int> figureRows;
		List<int> figureCols;
		GetSettledFigureRowsAndCols(figureBlocks, out figureRows, out figureCols);

		matcher.HandleFilledUpRows(figureRows);
		//
		IEnumerator matchesRoutine = matcher.HandleMatches(figureRows, figureCols);
		if (matchesRoutine != null)
			yield return Grid.Instance.StartCoroutine(matchesRoutine);
		//
		/*
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
			if (CanLowerBlockInCell(cell.xCoord, cell.yCoord))
			{
				LowerBlockInCell(cell.xCoord, cell.yCoord, true).LaunchInParallelCoroutinesGroup(this, "Figure blocks lowering");
				//yield return StartCoroutine(LowerBlockInCell(cell.xCoord, cell.yCoord, true));
			}
		while (CoroutineExtension.GroupIsProcessing("Figure blocks lowering"))
			yield return null;
		*/
		if (ENewFigureSettled != null) ENewFigureSettled(figureInfo.dimensions);
		yield break;
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
		settledFigureDimensions.min = new Vector2(100, 100);
		settledFigureDimensions.max = new Vector2(-1, -1);
		//bool overStacked = false;
		List<Cell> filledCells = new List<Cell>();
		foreach (SettledBlock block in settledBlocks)
		{
			int xCoord = block.currentX;
			int yCoord = block.currentY;

			if (xCoord < 0 | yCoord < 0 | xCoord >= Grid.Instance.gridHorSize | yCoord >= Grid.Instance.gridVertSize)
				Debug.LogErrorFormat(Grid.Instance, "Filling in cell ({0}, {1}) which does not exist!", xCoord, yCoord);

			settledFigureDimensions.xMin = Mathf.Min(settledFigureDimensions.xMin, xCoord);
			settledFigureDimensions.yMin = Mathf.Min(settledFigureDimensions.yMin, yCoord);
			settledFigureDimensions.xMax = Mathf.Max(settledFigureDimensions.xMax, xCoord);
			settledFigureDimensions.yMax = Mathf.Max(settledFigureDimensions.yMax, yCoord);

			Cell filledCell = Grid.Instance.GetCell(xCoord, yCoord);
			filledCell.FillCell(block, false);
			filledCells.Add(filledCell);

		}


		return new SettledFigureInfo(settledFigureDimensions, filledCells);
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
}

