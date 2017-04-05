using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNDModel
{
	public List<ResearchTopic> currentTopics { get; private set; }

	public RNDModel()
	{
		currentTopics = new List<ResearchTopic>();
		currentTopics.Add(new PlasmaCannonTopic());
		currentTopics.Add(new HeavyLaserTopic());
		//currentTopics.Add(new MineLayerTopic());

		//currentTopics.Add(new MeltdownTriggerTopic());
		//currentTopics.Add(new CoolantInjectorTopic());
		//currentTopics.Add(new BlockEjectorTopic());
		currentTopics.Add(new ReactiveArmorTopic());
		//currentTopics.Add(new AfterburnerTopic());
		//currentTopics.Add(new ManeuveringJetsTopic());
		currentTopics.Add(new GreenAmpTopic());
		currentTopics.Add(new BlueAmpTopic());

	}
	
	public void AddTopics(params ResearchTopic[] addedTopics)
	{
		currentTopics.AddRange(addedTopics);
	}

	public bool HasUnresearchedTopics()
	{
		foreach (ResearchTopic topic in currentTopics)
			if (!topic.researched)
				return true;

		return false;
	}

	public List<ResearchTopic> GetUnresearchedTopics()
	{
		List<ResearchTopic> result = new List<ResearchTopic>();
		foreach (ResearchTopic topic in currentTopics)
			if (!topic.researched)
				result.Add(topic);

		return result;
	}

}

public class ResearchTopic
{
	public static event UnityEngine.Events.UnityAction<ResearchTopic> ETopicResearched;
	public static event UnityEngine.Events.UnityAction<ResearchTopic> ETopicProduced;

	public int intelRequired { get; protected set; }
	public int intelSpent { get; private set; }
	public bool researched
	{
		get { return intelSpent >= intelRequired; }
	}

	public int materialsRequired { get; protected set; }
	public int materialsSpent { get; private set; }
	public bool built
	{
		get { return materialsSpent >= materialsRequired; }
	}

	public ShipEquipment providesEquipment { get; protected set; }

	public List<ResearchTopic> unlocksTopics { get; protected set; }

	public ResearchTopic()
	{
		intelRequired = 0;
		intelSpent = 0;
		materialsRequired = 0;
		materialsSpent = 0;
		unlocksTopics = new List<ResearchTopic>();
		providesEquipment = null;
		InitializeValues();
	}

	protected virtual void InitializeValues(){}

	public int InvestIntel(int intelAmount)
	{
		int actualAmountSpent = Mathf.Min(intelAmount, intelRequired-intelSpent);
		intelSpent += actualAmountSpent;
		if (intelSpent == intelRequired && ETopicResearched != null) ETopicResearched(this);
		return actualAmountSpent;
	}
	public int InvestMaterials(int materialsAmount)
	{
		int actualAmountSpent = Mathf.Min(materialsAmount, materialsRequired - materialsSpent);
		materialsSpent += actualAmountSpent;
		if (materialsSpent == materialsRequired && ETopicProduced != null) ETopicProduced(this);
		return actualAmountSpent;
	}


}

