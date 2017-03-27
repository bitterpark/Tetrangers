using System.Collections.Generic;
using UnityEngine;

public class EnemyShipEquipmentController : ShipEquipmentController
{
	public static event UnityEngine.Events.UnityAction EEnemyTurnFinished;

	public EnemyShipEquipmentController(ShipModel shipModel, EquipmentListView equipmentView) : base(shipModel, equipmentView)
	{
		BattleManager.EEngagementModeStarted += DoEnemyTurn;
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		BattleManager.EEngagementModeStarted -= DoEnemyTurn;
		EEnemyTurnFinished = null;
	}

	void DoEnemyTurn()
	{
		//Debug.Log("Doing enemy turn");
		if (!TryActivateBestEquipment())
		{
			if (EEnemyTurnFinished != null) EEnemyTurnFinished();
			//Debug.Log("Enemy turn finished");
		}
	}

	protected override void HandleEquipmentButtonAnimationFinish()
	{
		DoEnemyTurn();
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

