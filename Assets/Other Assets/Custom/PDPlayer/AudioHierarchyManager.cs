using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[ExecuteInEditMode]
	public class AudioHierarchyManager : MonoBehaviour {

		public string audioClipsPath;
		
		AudioSource[] audioSources = new AudioSource[0];
		AudioClip[] currentAudioClips = new AudioClip[0];
		AudioClip[] audioClips = new AudioClip[0];
		
		void Update() {
			LoadAudioClips();
			CreateHierarchy();
		}

		void CreateHierarchy() {
			SetCurrentAudioClips();
			
			foreach (AudioClip audioClip in audioClips) {
				if (!currentAudioClips.Contains(audioClip)){
					GameObject child = new GameObject(audioClip.name);
					AudioInfo audioInfo = child.GetOrAddComponent<AudioInfo>();
					audioInfo.Clip = audioClip;
					child.transform.parent = transform;
				}
			}
		}
		
		void LoadAudioClips() {
			audioClips = Resources.LoadAll<AudioClip>(audioClipsPath);
		}

		void SetCurrentAudioClips() {
			audioSources = GetComponentsInChildren<AudioSource>();
			currentAudioClips = new AudioClip[audioSources.Length];
			
			for (int i = 0; i < audioSources.Length; i++) {
				currentAudioClips[i] = audioSources[i].clip;
			}
		}
	}
}
