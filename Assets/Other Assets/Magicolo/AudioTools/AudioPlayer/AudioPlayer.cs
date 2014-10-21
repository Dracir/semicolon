using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;

// FIXME Update the AudioHierarchyManager only when appropriate instead of on every Update

[ExecuteInEditMode]
public class AudioPlayer : Magicolo.AudioTools.Player {

	static AudioPlayer instance;
	static AudioPlayer Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<AudioPlayer>();
			}
			return instance;
		}
	}
	
	#region Components
	public AudioHierarchyManager hierarchyManager;
	public AudioPlayerEditorHelper editorHelper;
	public AudioPlayerItemManager itemManager;
	#endregion
	
	protected override void Awake() {
		base.Awake();
		
		this.SetExecutionOrder(-11);
		editorHelper = AudioPlayerEditorHelper.GetInstance(Instance);
		editorHelper.Update();
		
		audioSettings = audioSettings ?? new GeneralAudioSettings(Instance);
		hierarchyManager = hierarchyManager ?? new AudioHierarchyManager(Instance);
		
		audioSettings.Start();
		
		if (Application.isPlaying) {
			hierarchyManager.Start();
			itemManager = new AudioPlayerItemManager(Instance);
			metronome.Subscribe(itemManager);
			metronome.Play();
		}
	}
	
	protected override void Update() {
		base.Update();
		
		if (Application.isPlaying) {
			itemManager.Update();
		}
		else {
			audioSettings.Update();
			hierarchyManager.Update();
			editorHelper.Update();
		}
	}
		
	public static AudioItem Play(string soundName, GameObject source, float delay, SyncMode syncMode, params AudioOption[] audioOptions) {
		return instance.itemManager.Play(soundName, source, delay, syncMode, audioOptions);
	}
		
	public static AudioItem Play(string soundName, GameObject source, float delay, params AudioOption[] audioOptions) {
		return instance.itemManager.Play(soundName, source, delay, SyncMode.None, audioOptions);
	}
	
	/// <summary>
	/// Plays an audio source spatialized around the <paramref name="source"/>.
	/// </summary>
	/// <param name="soundName">The name of sound to be played.</param>
	/// <param name="source">The source around which the audio source will be spatialized.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="soundName"/> inspector.</param>
	/// <returns>The AudioItem that will let you control the audio source.</returns>
	public static AudioItem Play(string soundName, GameObject source, params AudioOption[] audioOptions) {
		return instance.itemManager.Play(soundName, source, audioOptions);
	}
	
	public static AudioItem Play(string soundName, float delay, SyncMode syncMode, params AudioOption[] audioOptions) {
		return instance.itemManager.Play(soundName, null, delay, syncMode, audioOptions);
	}

	public static AudioItem Play(string soundName, float delay, params AudioOption[] audioOptions) {
		return instance.itemManager.Play(soundName, null, delay, SyncMode.None, audioOptions);
	}

	/// <summary>
	/// Plays an audio source spatialized around the listener.
	/// </summary>
	/// <param name="soundName">The name of sound to be played.</param>
	/// <param name = "audioOptions">Options that will override the default options set in the <paramref name="soundName"/> inspector.</param>
	/// <returns>The AudioItem that will let you control the audio source.</returns>
	public static AudioItem Play(string soundName, params AudioOption[] audioOptions) {
		return instance.itemManager.Play(soundName, null, audioOptions);
	}

	/// <summary>
	/// Gets the master volume of the PDPlayer.
	/// </summary>
	/// <returns>The master volume.</returns>
	public static float GetMasterVolume() {
		return instance.audioSettings.masterVolume;
	}
	
	/// <summary>
	/// Ramps the master volume of the AudioPlayer.
	/// </summary>
	/// <param name="targetVolume">The target to which the volume will be ramped.</param>
	/// <param name="time">The time it will take for the volume to reach the <paramref name="targetVolume"/>.</param>
	public static void SetMasterVolume(float targetVolume, float time) {
		instance.itemManager.SetMasterVolume(targetVolume, time);
	}
	
	/// <summary>
	/// Sets the master volume of the AudioPlayer.
	/// </summary>
	/// <param name="targetVolume">The target to which the volume will be set.</param>
	public static void SetMasterVolume(float targetVolume) {
		instance.itemManager.SetMasterVolume(targetVolume);
	}

	/// <summary>
	/// Gets the tempo settigns.
	/// </summary>
	/// <param name="beatsPerMinute">The number of beat events per minute.</param>
	/// <param name="beatsPerMeasure">The number of beats required before a measure event is triggered.</param>
	public static void GetTempo(out float beatsPerMinute, out int beatsPerMeasure){
		Instance.metronome.GetTempo(out beatsPerMinute, out beatsPerMeasure);
	}
	
	/// <summary>
	/// Sets the tempo settings.
	/// </summary>
	/// <param name="beatsPerMinute">The number of beat events per minute.</param>
	/// <param name="beatsPerMeasure">The number of beats required before a measure event is triggered.</param>
	public static void SetTempo(float beatsPerMinute, int beatsPerMeasure) {
		Instance.metronome.SetTempo(beatsPerMinute, beatsPerMeasure);
	}
}
