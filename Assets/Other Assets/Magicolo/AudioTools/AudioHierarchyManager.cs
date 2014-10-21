using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioHierarchyManager : IStartable {

		public string audioClipsPath;
		
		public AudioOptions[] audioOptions = new AudioOptions[0];
		public AudioSource[] audioSources = new AudioSource[0];
		public AudioClip[] currentAudioClips = new AudioClip[0];
		public AudioClip[] audioClips = new AudioClip[0];
		public List<GameObject> folderStructure = new List<GameObject>();
		
		public AudioPlayer audioPlayer;
		public GeneralAudioSettings audioSettings;

		Dictionary<string, AudioInfo> audioInfos;
		
		public AudioHierarchyManager(AudioPlayer audioPlayer) {
			this.audioPlayer = audioPlayer;
			audioSettings = audioPlayer.audioSettings;
		}
		
		public void Start() {
			if (Application.isPlaying) {
				BuildAudioInfoDict();
				FreezeHierarchy();
				audioPlayer.SetChildrenActive(false);
			}
		}
		
		public void Update() {
			UpdateAudioOptions();
			UpdateHierarchy();
		}

		void UpdateAudioOptions() {
			audioOptions = audioPlayer.GetComponentsInChildren<AudioOptions>();
			
			foreach (AudioOptions options in audioOptions) {
				options.Update();
			}
		}
		
		void UpdateHierarchy() {
			SetCurrentAudioClips();
			CreateHierarchy();
			EnsureUniqueNames();
			RemoveEmptyFolders();
			FreezeHierarchy();
			audioPlayer.gameObject.SortChildrenRecursive();
		}

		void CreateHierarchy() {
			#if UNITY_EDITOR
			foreach (AudioClip audioClip in audioClips) {
				string audioClipPath = UnityEditor.AssetDatabase.GetAssetPath(audioClip).TrimStart(("Assets/Resources/" + audioClipsPath).ToCharArray());
				string audioClipDirectory = Path.GetDirectoryName(audioClipPath);
				GameObject parent = GetOrAddFolder(audioClipDirectory);
				GameObject child = audioPlayer.gameObject.FindChildRecursive(audioClip.name);
				if (child == null) {
					child = new GameObject(audioClip.name);
					AudioOptions options = child.GetOrAddComponent<AudioOptions>();
					AudioSource source = options.GetOrAddComponent<AudioSource>();
					source.playOnAwake = false;
					source.clip = audioClip;
					options.audioInfo = new AudioInfo(source, options, audioPlayer);
					options.hierarchyManager = this;
					options.audioPlayer = audioPlayer;
				}
				child.transform.parent = parent.transform;
				child.transform.Reset();
			}
			#endif
		}
		
		GameObject GetOrAddFolder(string directory) {
			string[] folderNames = directory.Split(Path.AltDirectorySeparatorChar);
			GameObject parent = audioPlayer.gameObject;
			GameObject folder = audioPlayer.gameObject;
			
			foreach (string folderName in folderNames) {
				if (string.IsNullOrEmpty(folderName)) {
					continue;
				}
				
				folder = parent.FindChild(folderName);
				if (folder == null) {
					folder = new GameObject(folderName);
					folder.transform.parent = parent.transform;
					folderStructure.Add(folder);
				}
				parent = folder;
			}
			return parent;
		}

		void RemoveEmptyFolders() {
			foreach (GameObject folder in folderStructure.ToArray()) {
				if (folder != null) {
					if (folder.transform.childCount == 0) {
						RemoveEmptyFolder(folder);
					}
				}
			}
		}
		
		void RemoveEmptyFolder(GameObject folder) {
			Transform parent = folder.transform.parent;
			
			if (parent != null && parent.childCount == 1 && parent != audioPlayer.transform) {
				folderStructure.Remove(folder);
				RemoveEmptyFolder(folder.transform.parent.gameObject);
			}
			else {
				folderStructure.Remove(folder);
				folder.Remove();
			}
		}

		void EnsureUniqueNames() {
			foreach (AudioOptions options in audioOptions) {
				options.SetUniqueName(options.Name, "", audioOptions);
			}
		}

		void FreezeHierarchy() {
			audioPlayer.transform.hideFlags = HideFlags.HideInInspector;
			audioPlayer.transform.Reset();
			foreach (GameObject child in audioPlayer.gameObject.GetChildrenRecursive()) {
				child.transform.hideFlags = HideFlags.HideInInspector;
				child.transform.Reset();
			}
		}
		
		void SetCurrentAudioClips() {
			audioClips = Resources.LoadAll<AudioClip>(audioClipsPath);
			audioSources = audioPlayer.GetComponentsInChildren<AudioSource>();
			currentAudioClips = new AudioClip[audioSources.Length];
			
			for (int i = 0; i < audioSources.Length; i++) {
				currentAudioClips[i] = audioSources[i].clip;
			}
			
			audioPlayer.SortChildrenRecursive();
		}
	
		void BuildAudioInfoDict() {
			audioInfos = new Dictionary<string, AudioInfo>();
			
			foreach (AudioOptions audioOptions in Object.FindObjectsOfType<AudioOptions>()) {
				audioInfos[audioOptions.Name] = audioOptions.audioInfo;
			}
		}
		
		public AudioInfo GetAudioInfo(string key) {
			return  new AudioInfo(audioInfos[key]);
		}
	}
}
