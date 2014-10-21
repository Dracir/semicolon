using System.Collections.Generic;
using UnityEngine;
using Candlelight;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	public class Metronome : MonoBehaviour {

		[SerializeField, PropertyBackingField(typeof(Metronome), "BeatsPerMinute", typeof(RangeAttribute), 0.1F, 1000F)]
		float beatsPerMinute = 120;
		public float BeatsPerMinute {
			get {
				return beatsPerMinute;
			}
			set {
				beatsPerMinute = value;
				beatDuration = 60F / beatsPerMinute;
				measureDuration = (60F / beatsPerMinute) * beatsPerMeasure;
			}
		}
		
		[SerializeField, PropertyBackingField(typeof(Metronome), "BeatsPerMeasure", typeof(MinAttribute), 1F)]
		int beatsPerMeasure = 4;
		public int BeatsPerMeasure {
			get {
				return beatsPerMeasure;
			}
			set {
				beatsPerMeasure = value;
				beatDuration = 60D / beatsPerMinute;
				measureDuration = (60D / beatsPerMinute) * beatsPerMeasure;
			}
		}

		public bool playOnAwake;

		int currentBeat;
		public int CurrentBeat { 
			get {
				return currentBeat;
			}
		}
		
		int currentMeasure;
		public int CurrentMeasure { 
			get {
				return currentMeasure;
			}
		}
	
		bool isPlaying;
		public bool IsPlaying {
			get {
				return isPlaying;
			}
		}
		
		double nextBeatTime;
		double nextMeasureTime;
		double beatDuration;
		double measureDuration;
		List<ITickable> tickables = new List<ITickable>();
		IEnumerator ticker;
	
		void Awake() {
			if (playOnAwake) {
				Play();
			}
		}
		
		void Update() {
			Tick();
		}
	
		void LateUpdate() {
			Tick();
		}
	
		void FixedUpdate() {
			Tick();
		}
	
		void OnGUI() {
			Tick();
		}
	
		void OnRenderObject() {
			Tick();
		}
		
		public void Play() {
			isPlaying = true;
			
			currentBeat = 0;
			currentMeasure = 0;
			nextBeatTime = AudioSettings.dspTime;
			nextMeasureTime = AudioSettings.dspTime;
			beatDuration = 60F / beatsPerMinute;
			measureDuration = (60F / beatsPerMinute) * beatsPerMeasure;
			ticker = Ticker();
		}
		
		public void Stop() {
			isPlaying = false;
		}
		
		public void Subscribe(ITickable tickable) {
			tickables.Add(tickable);
		}
		
		public void Unsubscribe(ITickable tickable) {
			tickables.Remove(tickable);
		}
		
		public void GetTempo(out float beatsPerMinute, out int beatsPerMeasure) {
			beatsPerMinute = BeatsPerMinute;
			beatsPerMeasure = BeatsPerMeasure;
		}
		
		public void SetTempo(float beatsPerMinute, int beatsPerMeasure) {
			BeatsPerMinute = beatsPerMeasure;
			BeatsPerMeasure = beatsPerMeasure;
		}

		public float GetAdjustedDelay(float delay, SyncMode syncMode) {
			float adjustedDelay = 0;
			
			switch (syncMode) {
				case SyncMode.None:
					adjustedDelay = delay;
					break;
				case SyncMode.Beat:
					adjustedDelay = (float)(nextBeatTime - AudioSettings.dspTime + delay * beatDuration);
					break;
				case SyncMode.Measure:
					adjustedDelay = (float)(nextMeasureTime - AudioSettings.dspTime + delay * measureDuration);
					break;
			}
			return adjustedDelay;
		}

		public int ConvertToBeats(float delay, SyncMode syncMode) {
			int beats = 0;
			
			switch (syncMode) {
				case SyncMode.None:
					beats = (int)delay.Round();
					break;
				case SyncMode.Beat:
					beats = (int)delay.Round();
					break;
				case SyncMode.Measure:
					beats = (int)delay.Round() * BeatsPerMeasure;
					break;
			}
			return beats;
		}
		
		void Tick() {
			if (isPlaying) {
				ticker.MoveNext();
			}
		}
		
		void BeatEvent() {
			foreach (ITickable tickable in tickables) {
				if (tickable != null) {
					tickable.BeatEvent(CurrentBeat);
				}
			}
		}

		void MeasureEvent() {
			foreach (ITickable tickable in tickables) {
				if (tickable != null) {
					tickable.MeasureEvent(CurrentMeasure);
				}
			}
		}
	
		IEnumerator Ticker() {
			while (true) {
				double currentTime = AudioSettings.dspTime;
				if (currentTime >= nextBeatTime) {
					if (CurrentBeat == 0) {
						currentMeasure += 1;
						nextMeasureTime += measureDuration;
						MeasureEvent();
					}
				
					currentBeat = (CurrentBeat + 1) % beatsPerMeasure;
					nextBeatTime += beatDuration;
					BeatEvent();
				}
				yield return null;
			}
		}
	}
}
