using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MissionsScreen: BaseSubscreen
{
	public static event UnityAction ELeaveBaseScreenToggled;
	public static event UnityAction<Mission> ENewMissionStartToggled;

	[SerializeField]
	Image startMissionPanel;

	[SerializeField]
	Transform missionViewGroup;

	[SerializeField]
	MissionView missionViewPrefab;

	protected override void ExtenderOnAwake()
	{
		//startMissionButton.onClick.AddListener(StartNewMission);
	}

	public override void OpenSubscreen()
	{
		base.OpenSubscreen();
		startMissionPanel.gameObject.SetActive(true);
		foreach (Mission mission in MissionManager.Instance.availableMissions)
			AddMissionView(mission);
	}

	void AddMissionView(Mission mission)
	{
		MissionView newMissionView = Instantiate(missionViewPrefab);
		newMissionView.SetDisplayValues(mission.description, mission.rewards.ToArray());
		newMissionView.transform.SetParent(missionViewGroup);
		newMissionView.EMissionStartButtonPressed += ()=>StartNewMissionPressed(mission);
	}

	void StartNewMissionPressed(Mission startedMission)
	{
		CloseSubscreen();
		if (ELeaveBaseScreenToggled!=null) ELeaveBaseScreenToggled();
		if (ENewMissionStartToggled != null) ENewMissionStartToggled(startedMission);
		
	}

	protected override void CloseSubscreen()
	{
		foreach (MissionView view in missionViewGroup.GetComponentsInChildren<MissionView>())
			view.Dispose();
		startMissionPanel.gameObject.SetActive(false);
		base.CloseSubscreen();
		
	}

}

