using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using StatusEffects;

[RequireComponent(typeof(StatusEffectDisplayer))]
public class SectorEquipmentListView: EquipmentListView, ICanShowStatusEffects
{

	public StatusEffectDisplayer statusEffectDisplayer
	{
		get {return GetComponent<StatusEffectDisplayer>();}
	}
}

