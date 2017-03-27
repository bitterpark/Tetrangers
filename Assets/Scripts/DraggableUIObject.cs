using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace DraggableUIObjects
{

	[RequireComponent(typeof(RectTransform))]
	public class DraggableUIObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
	{
		public delegate void StaticDropEventDeleg(Transform draggedFrom, Transform droppedInto, DraggableUIObject droppedObject);
		public static event StaticDropEventDeleg EDroppedOnNewTarget;
		public event UnityAction<Transform, DraggableUIObject> EInstanceDroppedOnNewTarget;

		delegate void DroppedOnAnotherDraggableDeleg(DraggableUIObject objectThatWasDroppedOnto);
		static event DroppedOnAnotherDraggableDeleg EDroppedOnAnotherDraggableObject;
		delegate void DraggableObjectsSwapDeleg(DraggableUIObject draggedSwapObject, Transform draggedObjectsParent, DraggableUIObject staticSwapObject, Transform staticObjectsParent);
		static event DraggableObjectsSwapDeleg EObjectsSwapped;

		RectTransform myRectTransform;
		Transform startingParent;
		Vector2 startingPosition;
		bool dragSuccessful = false;

		bool addedACanvasGroup = false;

		private void Awake()
		{
			myRectTransform = GetComponent<RectTransform>();
			EObjectsSwapped += HandleSwapEvent;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			dragSuccessful = false;

			startingPosition = myRectTransform.anchoredPosition;
			startingParent = transform.parent;
			myRectTransform.SetParent(GetComponentInParent<Canvas>().transform, false);
			myRectTransform.SetAsLastSibling();

			CanvasGroup myCanvasGroup = gameObject.GetComponent<CanvasGroup>();
			if (myCanvasGroup == null)
			{
				myCanvasGroup = gameObject.AddComponent<CanvasGroup>();
				addedACanvasGroup = true;
			}
			myCanvasGroup.blocksRaycasts = false;

			DragUITarget.EDragTargetReached += HandleDragTargetReached;
			EDroppedOnAnotherDraggableObject += HandleSwapTargetReached;
		}

		public void OnDrag(PointerEventData eventData)
		{
			Vector3 mousePositionInWorldCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 newItemPosition = new Vector3(mousePositionInWorldCoords.x, mousePositionInWorldCoords.y, transform.position.z);
			transform.position = newItemPosition;
			//throw new NotImplementedException();
		}

		void HandleSwapTargetReached(DraggableUIObject swapTargetObject)
		{
			if (EObjectsSwapped != null) EObjectsSwapped(this,startingParent,swapTargetObject,swapTargetObject.transform.parent);
		}

		void HandleDragTargetReached(Transform targetTransform)
		{
			if (targetTransform != startingParent)
			{
				dragSuccessful = true;
				GetDroppedIntoTargetTransform(targetTransform);
			}
		}

		void HandleSwapEvent(DraggableUIObject draggedSwapObject, Transform draggedObjectsParent
			, DraggableUIObject staticSwapObject, Transform staticObjectsParent)
		{
			//Debug.Assert(swapObjectOnesParent!=swapObjectTwosParent,"parents are the same!");
			if (this == draggedSwapObject)
			{
				HandleDragTargetReached(staticObjectsParent);
				//Debug.Log("Swap object one handled");
			}
			else
				if (this == staticSwapObject)
			{
				GetDroppedIntoTargetTransform(draggedObjectsParent);
				//Debug.Log("Swap object two handled");
			}
		}

		void GetDroppedIntoTargetTransform(Transform targetTransform)
		{
			Transform previousParent;

			if (startingParent != null)
				previousParent = startingParent;
			else
				previousParent = transform.parent;

			if (EDroppedOnNewTarget != null) EDroppedOnNewTarget(previousParent, targetTransform, this);
			if (EInstanceDroppedOnNewTarget != null) EInstanceDroppedOnNewTarget(targetTransform, this);

			myRectTransform.SetParent(targetTransform, false);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			//Debug.Log("Draggable object OnEndDrag fired");
			DragUITarget.EDragTargetReached -= HandleDragTargetReached;
			EDroppedOnAnotherDraggableObject -= HandleSwapTargetReached;
			if (!dragSuccessful)
			{ 
				myRectTransform.SetParent(startingParent, false);
				myRectTransform.anchoredPosition = startingPosition;
			}
			if (addedACanvasGroup)
				GameObject.Destroy(gameObject.GetComponent<CanvasGroup>());
			else
				GetComponent<CanvasGroup>().blocksRaycasts = true;

			//startingParent = null;
		}

		public void OnDrop(PointerEventData eventData)
		{
			if (EDroppedOnAnotherDraggableObject != null) EDroppedOnAnotherDraggableObject(this);
			//Debug.Log("Draggable object OnDrop fired");
		}

		void OnDestroy()
		{
			//EDroppedOnNewTarget = null;
			EInstanceDroppedOnNewTarget = null;
			EObjectsSwapped -= HandleSwapEvent;
		}

		
	}
}

