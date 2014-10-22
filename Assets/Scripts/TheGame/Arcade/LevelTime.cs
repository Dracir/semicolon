using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Candlelight;


public class LevelTime : MonoBehaviour {

	public static LevelTime instance;
	public List<Observer> observers = new List<Observer>();
	
	[SerializeField, PropertyBackingFieldAttribute(typeof(LevelTime), "TimeLeft")]
	private float timeLeft;
	public float TimeLeft{
		get{ return timeLeft; }
		set{
			this.timeLeft = value;
			notifyObservers();
		}
	}

	void notifyObservers(){
		foreach (var element in observers) {
			element.notify();
		}
	}
	
	void Awake () {
		LevelTime.instance = this;
	}
	
	void Update(){
		TimeLeft -= Time.deltaTime;
		
		if(TimeLeft <= 0 ){
			GameObject.FindObjectOfType<ArcadeGameManager>().LoseGame();
		}
	}
}
