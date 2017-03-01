using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPowerup
{
	IEnumerator ActivateEffect();
}


public class FreezeTime : IPowerup
{
	const float freezeTime = 5f;

	public IEnumerator ActivateEffect()
	{
		float timePassed = 0;
		
		FigureController.frozen = true;
		while (timePassed < freezeTime && FigureController.frozen)
		{
			timePassed += Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}
		FigureController.frozen = false;
		yield break;
	}
}

public class Bomb : IPowerup
{
	bool bombDetonated = false;
	
	public IEnumerator ActivateEffect()
	{
		/*CAREFUL!!! - If the coroutine is stopped before this can fully execute, the methods will not
		  unsubscribe from the Grid events, causing a memory leak! */
		FigureSpawner.EFigureDropped += ActivateBomb;
		while (!bombDetonated)
			yield return new WaitForFixedUpdate();

		yield break;
	}

	void ActivateBomb()
	{
		FigureSpawner.EFigureDropped -= ActivateBomb;
		Grid.ENewFigureSettled += DetonateBomb;
	}

	void DetonateBomb(Rect bombFigureDimensions)
	{
		Grid.ENewFigureSettled -= DetonateBomb;

		int bombStartX = Mathf.Clamp((int)bombFigureDimensions.xMin - 1, 0,Grid.Instance.maxX);
		int bombStartY = Mathf.Clamp((int)bombFigureDimensions.yMin - 1, 0, Grid.Instance.maxY);
		int bombEndX = Mathf.Clamp((int)bombFigureDimensions.xMax + 1, 0, Grid.Instance.maxX);
		int bombEndY = Mathf.Clamp((int)bombFigureDimensions.yMax + 1, 0, Grid.Instance.maxY);

		Grid.Instance.ClearArea(bombStartX,bombStartY,bombEndX,bombEndY);
		bombDetonated = true;
	}

}

public class Change : IPowerup
{
	public IEnumerator ActivateEffect()
	{
		FigureSpawner.Instance.ChangeNextFigure();
		yield break;
	}
}
