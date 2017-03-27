using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class Subscreen: MonoBehaviour
{
	[SerializeField]
	Button closeSubscreenButton;

	public virtual void OpenSubscreen()
	{
		if (!gameObject.activeSelf)
			gameObject.SetActive(true);
	}

	protected virtual void CloseSubscreen()
	{
		gameObject.SetActive(false);
		SubscreenCloseEventCaller();
	}

	protected abstract void SubscreenCloseEventCaller();

	void Awake()
	{
		closeSubscreenButton.onClick.AddListener(CloseSubscreen);
		ExtenderOnAwake();
	}

	protected virtual void ExtenderOnAwake()
	{

	}

}

