using System;
using System.Collections.Generic;
using UnityEngine.Events;

public enum Goal {Attack,Defence,Buff}

public class BattleAI
{
	public static UnityAction<ShipEquipment> EAIUsedEquipment;
	public static UnityAction EAITurnFinished;

	EnemyShipModel myShipModel;
	//ShipModel opponentShipModel;
	Dictionary<Goal, int> defaultGoalPriorities = new Dictionary<Goal, int>();

	public BattleAI(EnemyShipModel myShipModel)
	{
		this.myShipModel = myShipModel;

		defaultGoalPriorities.Add(Goal.Attack, 3);
		defaultGoalPriorities.Add(Goal.Defence, 2);
		defaultGoalPriorities.Add(Goal.Buff, 1);

		BattleManager.EEngagementModeStarted += DoTurn;
		EnemyShipEquipmentController.EEnemyEquipmentUseFinished += DoTurn;
	}

	public void Dispose()
	{
		BattleManager.EEngagementModeStarted -= DoTurn;
		EnemyShipEquipmentController.EEnemyEquipmentUseFinished -= DoTurn;
	}

	void DoTurn()
	{
		ShipEquipment equipmentUsedInTurn = GetEquipmentToUse(true);
		if (equipmentUsedInTurn == null)
			equipmentUsedInTurn = GetEquipmentToUse(false);

		if (equipmentUsedInTurn != null && EAIUsedEquipment != null)
			EAIUsedEquipment(equipmentUsedInTurn);
		else
			if (EAITurnFinished != null) EAITurnFinished();

	}

	ShipEquipment GetEquipmentToUse(bool getBlueEnergyEquipment)
	{
		Dictionary<Goal, List<ShipEquipment>> equipmentLists = GetUsableEquipmentSortedByGoals(getBlueEnergyEquipment);
		foreach (Goal goal in GetGoalsSortedByPriority())
		{
			List<ShipEquipment> currentList = equipmentLists[goal];
			if (currentList.Count > 0)
			{
				ShipEquipment mostExpensiveEligibleEquipment = GetMostExpensiveEligibleEquipment(currentList, getBlueEnergyEquipment);

				if (myShipModel.equipmentUser.EnoughEnergyToUseEquipment(mostExpensiveEligibleEquipment))
					return mostExpensiveEligibleEquipment;
				else
					return null;
			}
		}

		return null;
	}

	List<Goal> GetGoalsSortedByPriority()
	{
		List<Goal> sortedGoals = new List<Goal>();
		System.Array allGoals = Enum.GetValues(typeof(Goal));
		for (int i = 0; i < allGoals.Length; i++)
			sortedGoals.Add((Goal)allGoals.GetValue(i));

		sortedGoals.Sort((Goal goal1, Goal goal2)=> 
		{
			if (GetCurrentGoalPriority(goal1) > GetCurrentGoalPriority(goal2))
				return -1;
			if (GetCurrentGoalPriority(goal1) < GetCurrentGoalPriority(goal2))
				return 1;

			return 0;
		});

		return sortedGoals;
	}

	int GetCurrentGoalPriority(Goal goal)
	{
		int modifier = 0;

		if (goal == Goal.Defence && myShipModel.healthManager.shields < myShipModel.healthManager.shieldsMax / 2)
			modifier += 1;
		return defaultGoalPriorities[goal] + modifier;
	}


	Dictionary<Goal,List<ShipEquipment>> GetUsableEquipmentSortedByGoals(bool blue)
	{
		Dictionary<Goal, List<ShipEquipment>> resultDict = new Dictionary<Goal, List<ShipEquipment>>();
		//Remember to draw a distinction between stuff that's not activatable because the ship doesn't have enough resources, and stuff that's on cooldown
		System.Array allGoals = Enum.GetValues(typeof(Goal));
		for (int i = 0; i < allGoals.Length; i++)
			resultDict.Add((Goal)allGoals.GetValue(i),new List<ShipEquipment>());

		foreach (ShipEquipment equipment in myShipModel.shipEquipment.GetAllUsableEquipment(false))
		{
			if (blue && equipment.blueEnergyCostToUse > 0)
				resultDict[equipment.equipmentGoal].Add(equipment);
			if (!blue && equipment.greenEnergyCostToUse > 0)
				resultDict[equipment.equipmentGoal].Add(equipment);
		}
		return resultDict;
	}

	ShipEquipment GetMostExpensiveEligibleEquipment(List<ShipEquipment> equipmentList, bool blue)
	{
		ShipEquipment mostExpensiveFound = null;
		if (equipmentList.Count > 0)
		{
			
			System.Func<ShipEquipment,ShipEquipment, ShipEquipment> getMostExpensive;
			if (blue)
			{
				getMostExpensive = (ShipEquipment equipment1, ShipEquipment equipment2) =>
				{
					if (equipment1.blueEnergyCostToUse > equipment2.blueEnergyCostToUse)
						return equipment1;
					else
						return equipment2;
				};
			}
			else
			{
				getMostExpensive = (ShipEquipment equipment1, ShipEquipment equipment2) =>
				{
					if (equipment1.greenEnergyCostToUse > equipment2.greenEnergyCostToUse)
						return equipment1;
					else
						return equipment2;
				};
			}
			/*
			equipmentList.Sort(usedSort);*/

			foreach (ShipEquipment equipment in equipmentList)
				if (mostExpensiveFound == null)
					mostExpensiveFound = equipment;
				else
					mostExpensiveFound = getMostExpensive(equipment,mostExpensiveFound);
		}
		return mostExpensiveFound;
	}

}

