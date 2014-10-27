using UnityEngine;
using System.Collections;

public class ArcadeGameManager : MonoBehaviour {
	
	public GameObject LoseGameCanvas;
	public string menuLevelName = "MenuGym";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)){
			LoseGame();
		}
	}
	
	public void LoseGame () {
		Time.timeScale = 0;
		if(LoseGameCanvas){
			LoseGameCanvas.SetActive(true);
		}
		
	}
	
	public void Restart() {
		Application.LoadLevel(Application.loadedLevel);
		Time.timeScale = 1;
	}
	
	public void MainMenu () {
		Application.LoadLevel(menuLevelName);
		Time.timeScale = 1;
	}
}
