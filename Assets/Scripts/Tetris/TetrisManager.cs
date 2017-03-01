using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisManager : Singleton<TetrisManager> 
{
	public delegate void EmptyDeleg();
	public static event EmptyDeleg ETetrisStarted;
	public static event EmptyDeleg ETetrisFinished;


	public void StartTetris()
	{
		Grid.Instance.Initialize();
		if (ETetrisStarted != null)
			ETetrisStarted();
	}
}
