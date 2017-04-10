using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
	readonly Color defaultColor = Color.black;
	readonly int defaultFontSize = 18;
	readonly float defaultUpwardDisplacement = 50f;
	float upwardDisplacementPerSecond;
	Text myText;
	/*
	public void Initialize(string text)
	{
		Initialize(text, defaultColor, defaultFontSize);
	}

	public void Initialize(string text, Color fontColor)
	{

	}*//*
	public static void CreateFloatingText(string text, Color textColor, int fontSize, float lifetime, Transform originTransform)
	{
		CreateFloatingText(text, textColor, fontSize, lifetime, originTransform);
	}*/
	public static void CreateFloatingText(string text, Color textColor, int fontSize, float lifetime, Transform originTransform)
	{
		CreateFloatingText(text,textColor,fontSize,lifetime,originTransform,originTransform.position);
	}
	public static void CreateFloatingText(string text, Color textColor, int fontSize, float lifetime, Transform originTransform, Vector3 originWorldPosition)
	{
		FloatingText newFloatingText = new GameObject().AddComponent<FloatingText>();
		newFloatingText.name = "Floating Text";
		newFloatingText.gameObject.AddComponent<ContentSizeFitter>().horizontalFit=ContentSizeFitter.FitMode.PreferredSize;
		newFloatingText.Initialize(text,lifetime, originTransform);
		newFloatingText.myText.color = textColor;
		newFloatingText.myText.fontSize = fontSize;
		newFloatingText.transform.position = originWorldPosition;
	}


	public void Initialize(string text, float lifetime, Transform originTransform)
	{
		myText = gameObject.AddComponent<Text>();
		myText.text = text;
		myText.color = defaultColor;
		myText.fontSize = defaultFontSize;
		myText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

		upwardDisplacementPerSecond = defaultUpwardDisplacement;

		Canvas parentCanvas = originTransform.GetComponentInParent<Canvas>();

		SetCanvasAndPosition(parentCanvas,originTransform.position);
		StartCoroutine(LifetimeCoroutine(lifetime));
	}
	
	void SetCanvasAndPosition(Canvas canvas, Vector3 worldPosition)
	{
		transform.SetParent(canvas.transform,false);
		transform.SetAsLastSibling();
	}

	IEnumerator LifetimeCoroutine(float lifetime)
	{
		RectTransform myRectTransform = GetComponent<RectTransform>();
		while (lifetime>0)
		{
			float adjustedDisplacement = upwardDisplacementPerSecond * Time.deltaTime;
			myRectTransform.anchoredPosition = myRectTransform.anchoredPosition + new Vector2(0, adjustedDisplacement);
			lifetime -=Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}
		GameObject.Destroy(this.gameObject);
		yield break;
	}

}
