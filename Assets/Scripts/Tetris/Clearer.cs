using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class Clearer : Singleton<Clearer>
{

	public static event UnityAction<int> ERowsCleared;
	public delegate PlayerShipModel.TotalEnergyGain BlocksClearDeleg(GridSegment.ClearedCellsInfo clearedCellsInfo);
	public static event BlocksClearDeleg EBlocksCleared;

	public static event UnityAction EToggleAllSettledBlocksSetToIsolated;
	public static event UnityAction<List<IEnumerator>> EExpireIsolatedBlocks;

	Matcher matcher;
	FigureSettler settler;
	GridFX gridFX;

	void Awake()
	{
		matcher = new Matcher();
		settler = new FigureSettler(matcher);
		gridFX = new GridFX(Grid.Instance.gridGroup);

		SettledBlock.EBlockDespawnedFromCell += ClearCellAtCoords;
	}

	public void EmptyBottomRowsInSegment(int numberOfRows, int segmentIndex)
	{
		StartCoroutine(EmptyBottomRowsInSegmentRoutine(numberOfRows, segmentIndex));
	}

	IEnumerator EmptyBottomRowsInSegmentRoutine(int numberOfRows, int segmentIndex)
	{
		int startX = Grid.Instance.GridSegments[segmentIndex].minX;
		int endX = Grid.Instance.GridSegments[segmentIndex].maxX;

		for (int i = numberOfRows - 1; i >= 0; i--)
			yield return StartCoroutine(ClearAreaRoutine(startX, i, endX, i, true));

		LowerAllSegmentRowsToBottom(numberOfRows, segmentIndex);
		yield break;
	}

	void LowerAllSegmentRowsToBottom(int bottomRowsRemoved, int segmentIndex)
	{
		int startX = Grid.Instance.GridSegments[segmentIndex].minX;
		int endX = Grid.Instance.GridSegments[segmentIndex].maxX;

		List<int> possibleMatchesInRows = new List<int>();
		for (int i = startX; i <= endX; i++)
			for (int j = bottomRowsRemoved; j <= Grid.Instance.maxSegmentY; j++)
			{
				bool loweredAtLeastOnce = false;
				for (int tries = 0; tries < bottomRowsRemoved; tries++)
					loweredAtLeastOnce = loweredAtLeastOnce | TryLowerBlockInCellByOne(i, j - tries);

				if (loweredAtLeastOnce && !possibleMatchesInRows.Contains(j - bottomRowsRemoved))
					possibleMatchesInRows.Add(j - bottomRowsRemoved);
			}
		matcher.HandleMatches(possibleMatchesInRows, new List<int>());
	}

	bool TryLowerBlockInCellByOne(int cellX, int cellY)
	{
		if (cellY - 1 < 0) return false;
		Cell startCell = Grid.Instance.GetCell(cellX, cellY);
		Cell endCell = Grid.Instance.GetCell(cellX, cellY - 1);
		if (!startCell.isUnoccupied && endCell.isUnoccupied)
		{
			SettledBlock block = startCell.ExtractBlockFromCell();
			block.MoveToGridCell(endCell.xCoord, endCell.yCoord);
			endCell.FillCell(block, true);
			return true;
		}
		return false;
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

	public IEnumerator ClearRowsRoutine(params int[] rows)
	{
		List<int> rowIndices = new List<int>(rows);

		rowIndices.Sort();

		for (int i = rowIndices.Count - 1; i >= 0; i--)
			yield return StartCoroutine(ClearAreaRoutine(0, rows[i], Grid.Instance.maxX, rows[i], false));

		yield break;
	}

	public void StartClearAreaRoutine(int leftX, int bottomY, int rightX, int topY)
	{
		StartCoroutine(ClearAreaRoutine(leftX, bottomY, rightX, topY, false));
	}
	public IEnumerator ClearAreaRoutine(int leftX, int bottomY, int rightX, int topY, bool giveEnergy)
	{
		Debug.AssertFormat(leftX >= 0 && leftX <= Grid.Instance.maxX
			&& bottomY >= 0 && bottomY <= Grid.Instance.maxY
			&& rightX >= 0 && rightX <= Grid.Instance.maxX
			&& topY >= 0 && topY <= Grid.Instance.maxY
			, "Area clear out of grid bounds at ({0},{1} - {2},{3})", leftX, bottomY, rightX, topY);

		//Goes top left to bottom right
		//List<int> rowsWithNewlyLoweredCells = new List<int>();
		List<Cell> clearedCells = new List<Cell>();

		for (int i = topY; i >= bottomY; i--)
			for (int j = leftX; j <= rightX; j++)
				clearedCells.Add(Grid.Instance.GetCell(j, i));

		yield return StartCoroutine(ClearCells(clearedCells, false));
	}

	public void ClearCellAtCoords(int xCoord, int yCoord)
	{
		StartCoroutine(ClearCells(new List<Cell> { Grid.Instance.GetCell(xCoord, yCoord) }));
	}

	public void EmptyCells(List<Cell> clearedCells)
	{
		StartCoroutine(ClearCells(clearedCells, true));
	}

	public IEnumerator ClearCells(List<Cell> clearedCells)
	{
		return ClearCells(clearedCells, false);
	}

	public IEnumerator ClearCells(List<Cell> clearedCells, bool giveNoEnergy)
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

		Dictionary<GridSegment, GridSegment.ClearedCellsInfo> clearInfoBySegment = new Dictionary<GridSegment, GridSegment.ClearedCellsInfo>();
		foreach (GridSegment segment in Grid.Instance.GridSegments)
			clearInfoBySegment.Add(segment, new GridSegment.ClearedCellsInfo());
		GridSegment.ClearedCellsInfo globalClearInfo = new GridSegment.ClearedCellsInfo();

		int clearingCoroutineGroupIndex = CoroutineExtension.GetLatestAvailableIndex();

		foreach (Cell cell in clearedCells)
		{
			if (!cell.isUnoccupied)
			{
				if (!giveNoEnergy) UpdateClearInfo(cell, ref globalClearInfo, ref clearInfoBySegment);

				IEnumerator cellClearingRoutine = cell.ClearCell();
				if (cellClearingRoutine != null)
					cellClearingRoutine.LaunchInParallelCoroutinesGroup(this, clearingCoroutineGroupIndex.ToString());
			}
		}
		while (CoroutineExtension.GroupIsProcessing(clearingCoroutineGroupIndex.ToString()))
			yield return null;

		BroadcastBlockClearsAndShowClearFX(clearedCells, globalClearInfo, clearInfoBySegment);
		yield return StartCoroutine(UpdateIsolatedBlocks());
		yield break;
	}

	void UpdateClearInfo(
		Cell clearedCell
		, ref GridSegment.ClearedCellsInfo globalClearInfo
		, ref Dictionary<GridSegment, GridSegment.ClearedCellsInfo> clearInfoBySegment)
	{
		SettledBlock blockInCell = clearedCell.settledBlockInCell;

		globalClearInfo.clearedCellsBelongingToSegment.Add(clearedCell);

		foreach (GridSegment segment in Grid.Instance.GridSegments)
			if (segment.CellCoordsAreWithinSegment(clearedCell.xCoord, clearedCell.yCoord))
			{
				if (segment.isUsable)
				{
					GridSegment.ClearedCellsInfo segmentInfo = clearInfoBySegment[segment];
					segmentInfo.clearedCellsBelongingToSegment.Add(clearedCell);

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
	}

	void BroadcastBlockClearsAndShowClearFX(
		List<Cell> clearedCells
		, GridSegment.ClearedCellsInfo globalClearInfo
		, Dictionary<GridSegment, GridSegment.ClearedCellsInfo> clearInfoBySegment)
	{
		foreach (GridSegment segment in Grid.Instance.GridSegments)
			segment.BroadcastBlockClears(clearInfoBySegment[segment]);

		if (EBlocksCleared != null)
		{
			PlayerShipModel.TotalEnergyGain totalGain = EBlocksCleared(globalClearInfo);
			gridFX.ShowEnergyGainFX(clearedCells, totalGain);
		}
	}

	IEnumerator UpdateIsolatedBlocks()
	{
		if (EToggleAllSettledBlocksSetToIsolated != null) EToggleAllSettledBlocksSetToIsolated();
		foreach (GridSegment segment in Grid.Instance.GridSegments)
			segment.SetGroundedBlocksInSegment();
		List<IEnumerator> expiredBlockDestructionRoutines = new List<IEnumerator>();
		if (EExpireIsolatedBlocks != null) EExpireIsolatedBlocks(expiredBlockDestructionRoutines);
		foreach (IEnumerator routine in expiredBlockDestructionRoutines)
			routine.LaunchInParallelCoroutinesGroup(this, "ExpiryRoutines");

		while (CoroutineExtension.GroupIsProcessing("ExpiryRoutines"))
			yield return null;

		yield break;
	}

	
}
