using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettledBlock : StaticBlock {

	public static event UnityEngine.Events.UnityAction<BlockType> EBlockCleared;

	public BlockType blockType { get; private set; }

	public void Initialize(int startingGridX, int startingGridY, BlockType type)
	{
		base.Initialize(startingGridX, startingGridY);
		blockType = type;
	}

	public void ClearBlock()
	{
		//Debug.Log("Clearing "+blockType+" block!");
		if (EBlockCleared != null) EBlockCleared(blockType);
		SpawnClearParticles();
		DestroyBlock();
	}

	public void DestroyBlock()
	{
		GameObject.Destroy(this.gameObject);	
	}

	void SpawnClearParticles()
	{
		ParticleDB.Instance.CreateRowClearParticles(transform.position);
	}
}
