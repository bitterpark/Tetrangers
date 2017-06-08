using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseScreen: MonoBehaviour
{

	[SerializeField]
	HangarManager hangarManager;
	[SerializeField]
	MissionsScreen missionsScreen;
	[SerializeField]
	RNDScreen rndScreen;

	[SerializeField]
	GameObject navMenuObject;
	[SerializeField]
	Button missionsButton;
	[SerializeField]
	Button hangarButton;
	[SerializeField]
	Button RNDButton;

	[SerializeField]
	GameObject resourcesPanel;
	[SerializeField]
	Text intelText;
	[SerializeField]
	Text materialsText;

	void Awake()
	{
		hangarButton.onClick.AddListener(OpenHangarSubscreen);
		missionsButton.onClick.AddListener(OpenMissionsSubscreen);
		RNDButton.onClick.AddListener(OpenRNDSubscreen);
		MissionsScreen.ELeaveBaseScreenToggled += CloseBaseScreen;
		MissionManager.EReturnToBaseToggled += OpenBaseScreen;
	}

	public void OpenBaseScreen()
	{
		MusicPlayer.Instance.PlayNextBaseTrack();

		GameDataManager.EIntelChanged += UpdateIntelText;
		GameDataManager.EMaterialsChanged += UpdateMaterialsText;
		UpdateIntelText();
		UpdateMaterialsText();

		if (!gameObject.activeSelf)
			gameObject.SetActive(true);
		navMenuObject.SetActive(true);
		resourcesPanel.SetActive(true);
	}

	void UpdateIntelText()
	{
		intelText.text = GameDataManager.Instance.intel.ToString();
	}
	void UpdateMaterialsText()
	{
		materialsText.text = GameDataManager.Instance.materials.ToString();
	}

	void OpenHangarSubscreen()
	{
		OpenSubscreen(hangarManager);
	}

	void OpenMissionsSubscreen()
	{
		OpenSubscreen(missionsScreen);
	}

	void OpenRNDSubscreen()
	{
		OpenSubscreen(rndScreen);
	}

	void OpenSubscreen(BaseSubscreen screen)
	{
		BaseSubscreen.ESubscreenClosed += ReturnFromSubscreen;
		navMenuObject.SetActive(false);
		screen.OpenSubscreen();
	}

	void ReturnFromSubscreen()
	{
		BaseSubscreen.ESubscreenClosed -= ReturnFromSubscreen;
		navMenuObject.SetActive(true);
	}

	void CloseBaseScreen()
	{
		navMenuObject.SetActive(false);
		resourcesPanel.SetActive(false);
		GameDataManager.EIntelChanged -= UpdateIntelText;
		GameDataManager.EMaterialsChanged -= UpdateMaterialsText;
	}

	//Debug

}

