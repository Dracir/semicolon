using UnityEngine;
using System.Collections;
using Magicolo.AudioTools;
using Magicolo.GeneralTools;

public class zTest : MonoBehaviour, ITickable {

	[Button("Test", "Test", NoPrefixLabel = true)] public bool test;
	
	public double beatsPerMinute = 120;
	public int beatsPerMeasure = 4;
	
	public AudioSource beatSource;
	public AudioSource measureSource;

	MetronomeUpdater metronomeUpdater;
	
	void Test() {
		
	}
	
	void Start() {
		metronomeUpdater = gameObject.AddComponent<MetronomeUpdater>();
		metronomeUpdater.metronome.Subscribe(this);
		metronomeUpdater.metronome.Start();
	}
	
	void Update() {
		metronomeUpdater.metronome.BeatsPerMinute = beatsPerMinute;
		metronomeUpdater.metronome.BeatsPerMeasure = beatsPerMeasure;
	}
	
	public void BeatEvent(int currentBeat) {
		beatSource.Play();
	}

	public void MeasureEvent(int currentMeasure) {
		measureSource.Play();
	}
}
