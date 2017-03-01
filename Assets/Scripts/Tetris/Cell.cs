using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell 
{
	public bool isUnoccupied
	{
		get { return staticBlockInCell == null;}
	}
	public bool hasPowerup
	{
		get { return powerupInCell != null; }
	}

	public int xCoord;
	public int yCoord;

	public SettledBlock staticBlockInCell { get; private set; }
	public PowerupBlock powerupInCell { get; set; }

	public Cell(int xCoord, int yCoord)
	{
		this.xCoord = xCoord;
		this.yCoord = yCoord;
	}

	public void FillCell(SettledBlock fillWithBlock, bool filledByRowLowering)
	{
		staticBlockInCell = fillWithBlock;
		if (powerupInCell != null)
		{
			if (filledByRowLowering)
			{
				powerupInCell.DisposePowerup();
				powerupInCell = null;
			}
			else
				powerupInCell.TogglePowerup();
		}
	}

	public void EmptyCell()
	{
		staticBlockInCell = null;
	}

}
