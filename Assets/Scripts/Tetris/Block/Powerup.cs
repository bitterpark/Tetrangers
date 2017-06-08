using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum PowerupType {Bomb, Change, LineClear, Damage};

public abstract class Powerup
{
	protected int x;
	protected int y;

	public static void PreparePowerup(PowerupType type, int powerupX, int powerupY)
	{
		Powerup powerup = GetPowerupInstance(type);
		powerup.x = powerupX;
		powerup.y = powerupY;
	}

	static Powerup GetPowerupInstance(PowerupType type)
	{
		switch (type)
		{
			case PowerupType.Bomb: return new Bomb();
			case PowerupType.Change: return new Change();
			case PowerupType.Damage: return new Damage();
			case PowerupType.LineClear: return new LineClear();
		}

		Debug.LogErrorFormat("Could not get powerup instance from type {0}", type);
		return null;
	}

	public static Sprite GetPowerupSprite(PowerupType type)
	{
		switch (type)
		{
			case PowerupType.Bomb: return SpriteDB.Instance.bombSprite;
			case PowerupType.Change: return SpriteDB.Instance.changeSprite;
			case PowerupType.Damage: return SpriteDB.Instance.damageSprite;
			case PowerupType.LineClear: return SpriteDB.Instance.lineClearSprite;
		}
		return null;
	}

	public Powerup()
	{
		FigureSettler.ETogglePowerupEffects += ActivatePowerup;
	}

	void ActivatePowerup(List<IEnumerator> powerupRoutines)
	{
		FigureSettler.ETogglePowerupEffects -= ActivatePowerup;
		powerupRoutines.Add(UsePowerup());
	}

	public abstract IEnumerator UsePowerup();

	
}


public class Bomb : Powerup
{
	//bool bombDetonated = false;

	public override IEnumerator UsePowerup()
	{
		//FigureSettler.ENewFigureSettled -= DetonateBomb;

		int bombStartX = Mathf.Clamp((int)x - 1, 0, Grid.Instance.maxX);
		int bombStartY = Mathf.Clamp((int)y - 1, 0, Grid.Instance.maxY);
		int bombEndX = Mathf.Clamp((int)x + 1, 0, Grid.Instance.maxX);
		int bombEndY = Mathf.Clamp((int)y + 1, 0, Grid.Instance.maxY);

		return Clearer.Instance.ClearAreaRoutine(bombStartX, bombStartY, bombEndX, bombEndY, true);
		//bombDetonated = true;
	}
	/*
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
	*/
}

public class Change : Powerup
{
	public override IEnumerator UsePowerup()
	{
		FigureSpawner.Instance.ChangeNextFigure();
		yield break;
	}

}

public class Damage: Powerup
{
	int damage = 50;

	public override IEnumerator UsePowerup()
	{
		int mySegmentIndex = Grid.Instance.GetSegmentIndexFromCoords(x, y);
		if (mySegmentIndex != -1)
			PlayerShipModel.main.shipSectors[mySegmentIndex].healthManager.TakeDamage(damage);
		yield break;
	}
}

public class LineClear : Powerup
{
	public override IEnumerator UsePowerup()
	{
		return Clearer.Instance.ClearRowsRoutine(y);
	}
}