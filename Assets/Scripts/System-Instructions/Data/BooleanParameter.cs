using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BooleanParameter : Parameter {

	public bool value;
	public bool Value{
		get{return value;}
		set{
			this.value = value;
			refresh();
		}
	}
	
	[Button(label:"refresh",methodName:"refresh", NoPrefixLabel=true)]
	public bool resetBtn;


	public override void refresh(){
		TextCollider2D tc = this.GetComponent<TextCollider2D> ();

		tc.color = GameConstantes.instance.booleanValueColor;
		tc.text = this.value ? "TRUE" : "FALSE";
		this.name = "Bool";
		
		this.transform.parent.GetComponent<Instruction>().refresh();
		this.notifyObservers();
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
	
	public override bool isOfType(DataType dataType){
		return dataType.Equals(DataType.BOOLEAN);
	}
}
