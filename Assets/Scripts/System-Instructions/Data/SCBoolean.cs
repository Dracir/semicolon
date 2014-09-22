using UnityEngine;
using System.Collections;

[System.Serializable]
public class SCBoolean : SCObject {

	public bool value;

	public SCBoolean(bool value){
		this.value = value;
	}

	public override string ToString (){
		return string.Format (value? "TRUE" : "FALSE");
	}
}
