using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public interface IRequiresPlayerTargetSelect
{
	void CallTargetSelectManager();
	void SetTarget(object selectedTarget);
}

public class PlayerTargetSelectManager : Singleton<PlayerTargetSelectManager>
{
	public static UnityAction<bool> ETargetSelectionFinished;

	[SerializeField]
	CanvasGroup battleCanvasGroup;
	[SerializeField]
	Text selectionText;

	GraphicRaycaster canvasRaycaster;

	private void Awake()
	{
		canvasRaycaster = GetComponentInParent<GraphicRaycaster>();
	}

	public void InitiateSelectingPlayerShipSector(IRequiresPlayerTargetSelect equipmentNeedingTargetSector)
	{
		StartCoroutine(SectorSelectingRoutine(equipmentNeedingTargetSector));
	}

	IEnumerator SectorSelectingRoutine(IRequiresPlayerTargetSelect equipmentNeedingTargetSector)
	{
		object foundTarget = null;
		battleCanvasGroup.interactable = false;
		selectionText.enabled = true;
		selectionText.text = "Select ship sector";

		while (true)
		{
			if (Input.GetMouseButtonDown(0))
			{
				List<RaycastResult> raycastResults = new List<RaycastResult>();
				PointerEventData pointerData = new PointerEventData(EventSystem.current);
				pointerData.position = Input.mousePosition;
				canvasRaycaster.Raycast(pointerData, raycastResults);
				foreach (RaycastResult result in raycastResults)
				{
					foundTarget = result.gameObject.GetComponent<SectorView>();
					if (foundTarget != null) break;
				}
				if (foundTarget != null) break;
			}
			else if (Input.GetMouseButtonDown(1)) break;
			yield return null;
		}
		if (foundTarget != null)
			equipmentNeedingTargetSector.SetTarget(foundTarget);
		if (ETargetSelectionFinished != null) ETargetSelectionFinished(foundTarget != null);
		ETargetSelectionFinished = null;
		battleCanvasGroup.interactable = true;
		selectionText.enabled = false;
		yield break;
	}

}
