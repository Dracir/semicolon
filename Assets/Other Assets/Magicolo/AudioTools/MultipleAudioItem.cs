using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class MultipleAudioItem : AudioItem {
		
		public List<AudioItem> audioItems = new List<AudioItem>();
		
		public MultipleAudioItem(string name, int id, AudioItemManager itemManager, Magicolo.AudioTools.Player player)
			: base(name, id, itemManager, player) {
		}
		
		public override void Update() {
			UpdateAudioItems();
			
			if (RemoveStoppedAudioItems() && State != AudioStates.Stopped) {
				Stop();
			}
		}

		public virtual void UpdateAudioItems() {
			foreach (AudioItem audioItem in audioItems) {
				audioItem.Update();
			}
		}
		
		protected override void UpdateVolume() {
			foreach (AudioItem audioItem in audioItems) {
				audioItem.SetVolume(Volume);
			}
		}
		
		protected override void UpdatePitch() {
			foreach (AudioItem audioItem in audioItems) {
				audioItem.SetPitch(Pitch);
			}
		}
		
		public virtual void AddAudioItem(AudioItem audioItem) {
			audioItems.Add(audioItem);
			UpdateVolume();
			UpdatePitch();
		}

		public virtual bool RemoveStoppedAudioItems() {
			bool allStopped = true;
			
			foreach (AudioItem audioItem in audioItems.ToArray()) {
				if (audioItem != null) {
					if (audioItem.GetState() == AudioStates.Stopped) {
						audioItems.Remove(audioItem);
					}
					else {
						allStopped = false;
					}
				}
			}
			return allStopped;
		}

		public override void Play(float delay, SyncMode syncMode) {
			Play(player.metronome.GetAdjustedDelay(delay, syncMode));
		}
		
		public override void Play(float delay) {
			player.coroutineHolder.AddCoroutine("PlayAfterDelay" + Name + Id, PlayAfterDelay(delay));
		}
		
		public override void Play() {
			base.Play();
				
			foreach (AudioItem audioItem in audioItems) {
				audioItem.Play();
			}
		}

		public override void Pause(float delay, SyncMode syncMode) {
			Pause(player.metronome.GetAdjustedDelay(delay, syncMode));
		}
		
		public override void Pause(float delay) {
			player.coroutineHolder.AddCoroutine("PauseAfterDelay" + Name + Id, PauseAfterDelay(delay));
		}
		
		public override void Pause() {
			base.Pause();
				
			foreach (AudioItem audioItem in audioItems) {
				audioItem.Pause();
			}
		}

		public override void Stop(float delay, SyncMode syncMode) {
			Stop(player.metronome.GetAdjustedDelay(delay, syncMode));
		}
		
		public override void Stop(float delay) {
			player.coroutineHolder.AddCoroutine("StopAfterDelay" + Name + Id, StopAfterDelay(delay));
		}
		
		public override void Stop() {
			base.Stop();
				
			foreach (AudioItem audioItem in audioItems) {
				audioItem.Stop();
			}
		}

		public override void StopImmediate() {
			base.StopImmediate();
			
			foreach (AudioItem audioItem in audioItems) {
				audioItem.StopImmediate();
			}
		}
		
		public override void SetVolume(float targetVolume, float time) {
			player.coroutineHolder.RemoveCoroutines("RampVolume" + Name + Id);
			player.coroutineHolder.AddCoroutine("RampVolume" + Name + Id, RampVolume(Volume, targetVolume, time));
		}
		
		public override void SetVolume(float targetVolume) {
			player.coroutineHolder.RemoveCoroutines("RampVolume" + Name + Id);
			Volume = targetVolume;
			UpdateVolume();
		}

		public override void SetPitch(float targetPitch, float time, float quantizeStep) {
			player.coroutineHolder.RemoveCoroutines("RampPitch" + Name + Id);
			player.coroutineHolder.AddCoroutine("RampPitch" + Name + Id, RampPitch(Pitch, targetPitch, time, quantizeStep));
		}
		
		public override void SetPitch(float targetPitch, float time) {
			player.coroutineHolder.RemoveCoroutines("RampPitch" + Name + Id);
			player.coroutineHolder.AddCoroutine("RampPitch" + Name + Id, RampPitch(Pitch, targetPitch, time, 0));
		}
		
		public override void SetPitch(float targetPitch) {
			player.coroutineHolder.RemoveCoroutines("RampPitch" + Name + Id);
			Pitch = targetPitch;
			UpdatePitch();
		}
	}
}
