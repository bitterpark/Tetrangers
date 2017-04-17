using StatusEffects;
using UnityEngine;

public interface ICanShowStatusEffects
{
	StatusEffectDisplayer statusEffectDisplayer { get; }
};

public class StatusEffectDisplayer : MonoBehaviour
{
	[SerializeField]
	StatusEffectView statusEffectsView;

	public void ShowStatusEffect(IDisplayableStatusEffect effect)
	{
		statusEffectsView.AddStatusEffectIcon(effect);
	}
}

