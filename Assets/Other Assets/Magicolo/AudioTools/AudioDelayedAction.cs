using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioDelayedAction : AudioAction {

		public int delay;
		
		public AudioDelayedAction(int delay, ActionTypes type, AudioItem audioItem)
			: base(type, audioItem) {
			this.delay = delay;
		}
		
		public void Tick(){
			if (delay <= 0){
				ApplyAction();
			}
			else {
				delay -= 1;
			}
		}
	}
}
