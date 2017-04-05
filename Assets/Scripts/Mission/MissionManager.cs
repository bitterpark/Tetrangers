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
		GenerateNewMissions();
	}

	public void OpenNewMission(Mission openedMission)
	{
		currentMission = openedMission;
		GenerateNewMissions();
		shipsDefeatedInCurrentMission = 0;
		
		OpenMissionProgressScreen();
		if (EMissionStarted != null) EMissionStarted();
	}

	void GenerateNewMissions()
	{
		availableMissions.Clear();
		availableMissions.Add(new Mission("Defeat all enemy ships", 1));
		availableMissions.Add(new Mission("Complete a sector patrol", 1));
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
		missionWonScreen.OpenSubscreen(currentMission.rewards.ToArray());
		MissionSubscreen.EContinuePressed += HandleMissionClosing;
	}
	
	void FailCurrentMission()
	{
		if (EMissionFailed != null) EMissionFailed();
		Reward missionFailurePenalty = Reward.GetLoseMissionPenalty();
		missionFailurePenalty.GainReward();
		missionFailedScreen.OpenSubscreen(missionFailurePenalty);
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
		System.Array allRewardTypes = System.Enum.GetValues(typeof(RewardType));

		for (int i = 0; i < 2; i++)
		{
			//Has to do this every iteration to reassess if blueprints are available (if first iteration snags the last eligible researchTopic
			List<RewardType> availableRewardTypes = new List<RewardType>();
			foreach (object type in allRewardTypes)
			{
				RewardType castType = (RewardType)type;
				if (Reward.CanGetRewardOfType(castType))
					availableRewardTypes.Add(castType);
			}

			RewardType randomType = availableRewardTypes[Random.Range(0, availableRewardTypes.Count)];
			rewards.Add(Reward.GetRewardOfType(randomType));
		}
	}
}

public enum RewardType { Intel, Materials, Blueprint };

public struct Reward
{
	public string rewardName;
	public string extraText;
	public Sprite rewardSprite;
	public int rewardQuantity;
	System.Action<int> rewardGainAction;

	public static Reward GetLoseMissionPenalty()
	{
		return new Reward("Materials", SpriteDB.Instance.materialsSprite, -400,
			(int quantity)=> { GameDataManager.Instance.ChangeMaterials(quantity); });
	}

	public static bool CanGetRewardOfType(RewardType type)
	{
		if (type == RewardType.Blueprint)
			return GameDataManager.Instance.playerResearch.HasUnresearchedTopics();
		else return true;
	}

	public static Reward GetRewardOfType(RewardType type)
	{
		if (type == RewardType.Intel)
			return new Reward("Intel", SpriteDB.Instance.intelSprite, 100, (int quantity) => { GameDataManager.Instance.ChangeIntel(quantity); });
		if (type == RewardType.Materials)
			return new Reward("Materials", SpriteDB.Instance.materialsSprite, 200, (int quantity) => { GameDataManager.Instance.ChangeMaterials(quantity); });
		if (type == RewardType.Blueprint)
			return CreateBlueprintReward();

		return new Reward();
	}

	static Reward CreateBlueprintReward()
	{
		List<ResearchTopic> allUnresearchedTopics = GameDataManager.Instance.playerResearch.GetUnresearchedTopics();
		if (allUnresearchedTopics.Count>0)
		{
			const int blueprintIntelGain = 200;

			ResearchTopic randomTopic = allUnresearchedTopics[Random.Range(0,allUnresearchedTopics.Count)];
			return new Reward("Blueprint", randomTopic.providesEquipment.name, SpriteDB.Instance.blueprintSprite, blueprintIntelGain,
				(int quantity) => { randomTopic.InvestIntel(quantity); }
				);
		}
		else
			return new Reward();
	}

	Reward(string name, Sprite sprite, int quantity, System.Action<int> gainAction)
	{
		rewardName = name;
		rewardSprite = sprite;
		rewardQuantity = quantity;
		rewardGainAction = gainAction;
		extraText = "";
	}

	Reward(string name, string extraText, Sprite sprite, int quantity, System.Action<int> gainAction)
	{
		rewardName = name;
		rewardSprite = sprite;
		rewardQuantity = quantity;
		rewardGainAction = gainAction;
		this.extraText = extraText;
	}

	public void GainReward()
	{
		rewardGainAction.Invoke(rewardQuantity);
	}

}
