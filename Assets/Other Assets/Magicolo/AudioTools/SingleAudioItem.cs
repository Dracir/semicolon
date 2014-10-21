using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class SingleAudioItem : AudioItem, ITickable {

		public AudioSource audioSource;
		public AudioInfo audioInfo;
		public GameObject gameObject;
		public CoroutineHolder coroutineHolder;
		public GainManager gainManager;
		
		public List<AudioDelayedAction> delayedActions = new List<AudioDelayedAction>();
		
		AudioStates pausedState;
		
		public SingleAudioItem(string name, int id, AudioSource audioSource, AudioInfo audioInfo, GameObject gameObject, CoroutineHolder coroutineHolder, GainManager gainManager, AudioItemManager itemManager, Magicolo.AudioTools.Player player)
			: base(name, id, itemManager, player) {
			
			this.audioSource = audioSource;
			this.audioInfo = audioInfo;
			this.gameObject = gameObject;
			this.coroutineHolder = coroutineHolder;
			this.gainManager = gainManager;
			
			Pitch = audioSource.pitch;
		}
		
		public override void Update() {
			if (!audioSource.loop) {
				if ((audioSource.pitch > 0 && audioSource.time >= audioSource.clip.length - audioInfo.fadeOut) || (audioSource.pitch < 0 && audioSource.time <= audioInfo.fadeOut)) {
					Stop();
				}
			}
			gameObject.name = string.Format("{0} ({1})", Name, State);
		}
		
		protected override void UpdateVolume() {
			gainManager.volume = Volume * player.audioSettings.masterVolume;
		}
		
		protected override void UpdatePitch() {
			audioSource.pitch = Pitch;
		}

		public override void Play(float delay, SyncMode syncMode) {
			delayedActions.Add(new AudioDelayedAction(player.metronome.ConvertToBeats(delay, syncMode), AudioAction.ActionTypes.Play, this));
		}
		
		public override void Play(float delay) {
			coroutineHolder.AddCoroutine("PlayAfterDelay", PlayAfterDelay(delay));
		}
		
		public override void Play() {
			if (State == AudioStates.Waiting) {
				//HACK Trick to deal with reversed sounds.
				if (audioSource.pitch < 0) {
					audioSource.time = audioSource.clip.length - 0.00001f;
				}
				coroutineHolder.AddCoroutine("FadeIn", FadeIn(audioSource.volume, audioInfo.fadeIn, audioInfo.fadeInCurve));
			}
			else if (State == AudioStates.Paused) {
				audioSource.Play();
				coroutineHolder.ResumeCoroutines("FadeIn");
				coroutineHolder.ResumeCoroutines("RampVolume");
				coroutineHolder.ResumeCoroutines("RampPitch");
				State = pausedState;
			}
		}
		
		public override void Pause(float delay, SyncMode syncMode) {
			delayedActions.Add(new AudioDelayedAction(player.metronome.ConvertToBeats(delay, syncMode), AudioAction.ActionTypes.Pause, this));
		}
		
		public override void Pause(float delay) {
			coroutineHolder.AddCoroutine("PauseAfterDelay", PauseAfterDelay(delay));
		}
		
		public override void Pause() {
			if (State == AudioStates.Playing || State == AudioStates.FadingIn) {
				audioSource.Pause();
				coroutineHolder.PauseCoroutines("FadeIn");
				coroutineHolder.PauseCoroutines("RampVolume");
				coroutineHolder.PauseCoroutines("RampPitch");
				pausedState = State;
				base.Pause();
			}
		}
		
		public override void Stop(float delay, SyncMode syncMode) {
			delayedActions.Add(new AudioDelayedAction(player.metronome.ConvertToBeats(delay, syncMode), AudioAction.ActionTypes.Stop, this));
		}
		
		public override void Stop(float delay) {
			coroutineHolder.AddCoroutine("StopAfterDelay", StopAfterDelay(delay));
		}
		
		public override void Stop() {
			if (State != AudioStates.Stopped || State != AudioStates.FadingOut) {
				coroutineHolder.AddCoroutine("FadeOut", FadeOut(0, audioInfo.fadeOut, audioInfo.fadeOutCurve));
			}
		}

		public override void StopImmediate() {
			if (State != AudioStates.Stopped) {
				base.Stop();
				audioSource.Stop();
				gainManager.Deactivate();
				itemManager.Deactivate(this);
				coroutineHolder.RemoveAllCoroutines();
			}
		}
		
		public override void SetVolume(float targetVolume, float time) {
			coroutineHolder.RemoveCoroutines("RampVolume");
			coroutineHolder.AddCoroutine("RampVolume", RampVolume(gainManager.volume, Mathf.Clamp(targetVolume, 0, 10), time));
		}

		public override void SetVolume(float targetVolume) {
			coroutineHolder.RemoveCoroutines("RampVolume");
			
			Volume = targetVolume;
			UpdateVolume();
		}

		public override void SetPitch(float targetPitch, float time, float quantizeStep) {
			coroutineHolder.RemoveCoroutines("RampPitch");
			coroutineHolder.AddCoroutine("RampPitch", RampPitch(audioSource.pitch, targetPitch, time, quantizeStep));
		}
		
		public override void SetPitch(float targetPitch, float time) {
			coroutineHolder.RemoveCoroutines("RampPitch");
			coroutineHolder.AddCoroutine("RampPitch", RampPitch(audioSource.pitch, targetPitch, time, 0));
		}
		
		public override void SetPitch(float targetPitch) {
			coroutineHolder.RemoveCoroutines("RampPitch");
			
			Pitch = targetPitch;
			UpdatePitch();
		}

		public virtual void BeatEvent(int currentBeat) {
			foreach (AudioDelayedAction delayedAction in delayedActions.ToArray()) {
				if (delayedAction == null) {
					continue;
				}
				
				delayedAction.Tick();
				if (delayedAction.isApplied) {
					delayedActions.Remove(delayedAction);
				}
			}
		}
		
		public virtual void MeasureEvent(int currentMeasure) {
		}
		
		#region IEnumerators
		public virtual IEnumerator FadeIn(float targetVolume, float time, AnimationCurve curve) {
			State = AudioStates.FadingIn;
			audioSource.Play();
			gainManager.Activate();
			itemManager.Activate(this);
			
			IEnumerator fade = FadeVolume(audioSource.volume, targetVolume, time, curve);
			while (fade.MoveNext()) {
				yield return fade.Current;
			}
			
			base.Play();
		}
		
		public virtual IEnumerator FadeOut(float targetVolume, float time, AnimationCurve curve) {
			State = AudioStates.FadingOut;
			coroutineHolder.RemoveCoroutines("FadeIn");
			
			IEnumerator fade = FadeVolume(audioSource.volume, targetVolume, time, curve);
			while (fade.MoveNext()) {
				yield return fade.Current;
			}
			
			base.Stop();
			audioSource.Stop();
			gainManager.Deactivate();
			itemManager.Deactivate(this);
			coroutineHolder.RemoveAllCoroutines();
		}

		public virtual IEnumerator FadeVolume(float startVolume, float targetVolume, float time, AnimationCurve curve) {
			float counter = 0;
			
			while (counter < time) {
				float fadeVolume = curve.Evaluate(counter / time);
				audioSource.volume = fadeVolume * startVolume;
				counter += Time.deltaTime;
				yield return new WaitForSeconds(0);
			}
			audioSource.volume = targetVolume;
		}
		#endregion
	}
}
