using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDSingleAudioItem : Magicolo.AudioTools.SingleAudioItem {
		
		public PDPlayer pdPlayer;
		
		public PDSingleAudioItem(string name, int id, AudioSource audioSource, AudioInfo audioInfo, GameObject gameObject, CoroutineHolder coroutineHolder, PDGainManager gainManager, PDAudioItemManager itemManager, PDPlayer pdPlayer)
			: base(name, id, audioSource, audioInfo, gameObject, coroutineHolder, gainManager, itemManager, pdPlayer) {
			
			this.pdPlayer = pdPlayer;
			pdPlayer.communicator.SendValue(Name + "_Volume", Volume);
		}

		protected override void UpdateVolume() {
			base.UpdateVolume();
			
			pdPlayer.communicator.SendValue(Name + "_Volume", Volume);
		}
		
		protected override void UpdatePitch() {
			base.UpdateVolume();
			
			pdPlayer.communicator.SendValue(Name + "_Pitch", Pitch);
		}
	}
}
