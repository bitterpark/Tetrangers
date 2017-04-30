using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HorizontalBarView : MonoBehaviour
{
	[SerializeField]
	RectTransform bar;
	[SerializeField]
	RectTransform underBar;
	[SerializeField]
	Text barText;
	[SerializeField]
	protected Color barColor;

	private void Awake()
	{
		UpdateBarColor();
	}

	public void SetBarValue(int newValue, int maxValue)
	{
		barText.text = newValue.ToString() + "/" + maxValue.ToString();

		float barPercentage;
		if (maxValue > 0)
			barPercentage = (float)newValue / (float)maxValue;
		else
			barPercentage = 1;

		bar.anchorMax = new Vector2(barPercentage, bar.anchorMax.y);
	}

	public virtual void UpdateBarColor()
	{
		if (bar!=null)
			bar.GetComponent<Image>().color = barColor;
		Color underbarColor = new Color(barColor.r * 0.6f, barColor.g * 0.6f, barColor.b * 0.6f);
		if (underBar!=null)
		underBar.GetComponent<Image>().color = underbarColor;
	}


}
