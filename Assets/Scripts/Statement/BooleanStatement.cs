using UnityEngine;
using System.Collections;

public class BooleanStatement : Statement {

	public bool value;


	protected override void resetText(){
		string text = this.statementText.Replace ("%v", value ? "true" : "false"); 
		setText (text);
	}



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
