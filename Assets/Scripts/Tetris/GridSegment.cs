using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridSegment
{
	public static event UnityAction<int> ERowsCleared;
	public delegate PlayerShipModel.TotalEnergyGain BlocksClearDeleg(ClearedCellsInfo clearInfo, int segmentIndex);
	public event BlocksClearDeleg EBlocksCleared;

	public event UnityAction EIsolatedBlockExpired;

	public int minX { get; private set; }
	public int minY { get; private set; }
	public int maxX { get; private set; }
	public int maxY { get; private set; }

	public int segmentIndex { get; private set; }

	public bool isUsable = true;

	public List<SettledBlock> blocksInSegment = new List<SettledBlock>();

	GridFX gridFX;

	public GridSegment(int minX, int minY, int maxX, int maxY, int segmentIndex, Transform gridGroup)
	{

		this.segmentIndex = segmentIndex;

		this.minX = minX;
		this.minY = minY;
		this.maxX = maxX;
		this.maxY = maxY;

		gridFX = new GridFX(gridGroup);

		SettledBlock.EIsolatedBlockExpired += HandleIsolatedBlockExpiry;
	}

	void HandleIsolatedBlockExpiry(int blockX, int blockY)
	{
		if (EIsolatedBlockExpired != null && CellCoordsAreWithinSegment(blockX, blockY))
			EIsolatedBlockExpired();
	}

	public bool CellCoordsAreWithinSegment(int cellX, int cellY)
	{
		return (cellX >= minX 
			&& cellY >=minY
			&& cellX <= maxX
			&& cellY <= maxY);
	}

	public void BroadcastBlockClears(ClearedCellsInfo info)
	{
		if (EBlocksCleared != null)
		{
			PlayerShipModel.TotalEnergyGain totalGain = EBlocksCleared(info, segmentIndex);
			gridFX.ShowEnergyGainFX(info.clearedCellsBelongingToSegment, totalGain);
		}
		
	}

	public class ClearedCellsInfo
	{
		public int blueBlocksCount = 0;
		public int greenBlocksCount = 0;
		public int shieldBlocksCount = 0;
		public int shipBlocksCount = 0;
		public List<Cell> clearedCellsBelongingToSegment = new List<Cell>();
	}

	public int GetBlocksOfTypeCount(BlockType type)
	{
		return GetBlocksOfType(type).Count;
	}

	public List<SettledBlock> GetBlocksOfType(BlockType type)
	{
		List<SettledBlock> blocksOfType = new List<SettledBlock>();
		foreach (SettledBlock block in blocksInSegment)
			if (block.blockType == type)
				blocksOfType.Add(block);

		return blocksOfType;
	}
	
	public List<SettledBlock> GetGroundedBlocksInSegment()
	{
		List<SettledBlock> groundedBlocks = GetGroundedBlocks();
		return groundedBlocks;
	}

	List<SettledBlock> GetGroundedBlocks()
	{
		List<SettledBlock> groundedBlocksInSegment = new List<SettledBlock>();
		foreach (SettledBlock block in SettledBlock.existingBlocks)
		{
			if (!block.isIsolated && CellCoordsAreWithinSegment(block.currentX, block.currentY))
				groundedBlocksInSegment.Add(block);
		}
		return groundedBlocksInSegment;
	}

	public void SetGroundedBlocksInSegment()
	{
		for (int i = minX; i <= maxX; i++)
			ScanlineFloodFillGroundedBlocks(i, 0);
	}

	void ScanlineFloodFillGroundedBlocks(int x, int y)
	{
		int x1;

		//draw current scanline from start position to the right
		x1 = x;
		Cell currentCell = Grid.Instance.GetCell(x1, y);

		if (currentCell.isUnoccupied || !currentCell.settledBlockInCell.isIsolated) return;

		while (x1 <= maxX && (!currentCell.isUnoccupied && currentCell.settledBlockInCell.isIsolated))
		{
			currentCell.settledBlockInCell.isIsolated = false;
			//Debug.Log("Isolated set to false");
			x1++;
			currentCell = Grid.Instance.GetCell(x1, y);
		}

		//draw current scanline from start position to the left
		x1 = x - 1;
		if (x1 > 0)
		{
			currentCell = Grid.Instance.GetCell(x1, y);
			while (x1 >= 0 && (!currentCell.isUnoccupied && currentCell.settledBlockInCell.isIsolated))
			{
				currentCell.settledBlockInCell.isIsolated = false;
				//Debug.Log("Isolated set to false");
				x1--;
				currentCell = Grid.Instance.GetCell(x1, y);
			}
		}
		//test for new scanlines above
		x1 = x;
		currentCell = Grid.Instance.GetCell(x1, y);
		if (y > 0)
		{
			while (x1 <= maxX && (!currentCell.isUnoccupied && !currentCell.settledBlockInCell.isIsolated))
			{
				Cell aboveCell = Grid.Instance.GetCell(x1, y - 1);
				if (!aboveCell.isUnoccupied && aboveCell.settledBlockInCell.isIsolated)
					ScanlineFloodFillGroundedBlocks(x1, y - 1);
				x1++;
				currentCell = Grid.Instance.GetCell(x1, y);
			}
		}
		if (x - 1 >= 0)
		{
			x1 = x - 1;
			currentCell = Grid.Instance.GetCell(x1, y);
			if (y > 0)
			{
				while (x1 >= 0 && (!currentCell.isUnoccupied && !currentCell.settledBlockInCell.isIsolated))
				{
					Cell aboveCell = Grid.Instance.GetCell(x1, y - 1);
					if (!aboveCell.isUnoccupied && aboveCell.settledBlockInCell.isIsolated)
						ScanlineFloodFillGroundedBlocks(x1, y - 1);
					x1--;
					currentCell = Grid.Instance.GetCell(x1, y);
				}
			}
		}
		//test for new scanlines below
		x1 = x;
		currentCell = Grid.Instance.GetCell(x1, y);
		if (y < maxY)
		{
			while (x1 <= maxX && (!currentCell.isUnoccupied && !currentCell.settledBlockInCell.isIsolated))
			{
				Cell belowCell = Grid.Instance.GetCell(x1, y + 1);
				if (!belowCell.isUnoccupied && belowCell.settledBlockInCell.isIsolated)
					ScanlineFloodFillGroundedBlocks(x1, y + 1);
				x1++;
				currentCell = Grid.Instance.GetCell(x1, y);
			}
		}
		x1 = x - 1;
		if (x1 > 0)
		{
			currentCell = Grid.Instance.GetCell(x1, y);
			if (y < maxY)
			{
				while (x1 >= 0 && (!currentCell.isUnoccupied && !currentCell.settledBlockInCell.isIsolated))
				{
					Cell belowCell = Grid.Instance.GetCell(x1, y + 1);
					if (!belowCell.isUnoccupied && belowCell.settledBlockInCell.isIsolated)
						ScanlineFloodFillGroundedBlocks(x1, y + 1);
					x1--;
					currentCell = Grid.Instance.GetCell(x1, y);
				}
			}
		}
	}

}

