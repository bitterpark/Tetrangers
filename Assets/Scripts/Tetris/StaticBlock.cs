using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticBlock : MonoBehaviour {

	[SerializeField]
	public int currentX { get; private set; }
	[SerializeField]
	public int currentY { get; private set; }

	public void MoveToGridCell(int cellX, int cellY)
	{
		currentX = cellX;
		currentY = cellY;
		GetComponent<RectTransform>().anchoredPosition = Grid.Instance.GetCellPositionInGrid(currentX, currentY);
	}

	public void Initialize(int startingGridX, int startingGridY)
	{
		transform.SetParent(Grid.Instance.gridGroup, false);
		MoveToGridCell(startingGridX, startingGridY);
	}
}
