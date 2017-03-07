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
	GameObject missionPanelGameobject;
	[SerializeField]
	Button startGameButton;
	

	void Awake()
	{
		startGameButton.onClick.AddListener(StartNewGame);
	}

	void StartNewGame()
	{
		//BattleManager.Instance.gameObject.SetActive(true);
		//combatPanelGameobject.SetActive(true);

		menuPanelGameobject.SetActive(false);
		GetComponent<MissionManager>().InitializeMissionManager();
	}
}
