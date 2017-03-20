using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : Singleton<PowerupSpawner> {

	[SerializeField]
	PowerupBlock powerupPrefab;

	float chanceToSpawnPowerupOnPlayerMove;

	List<PowerupType> powerupTypesNotInUse = new List<PowerupType>();

	void Awake()
	{
		TetrisManager.ETetrisEndClear += ClearOnTetrisFinish;
		TetrisManager.ENextPlayerMoveStarted += TriggerChanceToSpawn;
		PowerupActivator.EPowerupActivated += FreeUpPowerupType;

		ResetUnusedPowerupTypes();
		chanceToSpawnPowerupOnPlayerMove = BalanceValuesManager.Instance.powerupSpawnChancePerMove;
	}

	void ClearOnTetrisFinish()
	{
		ResetUnusedPowerupTypes();
	}

	void ResetUnusedPowerupTypes()
	{
		powerupTypesNotInUse.Clear();
		System.Array types = System.Enum.GetValues(typeof(PowerupType));
		for (int i = 0; i < types.Length; i++)
		{
			PowerupType type = (PowerupType)types.GetValue(i);
			powerupTypesNotInUse.Add(type);
		}
	}

	void TriggerChanceToSpawn()
	{
		//Debug.Log("Triggering chance to spawn powerup");
		if (powerupTypesNotInUse.Count>0 && Random.value < chanceToSpawnPowerupOnPlayerMove)
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

		PowerupType randomType = powerupTypesNotInUse[Random.Range(0,powerupTypesNotInUse.Count)];
		powerupTypesNotInUse.Remove(randomType);

		randomType = PowerupType.Change;//debug

		newPowerup.AssignPowerupType(randomType);
		spawnInCell.powerupInCell = newPowerup;
	}

	void FreeUpPowerupType(PowerupType type)
	{
		if (!powerupTypesNotInUse.Contains(type))
			powerupTypesNotInUse.Add(type);
	}

}
