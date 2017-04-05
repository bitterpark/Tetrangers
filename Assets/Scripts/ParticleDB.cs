using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDB : Singleton<ParticleDB> {

	[SerializeField]
	ParticleController figureSettledParticles;
	[SerializeField]
	ParticleController shipGotHitParticles;
	[SerializeField]
	ParticleController rowClearParticles;

	public void CreateShipGotHitParticles(Vector3 worldPosition)
	{
		CreateParticles(shipGotHitParticles, worldPosition);
	}

	public void CreateSettledFigureParticles(Vector3 worldPosition)
	{
		CreateParticles(figureSettledParticles,worldPosition);
	}

	public void CreateRowClearParticles(Vector3 worldPosition)
	{
		CreateParticles(rowClearParticles, worldPosition);
	}

	void CreateParticles(ParticleController usedPrefab, Vector3 worldPosition)
	{
		ParticleController particles = Instantiate(usedPrefab);
		Vector3 particlesPosition = worldPosition;
		particlesPosition.z = -2;
		particles.transform.position = particlesPosition;
		particles.EnableParticleSystemOnce();
	}

}
