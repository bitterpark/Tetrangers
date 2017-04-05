using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public	class RNDScreen: BaseSubscreen
{

	[SerializeField]
	Transform researchTopicGroup;
	[SerializeField]
	Transform weaponsTopicGroup;
	[SerializeField]
	Transform equipmentTopicGroup;

	[SerializeField]
	ResearchTopicView researchViewPrefab;

	protected override void ExtenderOnAwake()
	{
		ResearchTopic.ETopicResearched += HandleResearchDone;
		ResearchTopic.ETopicProduced += HandleProductionDone;
	}

	public override void OpenSubscreen()
	{
		base.OpenSubscreen();
		DisplayResearchTopics();
		
	}


	void DisplayResearchTopics()
	{
		foreach (ResearchTopic topic in GameDataManager.Instance.playerResearch.currentTopics)
			AddResearchTopicView(topic);
		ShipEquipmentView.EEquipmentMouseoverStopped += HandleTopicEquipmentMouseoverStop;

		StartCoroutine(WaitForContentSizeFitterToFillIn());
	}

	IEnumerator WaitForContentSizeFitterToFillIn()
	{
		researchTopicGroup.GetComponent<ContentSizeFitter>().enabled = false;
		yield return new WaitForEndOfFrame();
		researchTopicGroup.GetComponent<ContentSizeFitter>().enabled = true;
		yield break;
	}

	void AddResearchTopicView(ResearchTopic newTopic)
	{
		ResearchTopicView newView = Instantiate(researchViewPrefab);
		newView.SetDisplayValues(newTopic.intelSpent,newTopic.intelRequired,newTopic.materialsSpent,newTopic.materialsRequired,newTopic.providesEquipment);
		if (newTopic.providesEquipment.equipmentType==EquipmentTypes.Weapon)
			newView.transform.SetParent(weaponsTopicGroup, false);
		else
			newView.transform.SetParent(equipmentTopicGroup, false);
		newView.EResearchOrProduceButtonPressed += () => HandleTopicButtonPressed(newTopic,newView);
		newView.GetDisplayedEquipmentView().EEquipmentMousedOver += 
			(ShipEquipmentView view)=> { HandleTopicEquipmentMousedOver(view, newTopic.providesEquipment);};
		
	}

	void HandleTopicEquipmentMousedOver(ShipEquipmentView view, ShipEquipment equipment)
	{
		if (equipment != null && equipment.hasDescription)
			TooltipManager.Instance.CreateTooltip(equipment.description, view.transform);
	}

	void HandleTopicEquipmentMouseoverStop()
	{
		TooltipManager.Instance.DestroyAllTooltips();
	}

	void HandleTopicButtonPressed(ResearchTopic topic, ResearchTopicView view)
	{
		if (!topic.researched)
			topic.InvestIntel(GameDataManager.Instance.InvestIntel(topic.intelRequired));
		else
			topic.InvestMaterials(GameDataManager.Instance.InvestMaterials(topic.materialsRequired));

		view.SetDisplayValues(topic.intelSpent, topic.intelRequired, topic.materialsSpent, topic.materialsRequired, topic.providesEquipment);
	}

	void HandleResearchDone(ResearchTopic topic)
	{
		foreach (ResearchTopic unlockedTopic in topic.unlocksTopics)
		{
			AddResearchTopicView(unlockedTopic);
			GameDataManager.Instance.AddResearchTopicsToRND(unlockedTopic);
		}
	}

	void HandleProductionDone(ResearchTopic topic)
	{
		GameDataManager.Instance.AddEquipmentToHangar(topic.providesEquipment);
	}

	protected override void CloseSubscreen()
	{
		DisposeAllTopics();
		base.CloseSubscreen();
	}

	void DisposeAllTopics()
	{
		foreach (ResearchTopicView view in researchTopicGroup.GetComponentsInChildren<ResearchTopicView>())
			view.DisposeView();
		ShipEquipmentView.EEquipmentMouseoverStopped -= HandleTopicEquipmentMouseoverStop;
	}

}

