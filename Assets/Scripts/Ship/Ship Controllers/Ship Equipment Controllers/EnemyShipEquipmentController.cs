﻿using System.Collections.Generic;
using UnityEngine;

public class EnemyShipEquipmentController : ShipEquipmentController
{
	//public static event UnityEngine.Events.UnityAction EEnemyTurnFinished;
	public static event UnityEngine.Events.UnityAction EEnemyEquipmentUseFinished;

	TabController tabController;

	public EnemyShipEquipmentController(ShipEquipmentModel shipModel, TabbedEquipmentListView equipmentView) : base(shipModel, equipmentView)
	{
		//BattleManager.EEngagementModeStarted += DoEnemyTurn;
		BattleAI.EAIUsedEquipment += HandleEquipmentUse;

		tabController = new TabController(equipmentView.tabButtonsView, this);
		tabController.ShowWeaponsTab();
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		tabController.Dispose();
		BattleAI.EAIUsedEquipment -= HandleEquipmentUse;
		//BattleManager.EEngagementModeStarted -= DoEnemyTurn;
		EEnemyEquipmentUseFinished = null;
		//EEnemyTurnFinished = null;
	}

	void HandleEquipmentUse(ShipEquipment equipment)
	{
		tabController.ShowEquipmentTypeViews(equipment.equipmentType);
		GetViewRepresentingEquipment(equipment).DoButtonPress();
	}
	//Remove this later
	void DoEnemyTurn()
	{
		//Debug.Log("Doing enemy turn");
		if (!TryActivateBestEquipment())
		{
			//if (EEnemyTurnFinished != null) EEnemyTurnFinished();
			//Debug.Log("Enemy turn finished");
		}
	}

	protected override void HandleEquipmentButtonAnimationFinish(ShipEquipmentView equipmentView)
	{
		base.HandleEquipmentButtonAnimationFinish(equipmentView);
		if (EEnemyEquipmentUseFinished != null) EEnemyEquipmentUseFinished();
		//DoEnemyTurn();
	}

	bool TryActivateBestEquipment()
	{
		List<ShipEquipment> usableEquipment;
		if (model.TryGetAllUsableEquipment(out usableEquipment))
		{

			ShipEquipment bestEquipment = null;
			int highestTotalPointsWorth = -1;
			foreach (ShipEquipment equipment in usableEquipment)
			{
				int equipmentTotalPointsWorth = BalanceValuesManager.Instance.GetTotalPointsValue(equipment.blueEnergyCostToUse, equipment.greenEnergyCostToUse);
				if (equipmentTotalPointsWorth > highestTotalPointsWorth || (equipmentTotalPointsWorth == highestTotalPointsWorth && Random.value > 0.5f))
				{
					highestTotalPointsWorth = equipmentTotalPointsWorth;
					bestEquipment = equipment;
				}
			}
			//usableEquipment[Random.Range(0,usableEquipment.Count)];
			Debug.Assert(bestEquipment != null, "Could not find best equipment to use by AI!");
			GetViewRepresentingEquipment(bestEquipment).DoButtonPress();
			return true;
		}
		else
			return false;
	}
	
}

