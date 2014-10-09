using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioItemManager {
		
		public List<SingleAudioItem> activeAudioItems = new List<SingleAudioItem>();
		public List<GameObject> activeAudioObjects = new List<GameObject>();
		public List<GameObject> inactiveAudioObjects = new List<GameObject>();
		
		public int idCounter;
		public AudioListener listener;
		public AudioInfoManager infoManager;
		public Magicolo.AudioTools.Player player;

		public AudioItemManager(AudioListener listener, AudioInfoManager infoManager, Magicolo.AudioTools.Player player) {
			this.listener = listener;
			this.infoManager = infoManager;
			this.player = player;
		}
		
		public virtual void Update() {
			foreach (AudioItem audioItem in activeAudioItems.ToArray()) {
				audioItem.Update();
			}
		}
		
		public virtual void Deactivate(SingleAudioItem audioItem) {
			activeAudioItems.Remove(audioItem);
			activeAudioObjects.Remove(audioItem.gameObject);
			inactiveAudioObjects.Add(audioItem.gameObject);
			audioItem.gameObject.transform.parent = player.transform;
			audioItem.gameObject.SetActive(false);
		}
		
		public void LimitVoices() {
			if (activeAudioItems.Count > player.maxVoices) {
				foreach (SingleAudioItem audioItem in activeAudioItems.ToArray()) {
					if (!audioItem.audioInfo.doNotKill) {
						audioItem.StopImmediate();
						
						if (activeAudioItems.Count <= player.maxVoices) {
							break;
						}
					}
				}
			}
		}
		
		public AudioSource GetAudioSource(AudioInfo audioInfo, GameObject source) {
			GameObject gameObject = GetGameObject(source);
			return SetAudioSource(gameObject.GetOrAddComponent<AudioSource>(), audioInfo);
		}
		
		public GameObject GetGameObject(GameObject source) {
			GameObject gameObject;
			
			gameObject = inactiveAudioObjects.Count == 0 ? new GameObject() : inactiveAudioObjects.Pop();
			gameObject.transform.parent = source == null ? listener.transform : source.transform;
			gameObject.transform.Reset();
			gameObject.SetActive(true);
			activeAudioObjects.Add(gameObject);
			
			return gameObject;
		}
	
		public AudioSource SetAudioSource(AudioSource audioSource, AudioInfo audioInfo) {
			audioSource.Copy(audioInfo.Source);
			return audioSource;
		}
	}
}
