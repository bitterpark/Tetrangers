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

	public bool isBeingDestroyed = false;

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
	bool _isIsolated = false;
	
	const int isolatedLifetimeMax = 0;
	int isolatedLifetimeRemaining = isolatedLifetimeMax;

	GridSegment settledInSegment;

	Animator myAnimator;

	public Cell GetCell()
	{
		return Grid.Instance.GetCell(currentX,currentY);
	}

	public void Initialize(int startingGridX, int startingGridY, BlockType type, PowerupInBlock powerup)
	{
		base.Initialize(startingGridX, startingGridY);

		blockType = type;

		existingBlocks.Add(this);
		settledInSegment = Grid.Instance.GetSegmentFromCoords(startingGridX, startingGridY);
		if (settledInSegment!=null) settledInSegment.blocksInSegment.Add(this);

		Clearer.EToggleAllSettledBlocksSetToIsolated += ResetIsolationStatus;
		//BattleManager.EEngagementModeStarted += HandleLifetimeDecrease;
		Clearer.EExpireIsolatedBlocks += HandleLifetimeDecrease;
		//TetrisManager.ECurrentPlayerMoveDone += HandleLifetimeDecrease;
		
		if (powerup != null)
		{
			powerup.Initialize(this);
			powerup.EBlockDespawning += DespawnFromCell;
		}
	}

	void HandleLifetimeDecrease(List<IEnumerator> expiryRoutines)
	{
		if (isIsolated)
		{
			//Debug.Log("Isolated lifetime handled, remaining lifetime: "+isolatedLifetimeRemaining);
			if (isolatedLifetimeRemaining == 0)
				expiryRoutines.Add(ExpireBlockRoutine());
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
		isBeingDestroyed = true;
		SoundFXPlayer.Instance.PlayBlockClearSound();
		myAnimator.SetTrigger("GotCleared");
		while (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Cleared"))
			yield return null;
		while (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Cleared"))
			yield return null;
		SpawnClearParticles();
		DestroyBlock();
		yield break;
	}

	IEnumerator ExpireBlockRoutine()
	{
		isBeingDestroyed = true;
		SoundFXPlayer.Instance.PlayIsolatedBlockExpireSound();
		myAnimator.SetTrigger("IsolatedBlockExpired");
		while (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Isolated_expired"))
			yield return null;
		while (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Isolated_expired"))
			yield return null;


		HandleExpireAnimationFinish();
		yield break;
	}
	//UNused, remove later
	void ExpireIsolatedBlock()
	{
		//Debug.Log("Isolated block expired");
		//Debug.Log("Isolated:" + isIsolated);
		SoundFXPlayer.Instance.PlayIsolatedBlockExpireSound();
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
		Clearer.EToggleAllSettledBlocksSetToIsolated -= ResetIsolationStatus;
		//BattleManager.EEngagementModeStarted -= HandleLifetimeDecrease;
		Clearer.EExpireIsolatedBlocks -= HandleLifetimeDecrease;
		existingBlocks.Remove(this);
		if (settledInSegment != null) settledInSegment.blocksInSegment.Remove(this);
		settledInSegment = null;
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
