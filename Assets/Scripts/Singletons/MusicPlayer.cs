using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : Singleton<MusicPlayer>
{

	[SerializeField]
	AudioSource player;

	[SerializeField]
	List<AudioClip> baseTracks = new List<AudioClip>();

	[SerializeField]
	List<AudioClip> combatTracks = new List<AudioClip>();

	int lastBaseTrackIndex = 0;
	int lastCombatTrackIndex = 0;

	void Awake()
	{
		lastBaseTrackIndex = Random.Range(0, baseTracks.Count);
		lastCombatTrackIndex = Random.Range(0, combatTracks.Count);
	}

	public void PlayNextBaseTrack()
	{
		if (player.isPlaying) player.Stop();

		Debug.Assert(baseTracks.Count > 0, "No base tracks to play!");

		int currentBaseTrackIndex = (int) Mathf.Repeat(lastBaseTrackIndex + 1, baseTracks.Count);
		player.clip = baseTracks[currentBaseTrackIndex];
		player.Play();
		lastBaseTrackIndex = currentBaseTrackIndex;
	}

	public void PlayNextCombatTrack()
	{
		if (player.isPlaying) player.Stop();

		Debug.Assert(combatTracks.Count > 0, "No combat tracks to play!");

		int currentCombatTrackIndex = (int) Mathf.Repeat(lastCombatTrackIndex + 1, combatTracks.Count);
		player.clip = combatTracks[currentCombatTrackIndex];
		player.Play();
		lastCombatTrackIndex = currentCombatTrackIndex;
	}

}
