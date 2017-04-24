using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell 
{
	public bool isUnoccupied
	{
		get { return settledBlockInCell == null;}
	}

	public int xCoord;
	public int yCoord;

	public SettledBlock settledBlockInCell { get; private set; }

	public Cell(int xCoord, int yCoord)
	{
		this.xCoord = xCoord;
		this.yCoord = yCoord;
	}

	public void FillCell(SettledBlock fillWithBlock, bool filledByRowLowering)
	{
		settledBlockInCell = fillWithBlock;
	}

	public void ClearCell()
	{
		if (settledBlockInCell != null)
			settledBlockInCell.ClearBlock();
		settledBlockInCell = null;
	}

	public SettledBlock ExtractBlockFromCell()
	{
		SettledBlock result = settledBlockInCell;
		settledBlockInCell = null;
		return result;
	}

	public void EmptyCell()
	{
		if (settledBlockInCell != null)
			settledBlockInCell.DestroyBlock();
		settledBlockInCell = null;
	}

}
