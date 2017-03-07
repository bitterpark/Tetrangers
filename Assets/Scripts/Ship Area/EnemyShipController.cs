using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipController : ShipController 
{

	public delegate void EmptyDeleg();
	public static event EmptyDeleg EEnemyTurnFinished;

	public EnemyShipController(ShipModel model, ShipView view)
		: base(model, view)
	{
		BattleManager.EEngagementModeStarted += DoEnemyTurn;
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		BattleManager.EEngagementModeStarted -= DoEnemyTurn;
		EEnemyTurnFinished = null;
	}


	protected override void HandleWeaponButtonPress(int buttonIndex)
	{
		model.FireWeapon(buttonIndex);
		//throw new System.NotImplementedException();
	}

	protected override void HandleWeaponButtonAnimationFinish()
	{
		DoEnemyTurn();
	}

	void DoEnemyTurn()
	{
		if (!TryActivateRandomWeapon())
			if (EEnemyTurnFinished != null) EEnemyTurnFinished();		
	}

	bool TryActivateRandomWeapon()
	{
		List<int> fireableWeaponIndeces;
		if (model.TryGetFireableWeapons(out fireableWeaponIndeces))
		{
			int randomWeaponIndex = fireableWeaponIndeces[Random.Range(0, fireableWeaponIndeces.Count)];
			view.GetWeaponViews()[randomWeaponIndex].DoButtonPress();
			return true;
		}
		else
			return false;
	}
}
