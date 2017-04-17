using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPowerup
{
	IEnumerator GetPowerupRoutine();
}

public abstract class Powerup: IPowerup
{

	public abstract IEnumerator GetPowerupRoutine();
	//USE THIS FOR SIGNING ACTIVE POWERUPS UP TO EVENTS, TO MAKE SURE THEY CAN UNSUBSCRIBE EVEN IF THE POWERUP ACTIVATOR STOPS THEM FROM FINISHING
	protected  virtual void InitializePowerup()
	{
		PowerupActivator.EDeactivateAllPowerupRoutines += DeinitializePowerup;
	}
	protected virtual void DeinitializePowerup()
	{
		PowerupActivator.EDeactivateAllPowerupRoutines -= DeinitializePowerup;
	}

}

public class FreezeTime : Powerup
{
	const float freezeTime = 5f;

	public override IEnumerator GetPowerupRoutine()
	{
		float timePassed = 0;
		
		FigureController.frozen = true;
		while (timePassed < freezeTime && FigureController.frozen)
		{
			timePassed += TetrisManager.tetrisDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		FigureController.frozen = false;
		yield break;
	}
}

public class Bomb : Powerup
{
	bool bombDetonated = false;

	protected override void InitializePowerup()
	{
		base.InitializePowerup();
		FigureSpawner.EFigureDropped += ActivateBomb;
	}

	void ActivateBomb()
	{
		FigureSpawner.EFigureDropped -= ActivateBomb;
		FigureSettler.ENewFigureSettled += DetonateBomb;
	}

	protected override void DeinitializePowerup()
	{
		base.DeinitializePowerup();
		FigureSpawner.EFigureDropped -= ActivateBomb;
		FigureSettler.ENewFigureSettled -= DetonateBomb;
	}

	public override IEnumerator GetPowerupRoutine()
	{
		InitializePowerup();
		while (!bombDetonated)
			yield return new WaitForFixedUpdate();
		DeinitializePowerup();
		yield break;
	}

	void DetonateBomb(Rect bombFigureDimensions)
	{
		FigureSettler.ENewFigureSettled -= DetonateBomb;

		int bombStartX = Mathf.Clamp((int)bombFigureDimensions.xMin - 1, 0,Grid.Instance.maxX);
		int bombStartY = Mathf.Clamp((int)bombFigureDimensions.yMin - 1, 0, Grid.Instance.maxY);
		int bombEndX = Mathf.Clamp((int)bombFigureDimensions.xMax + 1, 0, Grid.Instance.maxX);
		int bombEndY = Mathf.Clamp((int)bombFigureDimensions.yMax + 1, 0, Grid.Instance.maxY);

		Grid.Instance.ClearArea(bombStartX,bombStartY,bombEndX,bombEndY);
		bombDetonated = true;
	}

}

public class Change : Powerup
{
	public override IEnumerator GetPowerupRoutine()
	{
		FigureSpawner.Instance.ChangeNextFigure();
		yield break;
	}
}
