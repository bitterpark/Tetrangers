using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SettledBlock : StaticBlock {

	//public static event UnityAction<BlockType> EBlockCleared;
	public delegate void BlockExpiredDeleg(int gridX, int gridY);
	public static event BlockExpiredDeleg EIsolatedBlockExpired;
	public static event UnityAction<int, int> EBlockDespawnedFromCell;
	public event UnityAction EThisBlockCleared;

	public static List<SettledBlock> existingBlocks = new List<SettledBlock>();

	public BlockType blockType { get; private set; }

	public bool isIsolated
	{
		get { return _isIsolated; }
		set
		{
			if (value)
				isolatedLifetimeRemaining = isolatedLifetimeMax;

			_isIsolated = value;
			myAnimator.SetBool("IsIsolated", value);
		}
	}
	bool _isIsolated = true;
	
	const int isolatedLifetimeMax = 0;
	int isolatedLifetimeRemaining = isolatedLifetimeMax;

	Animator myAnimator;

	public void Initialize(int startingGridX, int startingGridY, BlockType type, PowerupInBlock powerup)
	{
		base.Initialize(startingGridX, startingGridY);

		blockType = type;

		existingBlocks.Add(this);

		FigureSettler.EToggleAllSettledBlocksSetToIsolated += ResetIsolationStatus;
		BattleManager.EEngagementModeStarted += HandleLifetimeDecrease;
		//if (blockType == BlockType.Blue)
		//PlayerShipModel.main.blueEnergyModel.EEnergyGainChanged += SetColorBasedOnPlayerEnergyGain;

		//if (blockType == BlockType.Green)
		//PlayerShipModel.EPlayerGreenGainChanged += SetColorBasedOnPlayerEnergyGain;

		//if (blockType == BlockType.Shield)
		//PlayerShipModel.EPlayerShieldGainChanged += SetColorBasedOnPlayerEnergyGain;

		//if (blockType != BlockType.Powerup)
		//SetColorBasedOnPlayerEnergyGain(PlayerShipModel.GetPlayerResourceIncome(type));

		if (powerup != null)
		{
			powerup.Initialize(this);
			powerup.EBlockDespawning += DespawnFromCell;
		}
	}

	void HandleLifetimeDecrease()
	{
		if (isIsolated)
		{
			//Debug.Log("Isolated lifetime handled, remaining lifetime: "+isolatedLifetimeRemaining);
			if (isolatedLifetimeRemaining == 0)
				ExpireIsolatedBlock();
			isolatedLifetimeRemaining--;
		}
	}

	void ResetIsolationStatus()
	{
		isIsolated = true;
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

	public IEnumerator AnimateMoveToGridCell(int cellX, int cellY)
	{
		yield return new WaitForSeconds(0.1f);
		MoveToGridCell(cellX, cellY);
		yield break;
	}

	public IEnumerator ClearBlock()
	{
		if (EThisBlockCleared != null) EThisBlockCleared();
		return ClearBlockRoutine();
	}
	IEnumerator ClearBlockRoutine()
	{
		myAnimator.SetTrigger("GotCleared");
		while (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Cleared"))
			yield return null;
		while (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Cleared"))
			yield return null;
		SpawnClearParticles();
		DestroyBlock();
		yield break;
	}

	void ExpireIsolatedBlock()
	{
		myAnimator.SetTrigger("IsolatedBlockExpired");
		myAnimator.GetBehaviour<ExpireStateFinishBehaviour>().EStateFinished += HandleExpireAnimationFinish;
	}

	void HandleExpireAnimationFinish()
	{
		if (EIsolatedBlockExpired != null) EIsolatedBlockExpired(currentX, currentY);
		DestroyBlock();
	}

	public void DestroyBlock()
	{
		FigureSettler.EToggleAllSettledBlocksSetToIsolated -= ResetIsolationStatus;
		BattleManager.EEngagementModeStarted -= HandleLifetimeDecrease;
		existingBlocks.Remove(this);
		//if (blockType == BlockType.Blue)
		//PlayerShipModel.EPlayerBlueGainChanged -= SetColorBasedOnPlayerEnergyGain;
		//if (blockType == BlockType.Green)
		//PlayerShipModel.EPlayerGreenGainChanged -= SetColorBasedOnPlayerEnergyGain;
		//if (blockType == BlockType.Shield)
			//PlayerShipModel.EPlayerShieldGainChanged -= SetColorBasedOnPlayerEnergyGain;

		EThisBlockCleared = null;

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

	private void Awake()
	{
		myAnimator = GetComponent<Animator>();
	}
}
