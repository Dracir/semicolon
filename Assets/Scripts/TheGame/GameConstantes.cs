using UnityEngine;
using System.Collections;

public class GameConstantes : MonoBehaviour {

	public GameCodeTheme currentTheme;
	
	public static GameConstantes instance;

	public GameConstantes(){
		GameConstantes.instance = this;	
	}

	void Start () {

	}
	
	void Update(){
		Camera.main.camera.backgroundColor = currentTheme.background;
	}
}
