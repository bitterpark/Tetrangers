using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipController : ShipController 
{
	public PlayerShipController(ShipModel model, ShipView view)
		: base(model, view)
	{
		EnemyShipController.EEnemyTurnFinished += TryEnableWeaponViewButtons;
		BattleManager.EEngagementModeEnded += DisableWeaponViewButtons;
	}

	public override void DisposeController(bool disposeModel)
	{
		base.DisposeController(disposeModel);
		EnemyShipController.EEnemyTurnFinished -= TryEnableWeaponViewButtons;
		BattleManager.EEngagementModeEnded -= DisableWeaponViewButtons;
	}


	void TryEnableWeaponViewButtons()
	{
		List<int> fireableWeaponIndices;
		if (model.TryGetFireableWeapons(out fireableWeaponIndices))
		{
			ShipWeaponView[] weaponViews = view.GetWeaponViews();
			foreach (ShipWeaponView weaponView in weaponViews)
				if (fireableWeaponIndices.Contains(weaponView.viewIndex))
					weaponView.SetButtonInteractable(true);
				else
					weaponView.SetButtonInteractable(false);
		}
	}

	void DisableWeaponViewButtons()
	{
		SetAllWeaponViewButtonsEnabled(false);
	}

	void SetAllWeaponViewButtonsEnabled(bool enabled)
	{
		foreach (ShipWeaponView weaponView in view.GetWeaponViews())
			weaponView.SetButtonInteractable(enabled);
	}

	protected override void HandleWeaponButtonPress(int buttonIndex)
	{
		SetAllWeaponViewButtonsEnabled(false);
		model.FireWeapon(buttonIndex);
	}

	protected override void HandleWeaponButtonAnimationFinish()
	{
		TryEnableWeaponViewButtons();
	}
}
