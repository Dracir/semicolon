using UnityEngine;
using System.Collections;

public class ArgumentSensor : MonoBehaviour {
	
	Parameter myParam;
	
	// Use this for initialization
	void Start () {
		myParam = GetComponent<Parameter>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter2D (Collider2D other){
		Parameter param = other.GetComponent<Parameter>();
		if (param){
			//swap
			var temp = myParam.value;
			myParam.value = param.value;
			param.value = temp;
		}
	}
}
