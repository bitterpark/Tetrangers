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

	public IEnumerator HandleFilledUpRows(List<int> checkedRowNumbers)
	{
		return Grid.Instance.ClearFilledUpRows(CheckForFilledUpRows(checkedRowNumbers));
	}

	List<List<Cell>> CheckForFilledUpRows(List<int> checkedRowNumbers)
	{

		List<List<Cell>> result = new List<List<Cell>>();

		foreach (GridSegment segment in Grid.Instance.GridSegments)
		{
			foreach (int rowNum in checkedRowNumbers)
			{
				bool rowFilledUp = true;
				List<Cell> filledRowCells = new List<Cell>();
				for (int i = segment.minX; i <= segment.maxX; i++)
				{
					Cell checkedCell = Grid.Instance.GetCell(i, rowNum);
					if (checkedCell.isUnoccupied)
					{
						rowFilledUp = false;
						break;
					}
					else filledRowCells.Add(checkedCell);
				}

				if (rowFilledUp)
					result.Add(filledRowCells);
			}
		}

		return result;
	}
}

