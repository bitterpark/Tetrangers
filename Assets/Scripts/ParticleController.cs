using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleController : MonoBehaviour 
{

	public void EnableParticleSystem()
	{
		GetComponent<ParticleSystem>().Play();
	}

	public void EnableParticleSystemOnce()
	{
		ParticleSystem mySystem = GetComponent<ParticleSystem>();
		mySystem.Play();
		StartCoroutine(DisposeSystem(mySystem.main.duration));
	}

	IEnumerator DisposeSystem(float lifetime)
	{
		yield return new WaitForSeconds(lifetime);
		GameObject.Destroy(this.gameObject);
		yield break;
	}

}
