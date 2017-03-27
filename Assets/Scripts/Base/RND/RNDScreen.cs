using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public	class RNDScreen: BaseSubscreen
{

	[SerializeField]
	Transform researchTopicGroup;

	[SerializeField]
	ResearchTopicView researchViewPrefab;

	public override void OpenSubscreen()
	{
		base.OpenSubscreen();
		DisplayResearchTopics();
		ResearchTopic.ETopicResearched += HandleResearchDone;
		ResearchTopic.ETopicProduced += HandleProductionDone;
	}


	void DisplayResearchTopics()
	{
		foreach (ResearchTopic topic in GameDataManager.Instance.playerResearch.currentTopics)
			AddResearchTopicView(topic);
	}

	void AddResearchTopicView(ResearchTopic newTopic)
	{
		ResearchTopicView newView = Instantiate(researchViewPrefab);
		newView.SetDisplayValues(newTopic.intelSpent,newTopic.intelRequired,newTopic.materialsSpent,newTopic.materialsRequired,newTopic.providesEquipment);
		newView.transform.SetParent(researchTopicGroup,false);
		newView.EResearchOrProduceButtonPressed += () => HandleTopicButtonPressed(newTopic,newView);
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
	}

}

