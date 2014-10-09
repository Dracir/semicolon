using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Candlelight;

public class LevelScore : MonoBehaviour {

	public static LevelScore instance;
	public List<Observer> observers = new List<Observer>();
	
	[SerializeField, PropertyBackingField(typeof(LevelScore), "Score")]
	private int score;
	public int Score{
		get{ return score; }
		set{
			this.score = value;
			notifyObservers();
		}
	}

	void notifyObservers(){
		foreach (var element in observers) {
			element.notify();
		}
	}
	
	void Awake () {
		LevelScore.instance = this;
	}
	
	
	void Update () {
	
	}
}
