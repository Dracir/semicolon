using UnityEngine;
using System.Collections;

public class LevelTime : MonoBehaviour {

	public static LevelTime instance;
	
	public int time;
	
	void Start () {
		LevelTime.instance = this;
	}
	
	
	void Update () {
	
	}
}
