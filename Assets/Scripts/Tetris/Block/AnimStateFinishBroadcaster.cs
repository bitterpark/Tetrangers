using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnetimeAnimStateFinishBroadcaster : StateMachineBehaviour
{

	public delegate void EmptyDeleg();
	public event EmptyDeleg EStateFinished;

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (EStateFinished != null)
			EStateFinished();
		EStateFinished = null;
	}
}