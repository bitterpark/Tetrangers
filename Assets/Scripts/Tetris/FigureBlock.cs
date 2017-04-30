using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BlockType { Blue, Green, Shield, ShipEnergy, Powerup }

public class FigureBlock : MonoBehaviour
{
	
	BlockType blockType;

	[SerializeField]
	Color shieldBlockColor;
	[SerializeField]
	Color blueBlockColor;
	[SerializeField]
	Color greenBlockColor;
	[SerializeField]
	Color shipBlockColor;
	[SerializeField]
	Color powerupBlockColor;

	public Vector2 getBlockOffsets
    {
        get { return new Vector2(xOffsetFromZero,yOffsetFromZero); }

    }
    [SerializeField]
	int xOffsetFromZero;
	[SerializeField]
	int yOffsetFromZero;
	int newXOffsetFromZero;
	int newYOffsetFromZero;

	PowerupInBlock attachedPowerup = null;

	public void Initialize()
	{
		System.Array blockTypes = System.Enum.GetValues(typeof(BlockType));

		List<BlockType> regularTypes = new List<BlockType>();
		for (int i=0; i<blockTypes.Length; i++)
			regularTypes.Add((BlockType)blockTypes.GetValue(i));

		regularTypes.Remove(BlockType.Powerup);
		BlockType randomType = regularTypes[Random.Range(0,regularTypes.Count)];

		if (Random.value < BalanceValuesManager.Instance.powerupSpawnChancePerMove)
			randomType = BlockType.Powerup;

		Initialize(randomType);
	}

	public void Initialize(BlockType assignedType)
	{
		blockType = assignedType;

		Image myImage = GetComponent<Image>();

		if (blockType == BlockType.Blue)
			myImage.color = blueBlockColor;
		else if (blockType == BlockType.Green)
			myImage.color = greenBlockColor;
		else if (blockType == BlockType.Shield)
			myImage.color = shieldBlockColor;
		else if (blockType == BlockType.ShipEnergy)
			myImage.color = shipBlockColor;
		else if (blockType == BlockType.Powerup)
		{
			myImage.color = powerupBlockColor;

			attachedPowerup = PowerupInBlockSpawner.Instance.GetRandomPowerupGameobject();
			attachedPowerup.transform.SetParent(transform, false);
		}
		//for (int i=0; i<blockTypes.Length; i++)
	}

	public void SettleBlock(int figureX, int figureY, out SettledBlock settledComponent)
	{
		settledComponent = gameObject.AddComponent<SettledBlock>();
		settledComponent.Initialize(figureX + xOffsetFromZero, figureY + yOffsetFromZero, blockType, attachedPowerup);
			
		Destroy(this);
	}

	public void SnapToParent()
	{
		GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffsetFromZero * Grid.Instance.cellSize, yOffsetFromZero * Grid.Instance.cellSize);
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
		/*
		float directionFromZerothCoord = Mathf.Atan2(yOffsetFromZero, xOffsetFromZero) * Mathf.Rad2Deg;
		directionFromZerothCoord = Mathf.MoveTowardsAngle(directionFromZerothCoord, directionFromZerothCoord - 90, 90);
		directionFromZerothCoord *= Mathf.Deg2Rad;

		float distanceFromZerothCoord = new Vector2(xOffsetFromZero, yOffsetFromZero).magnitude;
		newXOffset = Mathf.RoundToInt(Mathf.Cos(directionFromZerothCoord) * distanceFromZerothCoord);
		newYOffset = Mathf.RoundToInt(Mathf.Sin(directionFromZerothCoord) * distanceFromZerothCoord);
		*/
		int x = yOffsetFromZero; //this y - center point y
		int y = xOffsetFromZero;//this x - center point x
		newXOffset = x; //center point x - found x
		newYOffset = -y; //center point y + found y
		//int x = newYOffsetFromZero;

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
