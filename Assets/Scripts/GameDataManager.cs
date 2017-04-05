using System;
using System.Collections.Generic;
using UnityEngine;


public class GameDataManager: Singleton<GameDataManager>
{
	public static event UnityEngine.Events.UnityAction EIntelChanged;
	public static event UnityEngine.Events.UnityAction EMaterialsChanged;

	public PlayerShipModel playerShip { get; private set; }
	public HangarModel playerHangar { get; private set; }
	public RNDModel playerResearch { get; private set; }


	public int intel { get; private set; }
	public int materials { get; private set; }

	void Awake()
	{
		playerShip = PlayerShipModel.GetPlayerShipModelInstance();
		playerHangar = new HangarModel();
		playerResearch = new RNDModel();

		intel = BalanceValuesManager.Instance.startingIntel;
		materials = BalanceValuesManager.Instance.startingMaterials;
	}

	public int InvestIntel(int requiredIntel)
	{
		int investedIntel = Mathf.Min(requiredIntel,intel);
		ChangeIntel(-investedIntel);
		return investedIntel;
	}

	public int InvestMaterials(int requiredMaterials)
	{
		int investedMaterials = Mathf.Min(requiredMaterials, materials);
		ChangeMaterials(-investedMaterials);
		return investedMaterials;
	}

	public void ChangeIntel(int delta)
	{
		Debug.Assert(intel+delta >= 0, "Setting intel below zero!");
		intel += delta;
		if (EIntelChanged != null) EIntelChanged();
	}
	public void ChangeMaterials(int delta)
	{
		//Debug.Assert(materials - delta >= 0, "Setting materials below zero!");
		materials = Mathf.Max( materials+delta,0);
		if (EMaterialsChanged != null) EMaterialsChanged();
	}

	public void AddResearchTopicsToRND(params ResearchTopic[] addedTopics)
	{
		playerResearch.AddTopics(addedTopics);
	}
	/*
	public List<ResearchTopic> GetUnresearchedTopics()
	{
		foreach (ResearchTopic topic in playerResearch.currentTopics)
	}*/

	public void AddEquipmentToHangar(params ShipEquipment[] addedEquipment)
	{
		playerHangar.AddEquipment(addedEquipment);
	}

}

