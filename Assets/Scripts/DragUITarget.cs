using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DraggableUIObjects
{ 
	[RequireComponent(typeof(RectTransform))]
	public class DragUITarget : MonoBehaviour, IDropHandler
	{
		public static event UnityEngine.Events.UnityAction<Transform> EDragTargetReached;

		public void OnDrop(PointerEventData eventData)
		{
			//Debug.Log("OnDrop fired in Drag Target");
			if (EDragTargetReached != null) EDragTargetReached(transform);
		}
	}
}

