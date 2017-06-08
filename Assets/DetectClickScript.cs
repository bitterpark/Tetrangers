using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DetectClickScript : MonoBehaviour, IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		Vector3 worldCoordsFromScreen = Camera.main.ScreenToWorldPoint(eventData.position);
		Vector2 localCoordsFromWorld = transform.InverseTransformPoint(worldCoordsFromScreen);

		//REMEMBER TO REPLACE THE MAGIC NUMBER!

		int horizontalCellIndex = (int)localCoordsFromWorld.x / 30;
		int verticalCellIndex = (int)localCoordsFromWorld.y / 30;

		Debug.Log(String.Format("Pointer clicked at local position:{0};{1}", horizontalCellIndex, verticalCellIndex));
		
		//transform.InverseTransformPoint();
	}
}
