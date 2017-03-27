using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : Singleton<MainMenuManager> 
{
	[SerializeField]
	GameObject menuPanelGameobject;
	[SerializeField]
	GameObject combatPanelGameobject;
	[SerializeField]
	Button startGameButton;

	[SerializeField]
	BaseScreen baseScreen;
	

	void Awake()
	{
		startGameButton.onClick.AddListener(StartNewGame);
	}

	void StartNewGame()
	{
		menuPanelGameobject.SetActive(false);
		//GetComponent<MissionManager>().InitializeMissionManager();
		baseScreen.OpenBaseScreen();
	}
}
