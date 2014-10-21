using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioAction {

		public enum ActionTypes {
			Play,
			Pause,
			Stop
		}
		
		public bool isApplied;
		
		public ActionTypes type;
		public AudioItem audioItem;
		
		public AudioAction(AudioAction.ActionTypes type, AudioItem audioItem) {
			this.type = type;
			this.audioItem = audioItem;
		}
		
		public void ApplyAction() {
			if (isApplied) {
				return;
			}
			
			switch (type) {
				case ActionTypes.Play:
					audioItem.Play();
					break;
				case ActionTypes.Pause:
					audioItem.Pause();
					break;
				case ActionTypes.Stop:
					audioItem.Stop();
					break;
			}
			isApplied = true;
		}
	}
}
