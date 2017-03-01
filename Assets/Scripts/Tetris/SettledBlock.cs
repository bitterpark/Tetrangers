using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettledBlock : StaticBlock {


	public void ClearBlock()
	{
		SpawnClearParticles();
		GameObject.Destroy(this.gameObject);
	}
	
	void SpawnClearParticles()
	{
		ParticleController particles = Instantiate(ParticleDB.Instance.settledBlockDestroyParticles);
		particles.transform.position = this.transform.position;
		particles.EnableParticleSystemOnce();
	}
}
