using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HorizontalEnergyBarView : HorizontalBarView
{
	public Image gainImage;
	public Text gainText;

	public void SetGain(int newGain)
	{
		gainText.text = newGain.ToString();
	}

	public override void UpdateBarColor()
	{
		base.UpdateBarColor();
		if (gainImage!=null)
			gainImage.color = barColor;
	}
}
