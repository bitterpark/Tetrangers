using System.Collections.Generic;
using UnityEngine;


public class GridFX
{

	Transform gridGroup;

	public GridFX(Transform gridGroup)
	{
		this.gridGroup = gridGroup;
	}

	public void ShowEnergyGainFX(List<Cell> clearedCells, PlayerShipModel.TotalEnergyGain totalGain)
	{
		if (clearedCells.Count > 0)
		{
			Cell lastCell=clearedCells[clearedCells.Count-1];
			/*
			if (clearedCells.Count > 1)
				lastCell = clearedCells[Mathf.Clamp(clearedCells.Count / 2, 0, clearedCells.Count - 1)];
			else
				lastCell = clearedCells[0];
			*/
			Vector3 middleCellPos = Grid.Instance.GetCellWorldPosition(lastCell.xCoord, lastCell.yCoord);

			TryCreateEnergyGainText(totalGain.blueGain, Color.cyan, gridGroup, middleCellPos);
			TryCreateEnergyGainText(totalGain.greenGain, Color.green, gridGroup, middleCellPos);
			TryCreateEnergyGainText(totalGain.shieldGain, Color.blue, gridGroup, middleCellPos);
		}
	}

	void TryCreateEnergyGainText(int gain, Color color, Transform gridGroup, Vector3 worldPosition)
	{
		//Debug.Log("Creating energy gain text");
		worldPosition.x += Random.Range(-20, 21);
		if (gain > 0)
			FloatingText.CreateFloatingText(gain.ToString(), color, 40, 1.5f, gridGroup, worldPosition);
	}
}

