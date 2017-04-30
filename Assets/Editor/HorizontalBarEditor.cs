using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HorizontalBarView), true)]
public class HorizontalBarEditor : Editor {
	
	//SerializedProperty barColor;

	public void OnEnable()
	{
		//barColor = serializedObject.FindProperty("barColor");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		//serializedObject.Update();

		//barColor.colorValue = EditorGUILayout.ColorField("Bar Color", barColor.colorValue);
		HorizontalBarView view = target as HorizontalBarView;
		view.UpdateBarColor();		
		//serializedObject.ApplyModifiedProperties();
	}
	/*
	public void OnSceneGUI()
	{
		EditorGUI.BeginChangeCheck();
		HorizontalBarView view = target as HorizontalBarView;
		if (EditorGUI.EndChangeCheck())
			view.SetBarColor(view.barColor);
		
	}*/
}
