using UnityEngine;
using System.Collections;

public class DebugLog : Observer {

	
	public string textToLog;
	
	public override void notify(){
		Debug.Log(textToLog);
	}
}
