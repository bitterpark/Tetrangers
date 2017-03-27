using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseSubscreen: Subscreen
{
	public static event UnityEngine.Events.UnityAction ESubscreenClosed;

	protected override void SubscreenCloseEventCaller()
	{
		if (ESubscreenClosed != null) ESubscreenClosed();
	}
}

