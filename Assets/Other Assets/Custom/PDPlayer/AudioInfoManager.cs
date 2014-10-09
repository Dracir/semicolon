using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioInfoManager : Magicolo.GeneralTools.IStartable {

		public Magicolo.AudioTools.Player player;
		
		readonly Dictionary<string, AudioInfo> audioInfos = new Dictionary<string, AudioInfo>();
		
		public AudioInfoManager(Magicolo.AudioTools.Player player) {
			this.player = player;
		}
		
		public void Start() {
			BuildAudioInfoDict();
		}
		
		public AudioInfo GetAudioInfo(string key) {
			return audioInfos[key];
		}

		void BuildAudioInfoDict() {
			foreach (AudioInfo audioInfo in Object.FindObjectsOfType<AudioInfo>()) {
				audioInfos[audioInfo.Clip.name] = audioInfo;
			}
		}
	}
}
