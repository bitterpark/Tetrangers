using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StarmapObjectType {Player, Enemy }

public class StarmapObject : MonoBehaviour {

	public event UnityEngine.Events.UnityAction<Vector2> EObjectMoved;

	[SerializeField]
	BorderSquare squarePrefab;

	//List<BorderSquare> borderSquares = new List<BorderSquare>();
	List<Vector2> outsideCirclePoints = new List<Vector2>();

	int objectGridX;
	int objectGridY;

	int squareMinX;
	int squareMaxX;
	int squareMinY;
	int squareMaxY;

	const int radius = 5;

	Color borderColor;

	public void InitializeObject(int x, int y, Transform gridTransform, StarmapObjectType type)
	{
		transform.SetParent(gridTransform, false);
		MoveToGridPos(x, y);

		if (type == StarmapObjectType.Enemy)
			borderColor = Color.red;
		else
			borderColor = Color.blue;

		DrawCircle();

		StarmapManager.EGridCellClicked += MoveToGridPos;
	}

	void MoveToGridPos(int x, int y)
	{
		MoveToGridPos(new Vector2(x,y));
	}

	void MoveToGridPos(Vector2 newPos)
	{
		objectGridX = (int)newPos.x;
		objectGridY = (int)newPos.y;
		GetComponent<RectTransform>().anchoredPosition = new Vector2(30 * objectGridX, 30 * objectGridY);
		if (EObjectMoved != null) EObjectMoved(new Vector2(objectGridX, objectGridY));
	}

	void DrawCircle()
	{
		squareMinX = -radius;
		squareMaxX = squareMinX + radius * 2;
		squareMinY = -radius;
		squareMaxY = squareMinY + radius * 2;

		List<Vector2> circlePoints = new List<Vector2>();

		int r = radius;

		int y = r;
		int x = 0;
		int p = 1 - r;
		do
		{
			SpawnSymmetric(x, y, ref circlePoints);
			x = x + 1;
			if (p<0)
				p = p + 2 * (x + 1) + 1;
			else
			{
				y = y - 1;
				p = p + 2 * (x + 1) - 2 * (y - 1) + 1;
			}

		} while (x <= y);

		MarkOutsideCirclePoints(ref circlePoints);

		//yield break;
	}

	

	void SpawnSymmetric(int x, int y, ref List<Vector2> circlePoints)
	{
		SpawnSquare(x, y, ref circlePoints);
		//while (!Input.GetKeyDown(KeyCode.C)) yield return null;
		SpawnSquare(y, x, ref circlePoints);
		//while (!Input.GetKeyUp(KeyCode.C)) yield return null;

		SpawnSquare(y, -x, ref circlePoints);

		SpawnSquare(x, -y, ref circlePoints);

		SpawnSquare(-x, -y, ref circlePoints);

		SpawnSquare(-y, -x, ref circlePoints);

		SpawnSquare(-y, x, ref circlePoints);

		SpawnSquare(-x, y, ref circlePoints);

	}

	void SpawnSquare(int xCoord, int yCoord, ref List<Vector2> circlePoints)
	{
		Vector2 coordsVector = new Vector2(xCoord, yCoord);
		if (!circlePoints.Contains(coordsVector))
		{
			//circlePoints.Add(new Vector2(xCoord, yCoord));
			BorderSquare newSquare = Instantiate(squarePrefab);
			newSquare.Initialize(xCoord, yCoord, objectGridX, objectGridY, transform, borderColor);
			circlePoints.Add(coordsVector);

			if (!StarmapManager.Instance.CoordsWithinBounds(objectGridX + xCoord, objectGridY + yCoord))
				newSquare.enabled = false;
		}
	}

	void MarkOutsideCirclePoints(ref List<Vector2> circlePoints)
	{
		SortCirclePointsByY(ref circlePoints);

		int rowMinX = int.MaxValue;
		int rowMaxX = int.MinValue;

		for (int i = 0; i < circlePoints.Count; i++)
		{
			Vector2 exploredPoint = circlePoints[i];
			rowMinX = Mathf.Min(rowMinX, (int)exploredPoint.x);
			rowMaxX = Mathf.Max(rowMaxX, (int)exploredPoint.x);

			int rowY = (int)exploredPoint.y;

			bool lineExplored = (i + 1 == circlePoints.Count || circlePoints[i + 1].y != rowY);
			if (lineExplored)
			{
				//Go left
				for (int xLeft = rowMinX-1; xLeft >= squareMinX; xLeft--)
					outsideCirclePoints.Add(new Vector2(xLeft, rowY));
				//Go right
				for (int xRight = rowMaxX+1; xRight <= squareMaxX; xRight++)
					outsideCirclePoints.Add(new Vector2(xRight, rowY));

				rowMinX = int.MaxValue;
				rowMaxX = int.MinValue;
			}
		}
	}

	void SortCirclePointsByY(ref List<Vector2> circlePoints)
	{
		System.Comparison<Vector2> sortByYComparer = (Vector2 first, Vector2 second) =>
		{
			if (first.y > second.y)
				return -1;
			else
				return 1;
		};

		circlePoints.Sort(sortByYComparer);
	}

	/*
	void SpawnOutsideZoneSquare(int xCoord, int yCoord)
	{
		Image newSquare = Instantiate(squarePrefab);
		newSquare.transform.SetParent(transform, false);
		newSquare.transform.localPosition =  new Vector3(xCoord * 30, yCoord * 30, 0);
		newSquare.color = Color.gray;
		outsideCirclePoints.Add(new Vector2(xCoord, yCoord));
	}*/
}
