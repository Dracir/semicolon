using UnityEngine;
using System.Collections;

[System.Serializable]
public class SCInteger : SCObject {
	
	public int value;
	
	public SCInteger(int value){
		this.value = value;
	}
	
	public override string ToString (){
		return "" + value;
	}
}
