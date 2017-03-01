using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FigureBlock : MonoBehaviour
{	
	[SerializeField]
	int xOffsetFromZero;
	[SerializeField]
	int yOffsetFromZero;
	int newXOffsetFromZero;
	int newYOffsetFromZero;
	
	public bool TrySettleBlock(int figureX, int figureY, out SettledBlock settledComponent)
	{
		if (figureY+yOffsetFromZero <= Grid.Instance.maxYAllowedForSettling)
		{
			settledComponent = gameObject.AddComponent<SettledBlock>();
			settledComponent.Initialize(figureX+xOffsetFromZero, figureY+yOffsetFromZero);
			Destroy(this);
			return true;
		}
		else
		{
			settledComponent = null;
			Destroy(this.gameObject);
			return false;
		}
	}

	public void SnapToParent()
	{
		transform.localPosition = new Vector3(xOffsetFromZero * Grid.Instance.cellSize, yOffsetFromZero * Grid.Instance.cellSize);
	}

	public bool TryMoveWithFigure(int figureX, int figureY)
	{
		FigureController.EMoveStatusDetermined += ResolveStartedMove;
		if (Grid.Instance.CellExistsIsUnoccupied(figureX + xOffsetFromZero, figureY + yOffsetFromZero))
			return true;
		else
			return false;
	}

	void ResolveStartedMove(bool confirmed)
	{
		if (confirmed)
			SnapToParent();

		FigureController.EMoveStatusDetermined -= ResolveStartedMove;
	}

	public bool TryRotateAroundFigure(int figureX, int figureY)
	{
		//print("Rotate started for (" + xOffsetFromZero + ";" + yOffsetFromZero + ")");
		float directionFromZerothCoord = Mathf.Atan2(yOffsetFromZero, xOffsetFromZero) * Mathf.Rad2Deg;
		//print("Start direction:"+directionFromZerothCoord);	
		directionFromZerothCoord = Mathf.MoveTowardsAngle(directionFromZerothCoord, directionFromZerothCoord - 90, 90);
		//print("New direction:" + directionFromZerothCoord);
		directionFromZerothCoord *= Mathf.Deg2Rad;
		//Vector2 newCoordsVector = new Vector2(Mathf.Cos(directionFromZerothCoord),Mathf.Sin(directionFromZerothCoord))*distanceFromZerothCoord;
		//print("Rotating to (" +newCoordsVector+")");

		float distanceFromZerothCoord = new Vector2(xOffsetFromZero, yOffsetFromZero).magnitude;
		newXOffsetFromZero = Mathf.RoundToInt(Mathf.Cos(directionFromZerothCoord)*distanceFromZerothCoord);
		newYOffsetFromZero = Mathf.RoundToInt(Mathf.Sin(directionFromZerothCoord)*distanceFromZerothCoord);

		FigureController.ERotateStatusDetermined += ResolveStartedRotation;

		if (Grid.Instance.CellExistsIsUnoccupied(figureX + newXOffsetFromZero, figureY + newYOffsetFromZero))
		{
			//print("cell exists");
			return true;
		}
		else
		{
			//print("cell does not exist");
			return false;
		}
	}

	void ResolveStartedRotation(bool confirmed, int figureX, int figureY)
	{
		if (confirmed)
		{
			xOffsetFromZero = newXOffsetFromZero;
			yOffsetFromZero = newYOffsetFromZero;
			SnapToParent();
		}
		FigureController.ERotateStatusDetermined -= ResolveStartedRotation;
	}
	
}
