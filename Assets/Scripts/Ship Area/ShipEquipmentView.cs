using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ShipEquipmentView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public delegate void WeaponButtonPressedDeleg(ShipEquipmentView thisView);
	public event WeaponButtonPressedDeleg EEquipmentButtonPressed;
	public event UnityEngine.Events.UnityAction EEquipmentButtonAnimationFinished;

	public event UnityEngine.Events.UnityAction<ShipEquipmentView> EEquipmentMousedOver;
	public static event UnityEngine.Events.UnityAction EEquipmentMouseoverStopped;

	[SerializeField]
	GameObject damageObject;
	[SerializeField]
	GameObject blueEnergyCostObject;
	[SerializeField]
	GameObject greenEnergyCostObject;
	[SerializeField]
	GameObject cooldownTimeObject;
	[SerializeField]
	GameObject generatorDeltaObject;
	[SerializeField]
	Image generatorDeltaUpIcon;
	[SerializeField]
	Image generatorDeltaDownIcon;

	[SerializeField]
	Text nameText;
	[SerializeField]
	Button fireWeaponButton;
	[SerializeField]
	Animator myAnimator;
	
	[SerializeField]
	Color activeCooldownColor;
	[SerializeField]
	Color inactiveCooldownColor;


	//public int viewIndex { get; private set; }

	public void Initialize()
	{
		//viewIndex = index;
		fireWeaponButton.onClick.AddListener(DoButtonPress);
		myAnimator.GetBehaviour<ViewButtonAnimBehaviour>().EStateFinished += AnimatorCallbackOnAnimationFinished;
	}

	void AnimatorCallbackOnAnimationFinished()
	{
		if (EEquipmentButtonAnimationFinished != null) EEquipmentButtonAnimationFinished();
	}

	public void DoButtonPress()
	{
		myAnimator.SetTrigger("Fired");
		if (EEquipmentButtonPressed != null) EEquipmentButtonPressed(this);
		
	}

	public void SetDisplayValues(int blueEnergyCost, int greenEnergyCost, int generatorDelta, string equipmentName)
	{
		SetDisplayValues(0, blueEnergyCost, greenEnergyCost, generatorDelta, equipmentName);
	}

	public void SetDisplayValues(int damage, int blueEnergyCost, int greenEnergyCost, int generatorDelta, string equipmentName)
	{
		SetEnergyCost(blueEnergyCost, greenEnergyCost);
		SetName(equipmentName);
		SetGeneratorDelta(generatorDelta);
		SetDamage(damage);
	}
	

	public void SetDamage(int damage)
	{
		if (damage > 0)
		{
			damageObject.SetActive(true);
			damageObject.GetComponentInChildren<Text>().text = damage.ToString();
		}
		else
			damageObject.SetActive(false);
	}

	public void SetEnergyCost(int blueCost, int greenCost)
	{
		if (blueCost > 0)
		{
			blueEnergyCostObject.SetActive(true);
			blueEnergyCostObject.GetComponentInChildren<Text>().text = blueCost.ToString();
		}
		else
			blueEnergyCostObject.SetActive(false);

		if (greenCost > 0)
		{
			greenEnergyCostObject.SetActive(true);
			greenEnergyCostObject.GetComponentInChildren<Text>().text = greenCost.ToString();
		}
		else
			greenEnergyCostObject.SetActive(false);
	}

	public void SetCooldownTime(int currentTime, int maxTime)
	{
		if (currentTime == 0)
			SetCooldownTimeText(maxTime, false);
		else
			SetCooldownTimeText(currentTime, true);
	}

	void SetCooldownTimeText(int time, bool current)
	{
		if (time > 0)
		{
			cooldownTimeObject.SetActive(true);
			Text cooldownTimeText = cooldownTimeObject.GetComponentInChildren<Text>();
			Image cooldownTimeIcon = cooldownTimeObject.GetComponentInChildren<Image>();

			Color cooldownColor;
			if (current)
				cooldownColor = activeCooldownColor;
			else
				cooldownColor = inactiveCooldownColor;

			cooldownTimeText.color = cooldownColor;
			cooldownTimeIcon.color = cooldownColor;

			cooldownTimeText.text = time.ToString();
		}
		else
			cooldownTimeObject.SetActive(false);
	}

	public void SetGeneratorDelta(int delta)
	{
		if (delta == 0)
			generatorDeltaObject.SetActive(false);
		else
		{
			generatorDeltaObject.SetActive(true);
			Text generatorDeltaText = generatorDeltaObject.GetComponentInChildren<Text>();

			generatorDeltaText.text = delta.ToString();

			if (delta>0)
			{
				generatorDeltaText.color = Color.red;
				generatorDeltaUpIcon.gameObject.SetActive(true);
				generatorDeltaDownIcon.gameObject.SetActive(false);
			}
			else
			{
				generatorDeltaText.color = Color.blue;
				generatorDeltaUpIcon.gameObject.SetActive(false);
				generatorDeltaDownIcon.gameObject.SetActive(true);
			}
		}
	}

	public void SetName(string name)
	{
		nameText.text = name;
	}

	public void SetButtonInteractable(bool interactable)
	{
		fireWeaponButton.interactable = interactable;
	}

	public void DisposeView()
	{
		EEquipmentButtonPressed = null;
		EEquipmentButtonAnimationFinished = null;
		EEquipmentMousedOver = null;
		fireWeaponButton.onClick.RemoveAllListeners();
		myAnimator.GetBehaviour<ViewButtonAnimBehaviour>().EStateFinished -= AnimatorCallbackOnAnimationFinished;
		GameObject.Destroy(this.gameObject);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (EEquipmentMousedOver != null) EEquipmentMousedOver(this);
		//TooltipManager.Instance.CreateTooltip("Equipment tooltip\n this has multiple lines\n like so",transform);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (EEquipmentMouseoverStopped != null) EEquipmentMouseoverStopped();
		//TooltipManager.Instance.DestroyAllTooltips();
	}
}
