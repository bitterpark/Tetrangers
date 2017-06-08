using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarmapManager : Singleton<StarmapManager>
{

	public static event UnityEngine.Events.UnityAction<Vector2> EGridCellClicked;

	List<StarmapObject> objectsOnMap = new List<StarmapObject>();

	[SerializeField]
	int mapSizeVert;
	[SerializeField]
	int mapSizeHor;

	const int cellSize = 30;

	[SerializeField]
	StarmapObject starmapObjectPrefab;
	
	public bool CoordsWithinBounds(int x, int y)
	{
		return (x >= 0 && x < mapSizeHor && y >= 0 && y < mapSizeVert);
	}

	void Start()
	{
		RectTransform myRectTransform = GetComponent<RectTransform>();
		myRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cellSize * mapSizeHor);
		myRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cellSize * mapSizeVert);
		SpawnStarmapObject(3, 3, StarmapObjectType.Enemy);
		SpawnStarmapObject(15,10, StarmapObjectType.Player);

		GetComponent<GridCellClickHandler>().ECellClicked += EGridCellClicked;

	}

	void SpawnStarmapObject(int x, int y, StarmapObjectType type)
	{
		StarmapObject newObj = Instantiate(starmapObjectPrefab);
		newObj.InitializeObject(x, y, transform, type);
	}

}
