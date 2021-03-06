﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ICanHaveStatusEffects
{
	StatusEffectManager statusEffectManager { get; }
}

public class StatusEffectManager
{
	public event UnityAction<StatusEffect> EStatusEffectGained;

	List<StatusEffect> activeStatusEffects = new List<StatusEffect>();
	object effectSubject;

	public StatusEffectManager (object effectSubject)
	{
		this.effectSubject = effectSubject;
	}

	public void AddNewStatusEffect(StatusEffect effect)
	{
		activeStatusEffects.Add(effect);
		effect.ActivateEffect(effectSubject);
		effect.EStatusEffectEnded += HandleStatusEffectDeactivation;
		if (EStatusEffectGained != null) EStatusEffectGained(effect);
	}

	void HandleStatusEffectDeactivation(StatusEffect effect)
	{
		Debug.Assert(activeStatusEffects.Contains(effect), "Trying to remove status effect from a ship that doesn't have it!");
		activeStatusEffects.Remove(effect);
		effect.EStatusEffectEnded -= HandleStatusEffectDeactivation;
		//if (EStatusEffectRemoved != null) EStatusEffectRemoved(effect);
	}

	public void Dispose()
	{
		EStatusEffectGained = null;
	}
}
