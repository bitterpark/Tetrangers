using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : Singleton<PowerupSpawner> {

	[SerializeField]
	PowerupBlock powerupPrefab;

	const float chanceToSpawnPowerupOnPlayerMove = 0.5f;

	void Awake()
	{
		Grid.ENewPlayerMoveDone += TriggerChanceToSpawn;
	}

	void TriggerChanceToSpawn()
	{
		//Debug.Log("Triggering chance to spawn powerup");
		if (Random.value < chanceToSpawnPowerupOnPlayerMove)
			SpawnPowerupInRandomSpot();
	}

	void SpawnPowerupInRandomSpot()
	{
		List<Cell> potentialCells = FindPotentialSpawnCells();
		if (potentialCells.Count>0)
			SpawnPowerup(potentialCells[Random.Range(0,potentialCells.Count)]);
	}

	List<Cell> FindPotentialSpawnCells()
	{
		List<Cell> potentialCells = new List<Cell>();
		for (int j = 0; j <= Grid.Instance.maxX; j++)
		{
			for (int i = Grid.Instance.maxY; i >= 0; i--)
			{
				Cell checkedCell = Grid.Instance.GetCell(j, i);
				if (i == 0 || (!Grid.Instance.GetCell(j, i - 1).isUnoccupied ))
				{
					if (!checkedCell.hasPowerup)
					{
						potentialCells.Add(checkedCell);
						break;
					}
				}
			}
		}
		return potentialCells;
	}

	void SpawnPowerup(Cell spawnInCell)
	{
		PowerupBlock newPowerup = Instantiate(powerupPrefab);
		//Cell spawnInCell = Grid.Instance.GetCell(gridX, gridY);
		newPowerup.Initialize(spawnInCell.xCoord, spawnInCell.yCoord);
		spawnInCell.powerupInCell = newPowerup;
	}

}
