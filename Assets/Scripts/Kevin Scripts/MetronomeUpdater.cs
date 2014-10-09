using UnityEngine;
using Candlelight;
using System.Collections;
using Magicolo.AudioTools;
using Magicolo.GeneralTools;

public class MetronomeUpdater : MonoBehaviour {

	[SerializeField, PropertyBackingField(typeof(MetronomeUpdater), "BeatsPerMinute", typeof(RangeAttribute), 0F, 1000F)]
	double beatsPerMinute = 120;
	public double BeatsPerMinute {
		get {
			beatsPerMinute = metronome.BeatsPerMinute;
			return beatsPerMinute;
		}
		set {
			beatsPerMinute = value;
			metronome.BeatsPerMinute = beatsPerMinute;
		}
	}

	[SerializeField, PropertyBackingField(typeof(MetronomeUpdater), "BeatsPerMeasure", typeof(MinAttribute))]
	int beatsPerMeasure = 4;
	public int BeatsPerMeasure {
		get {
			beatsPerMeasure = metronome.BeatsPerMeasure;
			return beatsPerMeasure;
		}
		set {
			beatsPerMeasure = value;
			metronome.BeatsPerMeasure = beatsPerMeasure;
		}
	}
	
	public Metronome metronome;
	
	void Awake() {
		metronome = new Metronome(beatsPerMinute, beatsPerMeasure);
	}
	
	void Start() {
		metronome.Start();
	}
	
	void Update() {
		metronome.Update();
	}
	
	void LateUpdate() {
		metronome.Update();
	}
	
	void FixedUpdate() {
		metronome.Update();
	}
}
