using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionView : MonoBehaviour {

	public event UnityEngine.Events.UnityAction EMissionStartButtonPressed;

	[SerializeField]
	Text descriptionText;
	[SerializeField]
	Transform rewardsGroup;
	[SerializeField]
	RewardView rewardViewPrefab;
	[SerializeField]
	Button missionStartButton;


	public void SetDisplayValues(string missionDescription, params Reward[] rewards)
	{
		descriptionText.text = missionDescription;
		foreach (Reward reward in rewards)
			AddReward(reward);
	}

	void AddReward(Reward reward)
	{
		RewardView newView = Instantiate(rewardViewPrefab);
		newView.SetDisplayValues(reward.rewardName,reward.rewardSprite,reward.rewardQuantity, reward.extraText);
		newView.transform.SetParent(rewardsGroup);
	}

	void Awake()
	{
		missionStartButton.onClick.AddListener(() => { if (EMissionStartButtonPressed != null) EMissionStartButtonPressed(); });
	}


	public void Dispose()
	{
		EMissionStartButtonPressed = null;
		missionStartButton.onClick.RemoveAllListeners();
		GameObject.Destroy(this.gameObject);
	}

}
