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
		int newXOffset;
		int newYOffset;
		if (CanRotateAroundPoint(figureX, figureY, out newXOffset, out newYOffset))
		{
			StartRotation(newXOffset, newYOffset);
			return true;
		}
		else
			return false;
	}

	public bool CanRotateAroundPoint(int pointX, int pointY)
	{
		int dummyOutX;
		int dummyOutY;
		return CanRotateAroundPoint(pointX, pointY, out dummyOutX, out dummyOutY);
	}

	bool CanRotateAroundPoint(int pointX, int pointY, out int newXOffset, out int newYOffset)
	{
		float directionFromZerothCoord = Mathf.Atan2(yOffsetFromZero, xOffsetFromZero) * Mathf.Rad2Deg;
		directionFromZerothCoord = Mathf.MoveTowardsAngle(directionFromZerothCoord, directionFromZerothCoord - 90, 90);
		directionFromZerothCoord *= Mathf.Deg2Rad;

		float distanceFromZerothCoord = new Vector2(xOffsetFromZero, yOffsetFromZero).magnitude;
		newXOffset = Mathf.RoundToInt(Mathf.Cos(directionFromZerothCoord) * distanceFromZerothCoord);
		newYOffset = Mathf.RoundToInt(Mathf.Sin(directionFromZerothCoord) * distanceFromZerothCoord);

		if (Grid.Instance.CellExistsIsUnoccupied(pointX + newXOffset, pointY + newYOffset))
			return true;
		else
			return false;
	}

	void StartRotation(int newXOffset, int newYOffset)
	{
		newXOffsetFromZero = newXOffset;
		newYOffsetFromZero = newYOffset;
		FigureController.ERotateStatusDetermined += ResolveStartedRotation;
	}

	void ResolveStartedRotation(bool canRotate, int figureX, int figureY)
	{
		if (canRotate)
		{
			xOffsetFromZero = newXOffsetFromZero;
			yOffsetFromZero = newYOffsetFromZero;
			SnapToParent();
		}
		FigureController.ERotateStatusDetermined -= ResolveStartedRotation;
	}
	
}
