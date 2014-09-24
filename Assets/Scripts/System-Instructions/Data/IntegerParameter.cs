using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntegerParameter : Parameter {

	public int value;
	public int Value{
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

		tc.color = GameConstantes.instance.integerValueColor;
		tc.text = "" + this.value;
		this.name = "Int";
		
		this.transform.parent.GetComponent<Instruction>().refresh();
	}

	public override void swapWith(Parameter otherParameter){
		var other = otherParameter as IntegerParameter;
		if (other != null) {

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
		return dataType.Equals(DataType.INTEGER);
	}
}
