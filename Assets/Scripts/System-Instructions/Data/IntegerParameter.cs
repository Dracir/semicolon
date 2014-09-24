using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Candlelight;

public class IntegerParameter : Parameter {

	[SerializeField, PropertyBackingField(typeof(IntegerParameter), "Valeur")]
	private int valeur;
	public int Valeur{
		get{return valeur;}
		set{
			this.valeur = value;
			refresh();
		}
	}
	

	public override void refresh(){
		TextCollider2D tc = this.GetComponent<TextCollider2D> ();

		tc.color = GameConstantes.instance.integerValueColor;
		tc.text = "" + this.valeur;
		this.name = "Int";
		
		this.transform.parent.GetComponent<Instruction>().refresh();
		this.notifyObservers();
	}

	public override void swapWith(Parameter otherParameter){
		var other = otherParameter as IntegerParameter;
		if (other != null) {

			var tmp = this.valeur;
			this.valeur = other.valeur;
			other.valeur = tmp;
			
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
