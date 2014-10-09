using UnityEngine;
using System.Collections;

public class LevelScore : MonoBehaviour {

	public static LevelScore instance;
	
	public int score;
	
	void Start () {
		LevelScore.instance = this;
	}
	
	
	void Update () {
	
	}
}
