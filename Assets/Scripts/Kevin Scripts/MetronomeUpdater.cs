using UnityEngine;
using Candlelight;
using System.Collections;
using Magicolo.AudioTools;
using Magicolo.GeneralTools;

public class MetronomeUpdater : MonoBehaviour {

	public Metronome metronome = new Metronome(120, 4);
	
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
