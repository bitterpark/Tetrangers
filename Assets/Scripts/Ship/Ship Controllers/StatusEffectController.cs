using System;
using System.Collections.Generic;
using StatusEffects;


public class StatusEffectController
{
	StatusEffectDisplayer view;
	StatusEffectManager modelEffectManager;

	public StatusEffectController(StatusEffectDisplayer displayer, StatusEffectManager modelEffectManager)
	{
		view = displayer;
		this.modelEffectManager = modelEffectManager;
		modelEffectManager.EStatusEffectGained += HandleStatusEffectAdding;
	}

	public void Dispose()
	{
		modelEffectManager.EStatusEffectGained -= HandleStatusEffectAdding;
		modelEffectManager = null;
	}

	void HandleStatusEffectAdding(IDisplayableStatusEffect effect)
	{
		view.ShowStatusEffect(effect);
	}
}

