using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StatusEffects;
using UnityEngine.Events;

public abstract class StatusEffect : IDisplayableStatusEffect
{
	public Color color
	{
		get; protected set;
	}

	public Sprite icon
	{
		get; protected set;
	}

	public string description
	{
		get; protected set;
	}

	public string name
	{
		get; protected set;
	}

	public int durationRemaining { get; protected set; }

	public event UnityAction<StatusEffect> EStatusEffectEnded;

	public StatusEffect ()
	{
		InitializeValues();
	}

	protected abstract void InitializeValues();

	public void ActivateEffect(object activateOnObject)
	{
		SubclassActivation(activateOnObject);
		ExtenderActivation(activateOnObject);
		BattleManager.EBattleFinished += DeactivateEffect;
		
	}

	protected abstract void SubclassActivation(object activateOnObject);
	protected abstract void ExtenderActivation(object activateOnObject);

	protected void DeactivateEffect()
	{
		ExtenderDeactivation();
		if (EStatusEffectEnded != null) EStatusEffectEnded(this);
		EStatusEffectEnded = null;
		
		BattleManager.EBattleFinished -= DeactivateEffect;
		BattleAI.EAITurnFinished -= DeactivateEffect;
		BattleManager.EEngagementModeStarted -= DeactivateEffect;
	}

	protected abstract void SubclassDeactivation();

	protected virtual void ExtenderDeactivation() { }


}