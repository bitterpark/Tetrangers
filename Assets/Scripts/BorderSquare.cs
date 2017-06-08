using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BorderSquare : MonoBehaviour
{
	int relativeX;
	int relativeY;

	public void Initialize(int relativeX, int relativeY, int parentGridX, int parentGridY, Transform parent, Color color)
	{
		this.relativeX = relativeX;
		this.relativeY = relativeY;

		transform.SetParent(parent, false);
		transform.localPosition = new Vector3(relativeX * 30, relativeY * 30, 0);

		GetComponent<Image>().color = color;
		HandleParentMoving(parentGridX, parentGridY);

		parent.GetComponent<StarmapObject>().EObjectMoved += HandleParentMoving;

	}

	void HandleParentMoving(Vector2 newParentCoords)
	{
		HandleParentMoving((int)newParentCoords.x, (int)newParentCoords.y);
	}

	void HandleParentMoving(int newParentX, int newParentY)
	{
		if (!StarmapManager.Instance.CoordsWithinBounds(newParentX + relativeX, newParentY + relativeY))
			GetComponent<Image>().enabled = false;
		else
			GetComponent<Image>().enabled = true;
	}
}