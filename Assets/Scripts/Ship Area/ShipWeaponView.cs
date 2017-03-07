using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipWeaponView : MonoBehaviour {

	public delegate void WeaponButtonPressedDeleg(int index);
	public event WeaponButtonPressedDeleg EWeaponButtonPressed;
	public delegate void EmptyDeleg();
	public event EmptyDeleg EWeaponButtonAnimationFinished;

	[SerializeField]
	Text damageText;
	[SerializeField]
	Text energyCostText;
	[SerializeField]
	GameObject cooldownTimeObject;
	[SerializeField]
	Text nameText;
	[SerializeField]
	Button fireWeaponButton;
	[SerializeField]
	Animator myAnimator;

	public int viewIndex { get; private set; }

	public void Initialize(int index)
	{
		viewIndex = index;
		fireWeaponButton.onClick.AddListener(DoButtonPress);
		myAnimator.GetBehaviour<ViewButtonAnimBehaviour>().EStateFinished += AnimatorCallbackOnAnimationFinished;
	}

	void AnimatorCallbackOnAnimationFinished()
	{
		if (EWeaponButtonAnimationFinished != null) EWeaponButtonAnimationFinished();
	}

	public void DoButtonPress()
	{
		myAnimator.SetTrigger("Fired");
		if (EWeaponButtonPressed != null) EWeaponButtonPressed(viewIndex);
		
	}

	public void SetDisplayValues(int damage, int eneryCost, string weaponName)
	{
		SetDamage(damage);
		SetEnergyCost(eneryCost);
		SetName(weaponName);
	}

	public void SetDamage(int damage)
	{
		damageText.text = damage.ToString();
	}

	public void SetEnergyCost(int cost)
	{
		energyCostText.text = cost.ToString();
	}

	public void SetCooldownTime(int time)
	{
		if (time == 0)
			cooldownTimeObject.SetActive(false);
		else
		{
			cooldownTimeObject.SetActive(true);
			cooldownTimeObject.GetComponentInChildren<Text>().text = time.ToString();
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

	void OnDisable()
	{
		EWeaponButtonPressed = null;
		EWeaponButtonAnimationFinished = null;
		fireWeaponButton.onClick.RemoveAllListeners();
		myAnimator.GetBehaviour<ViewButtonAnimBehaviour>().EStateFinished -= AnimatorCallbackOnAnimationFinished;
	}

}
