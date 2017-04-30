using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ResearchTopicView: MonoBehaviour
{
	public event UnityEngine.Events.UnityAction EResearchOrProduceButtonPressed;

	[SerializeField]
	GameObject researchProgressObject;
	[SerializeField]
	RectTransform researchProgressBar;
	[SerializeField]
	Text researchProgressText;

	[SerializeField]
	GameObject productionProgressObject;
	[SerializeField]
	RectTransform productionProgressBar;
	[SerializeField]
	Text productionProgressText;

	[SerializeField]
	Button researchOrProduceButton;

	[SerializeField]
	Transform equipmentItemViewPosition;
	[SerializeField]
	ShipEquipmentView equipmentViewPrefab;

	void Awake()
	{
		researchOrProduceButton.onClick.AddListener(()=> { if (EResearchOrProduceButtonPressed != null) EResearchOrProduceButtonPressed(); });
	}

	public void SetDisplayValues(int currentIntel, int requiredIntel, int currentMaterials, int requiredMaterials, ShipEquipment equipment)
	{
		SetResearchProgress(currentIntel, requiredIntel);
		if (currentIntel == requiredIntel)
			SetProductionProgress(currentMaterials, requiredMaterials);
		else
			productionProgressObject.SetActive(false);

		SetDisplayedEquipment(equipment);
	}

	void SetDisplayedEquipment(ShipEquipment equipment)
	{
		if (GetDisplayedEquipmentView() == null)
		{
			ShipEquipmentView newEquipmentView = Instantiate(equipmentViewPrefab);
			newEquipmentView.SetDisplayValues(
				equipment.blueEnergyCostToUse
				,equipment.greenEnergyCostToUse
				,equipment.shipEnergyCostToUse
				,equipment.generatorLevelDelta
				,equipment.maxCooldownTime
				,equipment.name);
			if (equipment.equipmentType == EquipmentTypes.Weapon)
			{
				ShipWeapon weapon = equipment as ShipWeapon;
				newEquipmentView.SetDamage(weapon.damage);
				newEquipmentView.SetLockonTime(weapon.lockOnTimeRemaining);
				//newEquipmentView.SetButtonInteractable(false);
			}
			newEquipmentView.transform.SetParent(equipmentItemViewPosition, false);
		}
	}

	void SetResearchProgress(int currentIntel, int requiredIntel)
	{
		if (currentIntel != requiredIntel)
		{
			researchProgressObject.SetActive(true);
			researchProgressText.text = currentIntel + "/" + requiredIntel;
			//researchProgressBar=
			float barPercentage;
			if (requiredIntel > 0)
				barPercentage = (float)currentIntel / (float)requiredIntel;
			else
				barPercentage = 1;

			researchProgressBar.anchorMax = new Vector2(barPercentage, researchProgressBar.anchorMax.y);

			researchOrProduceButton.GetComponentInChildren<Text>().text = "Research ("+(requiredIntel-currentIntel)+")";
		}
		else
			researchProgressObject.SetActive(false);
	}

	void SetProductionProgress(int currentMaterials, int requiredMaterials)
	{
		
		productionProgressObject.SetActive(true);
		productionProgressText.text = currentMaterials + "/" + requiredMaterials;

		float barPercentage;
		if (requiredMaterials > 0)
			barPercentage = (float)currentMaterials / (float)requiredMaterials;
		else
			barPercentage = 1;

		productionProgressBar.anchorMax = new Vector2(barPercentage, productionProgressBar.anchorMax.y);
		if (currentMaterials != requiredMaterials)
			researchOrProduceButton.GetComponentInChildren<Text>().text = "Produce (" + (requiredMaterials - currentMaterials) + ")";
		else
			researchOrProduceButton.gameObject.SetActive(false);
		
	}

	public ShipEquipmentView GetDisplayedEquipmentView()
	{
		return equipmentItemViewPosition.GetComponentInChildren<ShipEquipmentView>();
	}

	public void DisposeView()
	{
		EResearchOrProduceButtonPressed = null;
		researchOrProduceButton.onClick.RemoveAllListeners();
		GetDisplayedEquipmentView().DisposeView();

		GameObject.Destroy(this.gameObject);
	}

}

