using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class GridCellClickHandler : MonoBehaviour, IPointerClickHandler
{
	public UnityEngine.Events.UnityAction<Vector2> ECellClicked;

	[SerializeField]
	int cellSize;

	public void OnPointerClick(PointerEventData eventData)
	{
		Vector3 worldCoordsFromScreen = Camera.main.ScreenToWorldPoint(eventData.position);
		Vector2 localCoordsFromWorld = transform.InverseTransformPoint(worldCoordsFromScreen);

		int horizontalCellIndex = (int)localCoordsFromWorld.x / cellSize;
		int verticalCellIndex = (int)localCoordsFromWorld.y / cellSize;

		//Debug.Log(string.Format("Pointer clicked at cell :{0};{1}", horizontalCellIndex, verticalCellIndex));
		if (ECellClicked != null) ECellClicked(new Vector2(horizontalCellIndex, verticalCellIndex));
	}
}
