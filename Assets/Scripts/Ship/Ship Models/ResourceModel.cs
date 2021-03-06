﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ResourceModel
{
	public event UnityAction EResourceChanged;
	public event UnityAction<int> EResourceGained;

	public int resourceCurrent
	{
		get { return _resourceCurrent; }
		set
		{
			int oldValue = _resourceCurrent;
			_resourceCurrent = Mathf.Clamp(value, 0, resourceMax);
			if (oldValue!= _resourceCurrent)
			{
				if (EResourceChanged != null)
					EResourceChanged();
				if (oldValue < _resourceCurrent && EResourceGained != null)
					EResourceGained(_resourceCurrent-oldValue); 
			}
			
		}
	}
	int _resourceCurrent;

	public int resourceMax { get; set; }

	public ResourceModel(int resourceMax)
	{
		SetStartingStats(resourceMax);
	}

	protected void SetStartingStats(int resourceMax)
	{
		this.resourceMax = resourceMax;
		ResetToStartingStats();
	}

	public abstract void ResetToStartingStats();

	public virtual void DisposeModel()
	{
		EResourceChanged = null;
	}

	public int GetActualResourceChange(int attemptedDelta)
	{
		int oldValue = resourceCurrent;
		resourceCurrent += attemptedDelta;
		return resourceCurrent - oldValue;
	}

}

