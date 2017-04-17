using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace StatusEffects
{
	class StatusEffectIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{

		//IDisplayableStatusEffect effect;
		string description;
		string effectName;

		public void InitializeIcon(IDisplayableStatusEffect effect, RectTransform parent)
		{

			//this.effect = effect;

			this.description = effect.description;
			this.effectName = effect.name;

			Image myImage = gameObject.AddComponent<Image>();
			myImage.sprite = effect.icon;
			myImage.color = effect.color;

			effect.EStatusEffectEnded += HandleStatusEffectEnded;

			RectTransform myRectTransform = GetComponent<RectTransform>();
			myRectTransform.SetParent(parent);
			myRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parent.rect.height);
			myRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parent.rect.height);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			TooltipManager.Instance.CreateTooltip(description, transform);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			TooltipManager.Instance.DestroyAllTooltips();
		}

		void HandleStatusEffectEnded(StatusEffect effect)
		{
			effect.EStatusEffectEnded -= HandleStatusEffectEnded;
			GameObject.Destroy(this.gameObject);
		}
	}
}
