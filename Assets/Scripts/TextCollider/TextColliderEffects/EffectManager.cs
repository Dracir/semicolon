using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour {

	private static List<Effect> gameEffects;
	private static List<Effect> gameEffectsToAdd;
	private static List<Effect> GameEffectsToRemove;
	
	
	void Start () {
		EffectManager.gameEffects = new List<Effect> ();
		EffectManager.gameEffectsToAdd = new List<Effect> ();
		EffectManager.GameEffectsToRemove = new List<Effect> ();
	}

	public static void AddGameEffect(Effect gameEffect){
		if (gameEffects == null) {
			Debug.LogError("EffectMenager in no GameObject! (The maploader should have loaded that.");
			return;
		}
		gameEffectsToAdd.Add (gameEffect);
	}

	void Update  () {
		addNewEffect ();
		activateAllEffect ();
		removeOldEffects ();
		addNewEffect ();
	}

	private void addNewEffect(){
		foreach(Effect s in gameEffectsToAdd){
			s.onStart();
			gameEffects.Add(s);
		}
		gameEffectsToAdd.Clear();
	}

	private void activateAllEffect(){
		foreach(Effect s in gameEffects){
			s.update(Time.deltaTime);
			if(s.isDone){
				s.onStop();
				GameEffectsToRemove.Add(s);
			}
		}
	}

	private void removeOldEffects(){
		foreach (Effect toRemove in GameEffectsToRemove) {
			gameEffects.Remove(toRemove);
		}
		GameEffectsToRemove.Clear();
	}
}
