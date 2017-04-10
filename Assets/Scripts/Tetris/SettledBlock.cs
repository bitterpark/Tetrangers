using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SettledBlock : StaticBlock {

	public static event UnityAction<BlockType> EBlockCleared;
	public static event UnityAction<int, int> EBlockDespawnedFromCell;
	public event UnityAction EThisBlockCleared;

	public BlockType blockType { get; private set; }

	public void Initialize(int startingGridX, int startingGridY, BlockType type, PowerupInBlock powerup)
	{
		base.Initialize(startingGridX, startingGridY);

		blockType = type;

		if (blockType == BlockType.Blue)
			PlayerShipModel.EPlayerBlueGainChanged += SetColorBasedOnPlayerEnergyGain;

		if (blockType == BlockType.Green)
			PlayerShipModel.EPlayerGreenGainChanged += SetColorBasedOnPlayerEnergyGain;

		if (blockType == BlockType.Shield)
			PlayerShipModel.EPlayerShieldGainChanged += SetColorBasedOnPlayerEnergyGain;

		if (blockType != BlockType.Powerup)
			SetColorBasedOnPlayerEnergyGain(PlayerShipModel.GetPlayerResourceIncome(type));

		if (powerup != null)
		{
			powerup.Initialize(this);
			powerup.EBlockDespawning += DespawnFromCell;
		}
	}

	void SetColorBasedOnPlayerEnergyGain(int gain)
	{
		Color newColor = GetComponent<Image>().color;
		if (gain > 0)
			newColor = new Color(newColor.r, newColor.g, newColor.b, 1f);

		else
			newColor = new Color(newColor.r, newColor.g, newColor.b, 0.4f);

		GetComponent<Image>().color = newColor;
	}

	void DespawnFromCell()
	{
		if (EBlockDespawnedFromCell!=null) EBlockDespawnedFromCell(currentX, currentY);
	}

	public void ClearBlock()
	{
		//Debug.Log("Clearing "+blockType+" block!");
		if (EBlockCleared != null) EBlockCleared(blockType);
		if (EThisBlockCleared != null) EThisBlockCleared();

		EThisBlockCleared = null;

		if (blockType == BlockType.Blue)
			PlayerShipModel.EPlayerBlueGainChanged -= SetColorBasedOnPlayerEnergyGain;
		if (blockType == BlockType.Green)
			PlayerShipModel.EPlayerGreenGainChanged -= SetColorBasedOnPlayerEnergyGain;
		if (blockType == BlockType.Shield)
			PlayerShipModel.EPlayerShieldGainChanged -= SetColorBasedOnPlayerEnergyGain;

		SpawnClearParticles();
		DestroyBlock();
	}

	public void DestroyBlock()
	{
		PowerupInBlock attachedPowerup = gameObject.GetComponentInChildren<PowerupInBlock>();
		if (attachedPowerup != null) attachedPowerup.DisposePowerup();
		GameObject.Destroy(this.gameObject);	
	}

	void SpawnClearParticles()
	{
		//ParticleDB.Instance.CreateRowClearParticles(transform.position);
		if (blockType!=BlockType.Powerup)
			ParticleDB.Instance.CreateClearedBlockParticles(transform.position, blockType);
	}
}
