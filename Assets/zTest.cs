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

	void Test() {
		
	}
	
	void Start() {
		References.Metronome.Subscribe(this);
	}
	
	public void BeatEvent(int currentBeat) {
		beatSource.Play();
	}

	public void MeasureEvent(int currentMeasure) {
		measureSource.Play();
	}
}
