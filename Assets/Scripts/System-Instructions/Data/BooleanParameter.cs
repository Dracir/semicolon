using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BooleanParameter : Parameter {

	public bool value;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	public override void refresh(){
		TextCollider2D tc = this.GetComponent<TextCollider2D> ();

		tc.color = GameConstantes.instance.booleanValueColor;
		this.name = "Bool";
				
		if (this.value) {
			tc.text = "TRUE";		
		} else {
			tc.text = "FALSE";
		}
		
		

	}

	public override void swapWith(Parameter otherParameter){
		if (otherParameter is BooleanParameter) {
			BooleanParameter other = (BooleanParameter) otherParameter;

			var tmp = this.value;
			this.value = other.value;
			other.value = tmp;
			
			this.refresh();
			other.refresh();
			
			this.transform.parent.GetComponent<Instruction> ().reset ();
			other.transform.parent.GetComponent<Instruction> ().reset ();
		}

	}
}
