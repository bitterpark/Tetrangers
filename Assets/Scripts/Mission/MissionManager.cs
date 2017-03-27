using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class MissionManager : Singleton<MissionManager> 
{
	[SerializeField]
	MissionProgressScreen missionProgressScreen;
	[SerializeField]
	MissionCompletedScreen missionFailedScreen;
	[SerializeField]
	MissionCompletedScreen missionWonScreen;

	[SerializeField]
	BattleManager battleManager;

	public static event UnityAction EMissionWon;
	public static event UnityAction EMissionFailed;
	public static event UnityAction EMissionStarted;
	public static event UnityAction EReturnToBaseToggled;

	public List<Mission> availableMissions { get; private set; }

	Mission currentMission;
	int shipsDefeatedInCurrentMission;

	void Awake()
	{
		MissionsScreen.ENewMissionStartToggled += OpenNewMission;
		BattleManager.EBattleWon += ProgressCurrentMission;
		BattleManager.EBattleLost += FailCurrentMission;

		availableMissions = new List<Mission>();
		availableMissions.Add(new Mission("Defeat all enemy ships", 3));
		availableMissions.Add(new Mission("Defeat some enemy ships", 3));
	}

	public void OpenNewMission(Mission openedMission)
	{
		currentMission = openedMission;
		shipsDefeatedInCurrentMission = 0;
		OpenMissionProgressScreen();
		if (EMissionStarted != null) EMissionStarted();

	}

	void ProgressCurrentMission()
	{
		shipsDefeatedInCurrentMission++;
		if (shipsDefeatedInCurrentMission < currentMission.missionLength)
			OpenMissionProgressScreen();
		else
			WinCurrentMission();
	}

	void OpenMissionProgressScreen()
	{
		missionProgressScreen.OpenSubscreen(
			shipsDefeatedInCurrentMission
			,currentMission.missionLength
			, currentMission.enemyShips[shipsDefeatedInCurrentMission].shipName);
		MissionSubscreen.EContinuePressed += TriggerNextBattle;
	}

	void TriggerNextBattle()
	{
		MissionSubscreen.EContinuePressed -= TriggerNextBattle;
		battleManager.StartNewBattle(GameDataManager.Instance.playerShip, currentMission.enemyShips[shipsDefeatedInCurrentMission]);
	}

	void WinCurrentMission()
	{
		if (EMissionWon != null) EMissionWon();
		foreach (Reward reward in currentMission.rewards)
			reward.GainReward();
		missionWonScreen.OpenSubscreen();
		missionWonScreen.AddMissionRewards(currentMission.rewards);
		MissionSubscreen.EContinuePressed += HandleMissionClosing;
	}
	
	void FailCurrentMission()
	{
		if (EMissionFailed != null) EMissionFailed();
		missionFailedScreen.OpenSubscreen();
		MissionSubscreen.EContinuePressed += HandleMissionClosing;
	}

	void HandleMissionClosing()
	{
		MissionSubscreen.EContinuePressed -= HandleMissionClosing;
		if (EReturnToBaseToggled != null) EReturnToBaseToggled();
	}
}

public struct Mission
{
	public EnemyShipModel[] enemyShips;
	public string description;

	public List<Reward> rewards;

	public int missionLength { get { return enemyShips.Length; } }

	public Mission(string description, int enemyShipCount)
	{
		enemyShips = new EnemyShipModel[enemyShipCount];
		for (int i = 0; i < enemyShipCount; i++)
			enemyShips[i] = EnemyShipModel.GetEnemyShipModelInstance();
		this.description = description;
		rewards = new List<Reward>();
		GenerateRewards();
	}

	public Mission(string description, params EnemyShipModel[] enemyShips)
	{
		this.enemyShips = enemyShips;
		this.description = description;
		rewards = new List<Reward>();
		GenerateRewards();
	}

	void GenerateRewards()
	{
		System.Array rewardsTypes = System.Enum.GetValues(typeof(RewardType));
		for (int i = 0; i < 2; i++)
		{
			RewardType randomType = (RewardType)rewardsTypes.GetValue(Random.Range(0, rewardsTypes.Length));
			rewards.Add(Reward.GetRewardOfType(randomType));
		}
	}
}

public enum RewardType { Intel, Materials };

public struct Reward
{
	public string rewardName;
	public Sprite rewardSprite;
	public int rewardQuantity;
	System.Action<int> rewardGainAction;

	public static Reward GetRewardOfType(RewardType type)
	{
		if (type == RewardType.Intel)
			return new Reward("Intel", SpriteDB.Instance.intelSprite, 100, (int quantity) => { GameDataManager.Instance.ChangeIntel(quantity); });
		if (type == RewardType.Materials)
			return new Reward("Materials", SpriteDB.Instance.materialsSprite, 200, (int quantity) => { GameDataManager.Instance.ChangeMaterials(quantity); });

		return new Reward();
	}


	Reward(string name, Sprite sprite, int quantity, System.Action<int> gainAction)
	{
		rewardName = name;
		rewardSprite = sprite;
		rewardQuantity = quantity;
		rewardGainAction = gainAction;
	}



	public void GainReward()
	{
		rewardGainAction.Invoke(rewardQuantity);
	}

}
