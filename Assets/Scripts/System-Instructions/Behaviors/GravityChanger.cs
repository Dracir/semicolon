using UnityEngine;
using System.Collections;

public class GravityChanger : Observer {

	public IntegerParameter gravityValue;

	private Player player;
	
	void Start () {
		this.player = GameObject.Find("Player").GetComponent<Player>();
	}
	
	
	void Update () {
	
	}
	
	public override void notify(){
		if(gravityValue != null){
			Semicolon.instance.SetJumpHeight(gravityValue.value * 10);
		}
		
	}
}
