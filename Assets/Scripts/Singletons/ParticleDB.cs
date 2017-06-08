using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDB : Singleton<ParticleDB> {

	[SerializeField]
	ParticleController blueClearedBlockParticles;
	[SerializeField]
	ParticleController greenClearedBlockParticles;
	[SerializeField]
	ParticleController shieldClearedBlockParticles;
	[SerializeField]
	ParticleController shipGotHitParticles;
	[SerializeField]
	ParticleController rowClearParticles;

	public void CreateShipGotHitParticles(Vector3 worldPosition)
	{
		CreateParticles(shipGotHitParticles, worldPosition);
	}

	public void CreateClearedBlockParticles(Vector3 worldPosition, BlockType blockType)
	{
		ParticleController particles = null;
		if (blockType == BlockType.Blue)
			particles = blueClearedBlockParticles;
		if (blockType == BlockType.Green)
			particles = greenClearedBlockParticles;
		if (blockType == BlockType.Shield)
			particles = shieldClearedBlockParticles;
		if (blockType == BlockType.ShipEnergy)
			particles = shieldClearedBlockParticles;

		Debug.Assert(particles != null, "Could not find the right cleared block particles!");
		CreateParticles(particles, worldPosition);
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
