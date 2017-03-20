using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DraggableUIObjects
{

	[RequireComponent(typeof(RectTransform))]
	public class DraggableUIObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public static event UnityEngine.Events.UnityAction<DragUITarget> EDroppedOnNewTarget;


		RectTransform myRectTransform;
		Transform startingParent;
		Vector2 startingPosition;
		bool dragSuccessful = false;

		private void Awake()
		{
			myRectTransform = GetComponent<RectTransform>();
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			dragSuccessful = false;

			startingPosition = myRectTransform.anchoredPosition;
			startingParent = transform.parent;
			myRectTransform.SetParent(GetComponentInParent<Canvas>().transform, false);
			myRectTransform.SetAsLastSibling();
			//TEMP, MAKE SURE TO REMOVE THE COMPONENT ON ENDDRAG OR THINK OF SOMETHING ELSE
			gameObject.AddComponent<CanvasGroup>().blocksRaycasts = false;

			DragUITarget.EDragTargetReached += HandleDragTargetReached;
			//throw new NotImplementedException();
		}

		public void OnDrag(PointerEventData eventData)
		{
			Vector3 mousePositionInWorldCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 newItemPosition = new Vector3(mousePositionInWorldCoords.x, mousePositionInWorldCoords.y, transform.position.z);
			transform.position = newItemPosition;
			//throw new NotImplementedException();
		}
		/*
		public void OnDrop(PointerEventData eventData)
		{
			Debug.Log("Draggable object dropped");
			//throw new NotImplementedException();
		}*/

		void HandleDragTargetReached(DragUITarget target)
		{
			dragSuccessful = true;
			if (EDroppedOnNewTarget != null) EDroppedOnNewTarget(target);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			DragUITarget.EDragTargetReached -= HandleDragTargetReached;

			if (!dragSuccessful)
			{ 
				myRectTransform.SetParent(startingParent, false);
				myRectTransform.anchoredPosition = startingPosition;
			}
			gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
			//throw new NotImplementedException();
		}
	}
}

