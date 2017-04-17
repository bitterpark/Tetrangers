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
		baseScreen.OpenBaseScreen();
	}

	//EXPERIMENTAL STUFF
	public event UnityEngine.Events.UnityAction CalledEvent;
	public event UnityEngine.Events.UnityAction EmbeddedEvent
	{
		add { CalledEvent += value; }
		remove { CalledEvent -= value; }
	}

	/*
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.J))
			ExperimentalStuff();
		if (Input.GetKeyDown(KeyCode.I))
			ETestEvent();
	}

	//delegate int TestDeleg(int arg);
	event UnityEngine.Events.UnityAction ETestEvent;

	void ExperimentalStuff()
	{
		//ETestEvent += TesTest2;
		//ETestEvent += TesTest;
		UnityEngine.Events.UnityAction fck = null;
		fck=()=> { ETestEvent -= fck; Debug.Log("Anonymous event fired"); };

		ETestEvent += fck;
		//ETestEvent();

		//int testInt = ETestEvent(10);
		//Debug.Log(testInt);

	}

	int TesTest2(int arg2)
	{
		Debug.Log("Test 2 fired");
		arg2 -= 2;
		return arg2;
	}
	int TesTest(int arg)
	{
		Debug.Log("Test 1 fired");
		arg -= 5;
		return arg;
	}
	*/


}
