using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardView: MonoBehaviour
{
	[SerializeField]
	Text headerText;
	[SerializeField]
	Image image;
	[SerializeField]
	Text extraText;
	[SerializeField]
	Text quantityText;


	public void SetDisplayValues(string header, Sprite sprite, int quantity)
	{
		SetDisplayValues(header, sprite, quantity, "");
	}

	public void SetDisplayValues(string header, Sprite sprite, int quantity, string extra)
	{
		headerText.text = header;
		image.sprite = sprite;
		extraText.text = extra;
		quantityText.text = quantity.ToString();
	}

}

