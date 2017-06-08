using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FigureSettler
{
	public static event UnityAction ENewFigureSettled;
	public static event UnityAction<List<IEnumerator>> ETogglePowerupEffects;
	public static event UnityAction<int> EOverflowingBlocks;

	Matcher matcher;

	public FigureSettler(Matcher gridMatcher)
	{
		matcher = gridMatcher;
		FigureController.EFigureSettled += NewSettledFigureHandler;
		BattleManager.EBattleFinished += Clear;
	}

	public void Clear()
	{
		ETogglePowerupEffects = null;
	}

	void NewSettledFigureHandler(List<SettledBlock> figureBlocks)
	{
		Grid.Instance.StartCoroutine(NewSettledFigureRoutine(figureBlocks));
	}

	IEnumerator NewSettledFigureRoutine(List<SettledBlock> figureBlocks)
	{
		FillInSettledFigure(figureBlocks);

		//Might be some doubling here if a row gets cleared and then gets checked for matches anyway
		List<int> figureRows;
		List<int> figureCols;
		GetSettledFigureRowsAndCols(figureBlocks, out figureRows, out figureCols);

		yield return Grid.Instance.StartCoroutine(matcher.HandleFilledUpRows(figureRows));
		//
		IEnumerator matchesRoutine = matcher.HandleMatches(figureRows, figureCols);
		if (matchesRoutine != null)
			yield return Grid.Instance.StartCoroutine(matchesRoutine);

		List<IEnumerator> powerupEffectRoutines = new List<IEnumerator>();
		while (ETogglePowerupEffects != null)
		{
			ETogglePowerupEffects(powerupEffectRoutines);
			foreach (IEnumerator routine in powerupEffectRoutines)
				yield return Grid.Instance.StartCoroutine(routine);
		}

		HandleOverflowingBlocks(figureBlocks);
		//Grid.Instance.UpdateIsolatedBlocks();
		if (ENewFigureSettled != null) ENewFigureSettled();
		yield break;
	}

	void FillInSettledFigure(List<SettledBlock> settledBlocks)
	{
		//bool overStacked = false;
		foreach (SettledBlock block in settledBlocks)
		{
			int xCoord = block.currentX;
			int yCoord = block.currentY;

			if (xCoord < 0 | yCoord < 0 | xCoord >= Grid.Instance.gridHorSize | yCoord >= Grid.Instance.gridVertSize)
				Debug.LogErrorFormat(Grid.Instance, "Filling in cell ({0}, {1}) which does not exist!", xCoord, yCoord);

			Cell filledCell = Grid.Instance.GetCell(xCoord, yCoord);
			filledCell.FillCell(block, false);

		}
	}

	void GetSettledFigureRowsAndCols(List<SettledBlock> correctlySettledBlocks, out List<int> rowNumbers, out List<int> colNumbers)
	{
		rowNumbers = new List<int>();
		colNumbers = new List<int>();
		foreach (SettledBlock block in correctlySettledBlocks)
		{
			int rowNum = block.currentY;
			int colNum = block.currentX;
			if (!rowNumbers.Contains(rowNum))
				rowNumbers.Add(rowNum);
			if (!colNumbers.Contains(colNum))
				colNumbers.Add(colNum);
		}
	}

	void HandleOverflowingBlocks(List<SettledBlock> settledFigureBlocks)
	{
		int unmatchedOverflowingBlockCount = 0;
		foreach (SettledBlock block in settledFigureBlocks)
		{
			if (block != null)
			{
				bool blockIsOverflowing = true;
				foreach (GridSegment segment in Grid.Instance.GridSegments)
				{
					if (segment.isUsable && segment.CellCoordsAreWithinSegment(block.currentX, block.currentY))
					{
						blockIsOverflowing = false;
						break;
					}
				}
				if (blockIsOverflowing)
				{
					Cell blockCell = Grid.Instance.GetCell(block.currentX, block.currentY);
					blockCell.EmptyCell();
					//Debug.Log("Handling overflowing blocks!");
					unmatchedOverflowingBlockCount++;
				}
			}
		}
		if (unmatchedOverflowingBlockCount>0 && EOverflowingBlocks != null) EOverflowingBlocks(unmatchedOverflowingBlockCount);
	}


}

