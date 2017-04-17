using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;


public class Matcher
{

	

	const int minMatchSize = 3;

	public IEnumerator HandleMatches(List<int> checkedRows, List<int> checkedCols)
	{
		List<Cell> allMatches = FindHorizontalMatches(checkedRows, minMatchSize);
		allMatches.AddRange(FindVerticalMatches(checkedCols, minMatchSize));

		return ClearMatches(allMatches);
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
			for (int i = 0; i < Grid.Instance.gridHorSize; i++)
			{
				Cell exploredCell = Grid.Instance.GetCell(i,rowNum);//Grid.Instance.GetCell(i, rowNum);

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

						Cell previousCell = i > 0 ? Grid.Instance.GetCell(i - 1, rowNum) : null;

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

	IEnumerator ClearMatches(List<Cell> matchingBlockCells)
	{
		if (matchingBlockCells.Count > 0)
			return Grid.Instance.ClearCells(matchingBlockCells);

		return null;
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
			for (int i = 0; i < Grid.Instance.gridVertSize; i++)
			{
				Cell exploredCell = Grid.Instance.GetCell(colNum, i);

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

						Cell previousCell = i > 0 ? Grid.Instance.GetCell(colNum, i - 1) : null;

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

	public void HandleFilledUpRows(List<int> checkedRowNumbers)
	{
		Grid.Instance.ClearFilledUpRows(CheckForFilledUpRows(checkedRowNumbers));
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
			for (int i = 0; i < Grid.Instance.gridHorSize; i++)
				if (Grid.Instance.GetCell(i, rowNum).isUnoccupied)
				{
					rowFilledUp = false;
					break;
				}
			if (rowFilledUp)
				result.Add(rowNum);
		}

		return result;
	}
}

