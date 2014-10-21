using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioPlayerItemManager : AudioItemManager {
		
		public AudioPlayer audioPlayer;
		
		public AudioPlayerItemManager(AudioPlayer audioPlayer)
			: base(audioPlayer.listener, audioPlayer.infoManager, audioPlayer) {
			
			this.audioPlayer = audioPlayer;
		}

		public AudioItem Play(string soundName, GameObject source, float delay, SyncMode syncMode, params AudioOption[] audioOptions) {
			AudioItem audioItem = GetAudioItem(soundName, source, audioOptions);
			if (syncMode != SyncMode.None) {
				audioItem.Play(delay, syncMode);
			}
			else {
				audioItem.Play(delay);
			}
			return audioItem;
		}
		
		public AudioItem Play(string soundName, GameObject source, params AudioOption[] audioOptions) {
			AudioItem audioItem = GetAudioItem(soundName, source, audioOptions);
			audioItem.Play();
			return audioItem;
		}
		
		public AudioItem GetAudioItem(string soundName, GameObject source, params AudioOption[] audioOptions) {
			AudioInfo audioInfo = infoManager.GetAudioInfo(soundName);
			AudioSource audioSource = GetAudioSource(audioInfo, source, audioOptions);
			
			GameObject gameObject = audioSource.gameObject;
			CoroutineHolder coroutineHolder = gameObject.GetOrAddComponent<CoroutineHolder>();
			
			GainManager gainManager = gameObject.GetOrAddComponent<GainManager>();
			idCounter += 1;
			SingleAudioItem audioItem = new SingleAudioItem(soundName, idCounter, audioSource, audioInfo, gameObject, coroutineHolder, gainManager, this, audioPlayer);
			
			gainManager.Initialize(audioItem, audioPlayer);
			audioItem.Update();
			player.metronome.Subscribe(audioItem);
			inactiveAudioItems.Add(audioItem);
			
			return audioItem;
		}
	}
}
