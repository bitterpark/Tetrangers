using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : Singleton<BattleManager> {


	[SerializeField]
	ShipView playerShipView;
	[SerializeField]
	ShipView enemyShipView;
	[SerializeField]
	Sprite tempShipSprite;

	void Start()
	{
		StartNewBattle();
	}

	void Update()
	{
		//if (Input.GetKeyDown(KeyCode.P))
			//StartNewBattle();
	}

	public void StartNewBattle()
	{
		SpawnPlayerShip();
		SpawnEnemyShip();
		TetrisManager.Instance.StartTetris();

	}

	void SpawnPlayerShip()
	{
		ShipModel playerShipModel = new PlayerShipModel(100, 0, tempShipSprite, "Player Ship");
		ShipController playerShipController = new ShipController(playerShipModel, playerShipView);
	}

	void SpawnEnemyShip()
	{
		ShipModel enemyShipModel = new EnemyShipModel(50, 0, tempShipSprite, "Enemy Ship");
		ShipController enemyShipController = new ShipController(enemyShipModel, enemyShipView);
	}

}
