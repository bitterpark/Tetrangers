using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionCompletedScreen: MissionSubscreen
{
	//public event UnityEngine.Events.UnityAction EMissionCompteledButtonpressed;


	[SerializeField]
	Transform rewardsGroup;
	[SerializeField]
	RewardView rewardViewPrefab;

	public void OpenSubscreen(params Reward[] rewards)
	{
		base.OpenSubscreen();
		foreach (Reward reward in rewards)
			AddReward(reward);
	}

	void AddReward(Reward reward)
	{
		RewardView newView = Instantiate(rewardViewPrefab);
		newView.SetDisplayValues(reward.rewardName, reward.rewardSprite, reward.rewardQuantity, reward.extraText);
		newView.transform.SetParent(rewardsGroup);
	}

	protected override void CloseSubscreen()
	{
		foreach (RewardView view in rewardsGroup.GetComponentsInChildren<RewardView>())
			GameObject.Destroy(view.gameObject);
		base.CloseSubscreen();
	}
}

