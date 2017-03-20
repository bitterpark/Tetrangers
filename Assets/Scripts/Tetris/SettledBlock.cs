using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettledBlock : StaticBlock {


	public void ClearBlock()
	{
		SpawnClearParticles();
		DestroyBlock();
	}

	public void DestroyBlock()
	{
		GameObject.Destroy(this.gameObject);	
	}

	void SpawnClearParticles()
	{
		ParticleController particles = Instantiate(ParticleDB.Instance.settledBlockDestroyParticles);
		Vector3 particlesPosition = this.transform.position;
		particlesPosition.z = -2;
		particles.transform.position = particlesPosition;
		particles.EnableParticleSystemOnce();
	}
}
