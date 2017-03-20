using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoBehaviour 
{

	[SerializeField]
	Text tooltipText;
	[SerializeField]
	Text widthMeasuringText;
	[SerializeField]
	Image contentWrapper;

	const float maxLineWidth = 300;

	const float horizontalOffsetFromParentEdge = 40f;
	const float verticalOffsetFromScreenEdge = 20f;
	float tooltipWidth = 80f;

	public void DisplayTextOnly(string text, Transform tooltipParent)
	{
		SetupTextComponent(text);
		SetPositionAndSortOrder(tooltipParent, text);
	}


	void SetPositionAndSortOrder(Transform tooltipParent, string text)
	{
		GetComponent<Canvas>().worldCamera = Camera.main;
		widthMeasuringText.text = text;

		transform.SetParent(tooltipParent, false);

		Canvas.ForceUpdateCanvases();
		if (transform.parent == null)
			throw new System.Exception("Parent is null!");
		SetWidth();
		SetHorizontalPositionToLeftOrRight();
		ClampVerticalPositionToScreen();
		//transform.localScale = new Vector3(1 / transform.parent.localScale.x, 1 / transform.parent.localScale.y, 1);

		GetComponent<Canvas>().enabled = true;
		GetComponent<Canvas>().overrideSorting = true;
		GetComponent<Canvas>().sortingOrder = 100;
	}

	void SetWidth()
	{
		//float cardsContentWidth = cardsDisplayGroup.GetComponent<GridLayoutGroup>().preferredWidth;
		float textContentWidth = Mathf.Min(widthMeasuringText.GetComponent<RectTransform>().rect.width, maxLineWidth);
		tooltipWidth = textContentWidth;//Mathf.Max(cardsContentWidth, textContentWidth);
		contentWrapper.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tooltipWidth);
	}

	void SetHorizontalPositionToLeftOrRight()
	{
		SnapToLeftOfParent();

		if (Camera.main.WorldToScreenPoint(transform.position).x > Screen.width)
			SnapToRightOfParent();
		/*
		RectTransform.Edge tooltipSide;

		float newX = transform.parent.position.x + (transform.parent.GetComponent<RectTransform>().rect.width) + tooltipWidth + horizontalOffsetFromParentEdge;
		if (Camera.main.WorldToScreenPoint(new Vector3(newX, transform.parent.position.y, 0)).x > Screen.width)
			tooltipSide = RectTransform.Edge.Left;
		else
			tooltipSide = RectTransform.Edge.Right;

		GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(tooltipSide, -(tooltipWidth + horizontalOffsetFromParentEdge), tooltipWidth);*/
	}

	void SnapToLeftOfParent()
	{
		RectTransform myRectTransform = GetComponent<RectTransform>();
		myRectTransform.anchorMax = new Vector2(1f, 0.5f);
		myRectTransform.anchorMin = myRectTransform.anchorMax;
		myRectTransform.anchoredPosition = new Vector2(tooltipWidth * 0.5f + horizontalOffsetFromParentEdge, 0);

	}

	void SnapToRightOfParent()
	{
		RectTransform myRectTransform = GetComponent<RectTransform>();
		myRectTransform.anchorMax = new Vector2(0, 0.5f);
		myRectTransform.anchorMin = myRectTransform.anchorMax;
		myRectTransform.anchoredPosition = new Vector2(-(tooltipWidth * 0.5f + horizontalOffsetFromParentEdge), 0);

	}


	void ClampVerticalPositionToScreen()
	{
		//float cardsContentHeight = cardsDisplayGroup.GetComponent<GridLayoutGroup>().preferredHeight;
		float textContentHeight = tooltipText.preferredHeight;
		float tooltipHeight = textContentHeight;//cardsContentHeight + textContentHeight;

		float lowestWorldYPoint = transform.parent.position.y - tooltipHeight*0.5f;
		float lowestScreenYPoint = Camera.main.WorldToScreenPoint(new Vector3(transform.parent.position.x, lowestWorldYPoint, 0)).y;
		if (lowestScreenYPoint <= 0)
			transform.position += new Vector3(0, Mathf.Abs(lowestScreenYPoint) + verticalOffsetFromScreenEdge);

		float highestWorldYPoint = transform.parent.position.y + tooltipHeight * 0.5f;
		float highestScreenYPoint = Camera.main.WorldToScreenPoint(new Vector3(transform.parent.position.x, highestWorldYPoint, 0)).y;
		if (highestScreenYPoint >= Screen.height)
			transform.position -= new Vector3(0,(verticalOffsetFromScreenEdge - Screen.height) + highestScreenYPoint);
	}

	void SetupTextComponent(string text)
	{
		if (text != "")
			tooltipText.text = text;
		else
			tooltipText.enabled=false;
	}
}
