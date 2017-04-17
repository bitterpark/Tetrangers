using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineExtension
{
	// для отслеживания используем словарь <название группы, количество работающих корутинов>
	static private readonly Dictionary<string, int> Runners = new Dictionary<string, int>();

	public static int GetLatestAvailableIndex()
	{
		return Runners.Count;
	}

	public static int LaunchInParallelCoroutinesGroup(this IEnumerator coroutine, MonoBehaviour parent)
	{
		int latestAvailableIndex = Runners.Count;
		LaunchInParallelCoroutinesGroup(coroutine, parent, latestAvailableIndex.ToString());
		return latestAvailableIndex;
	}

	// MonoBehaviour нам нужен для запуска корутина в контексте вызывающего класса
	public static void LaunchInParallelCoroutinesGroup(this IEnumerator coroutine, MonoBehaviour parent, string groupName)
	{
		if (!Runners.ContainsKey(groupName))
			Runners.Add(groupName, 0);

		Runners[groupName]++;
		parent.StartCoroutine(DoParallel(coroutine, parent, groupName));
	}


	static IEnumerator DoParallel(IEnumerator coroutine, MonoBehaviour parent, string groupName)
	{
		yield return parent.StartCoroutine(coroutine);
		Runners[groupName]--;
	}

	// эту функцию используем, что бы узнать, есть ли в группе незавершенные корутины
	public static bool GroupIsProcessing(string groupName)
	{
		return (Runners.ContainsKey(groupName) && Runners[groupName] > 0);
	}
}
