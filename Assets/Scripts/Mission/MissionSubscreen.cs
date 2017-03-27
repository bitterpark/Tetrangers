using System;
using System.Collections.Generic;
using UnityEngine;


public class MissionSubscreen: Subscreen
{
	public static event UnityEngine.Events.UnityAction EContinuePressed;

	protected override void SubscreenCloseEventCaller()
	{
		if (EContinuePressed != null) EContinuePressed();
	}

}

