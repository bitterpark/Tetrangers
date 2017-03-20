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
		/*
		void Start()
		{
			AddStatusEffectIcon(new DummyTestClass(testStatusEffectSprite));
		}*/

		class DummyTestClass : IDisplayableStatusEffect
		{
			public DummyTestClass(Sprite myIcon)
			{
				_testIcon = myIcon;
			}

			public string description
			{
				get
				{
					return "This describes the effect of the thing and the stuff";
				}
			}

			public Sprite icon
			{
				get
				{
					return _testIcon;
				}
			}
			Sprite _testIcon;

			public Color color { get { return Color.green;} }

			public string name
			{
				get
				{
					return "TestName";
				}
			}

			public event UnityAction<StatusEffect> EStatusEffectEnded;
		}

	}

}

