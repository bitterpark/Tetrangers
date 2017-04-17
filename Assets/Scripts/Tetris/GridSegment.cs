using System;
using System.Collections.Generic;
using UnityEngine;


public class GridSegment
{
	public static event UnityEngine.Events.UnityAction<int> ERowsCleared;
	public delegate PlayerShipModel.TotalEnergyGain BlocksClearDeleg(int blueBlocks, int greenBlocks, int shieldBlocks, int segmentIndex);
	public event BlocksClearDeleg EBlocksCleared;

	int minX;
	int minY;
	int maxX;
	int maxY;

	int segmentIndex;

	GridFX gridFX;

	public GridSegment(int minX, int minY, int maxX, int maxY, int segmentIndex, Transform gridGroup)
	{
		this.segmentIndex = segmentIndex;

		this.minX = minX;
		this.minY = minY;
		this.maxX = maxX;
		this.maxY = maxY;

		gridFX = new GridFX(gridGroup);
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
			PlayerShipModel.TotalEnergyGain totalGain = EBlocksCleared(info.blueBlocksCount, info.greenBlocksCount, info.shieldBlocksCount, segmentIndex);
			gridFX.ShowEnergyGainFX(info.clearedCellsBelongingToSegment, totalGain);
		}
		
	}

	public class ClearedCellsInfo
	{
		public int blueBlocksCount = 0;
		public int greenBlocksCount = 0;
		public int shieldBlocksCount = 0;
		public List<Cell> clearedCellsBelongingToSegment = new List<Cell>();
	}

}

