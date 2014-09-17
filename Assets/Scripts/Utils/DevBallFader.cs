using UnityEngine;
using System.Collections;

public class DevBallFader : GizmoDad {
	
	private float destroyTimer = 7f;
	
	
	// Use this for initialization
	void Start () {
		
		Destroy(gameObject, destroyTimer);
	}
	
	// Update is called once per frame
	void Update () {
		
		
	}
	
	void ChangeColour(Couleur newColour){
		myColour = newColour;
	}
}
