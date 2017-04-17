using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace StatusEffects
{
	public interface IDisplayableStatusEffect
	{
		Sprite icon { get; }
		String name { get; }
		String description { get; }
		Color color { get; }
		event UnityEngine.Events.UnityAction<StatusEffect> EStatusEffectEnded;
	}

	[RequireComponent(typeof(HorizontalLayoutGroup))]
	public class StatusEffectView : MonoBehaviour
	{

		[SerializeField]
		Sprite testStatusEffectSprite;

		public void AddStatusEffectIcon(IDisplayableStatusEffect effect)
		{
			GameObject effectObject = new GameObject();
			StatusEffectIcon effectIcon = effectObject.AddComponent<StatusEffectIcon>();
			effectIcon.InitializeIcon(effect,GetComponent<RectTransform>());

			effectObject.transform.SetParent(transform);

		}
	}

}

