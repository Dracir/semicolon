using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Candlelight;


public class BooleanParameter : Parameter {

	[SerializeField, PropertyBackingField(typeof(BooleanParameter), "Valeur")]
	private bool valeur;
	public bool Valeur{
		get{return valeur;}
		set{
			this.valeur = value;
			refresh();
		}
	}

	public override void refresh(){
		TextCollider2D tc = this.GetComponent<TextCollider2D> ();

		tc.Color = GameConstantes.instance.currentTheme.booleanValueColor;
		tc.Text = this.Valeur ? "TRUE" : "FALSE";
		this.name = "Bool";
		
		this.transform.parent.GetComponent<Instruction>().refresh();
		this.notifyObservers();
	}

	public override void swapWith(Parameter otherParameter){
		if (otherParameter is BooleanParameter) {
			BooleanParameter other = (BooleanParameter) otherParameter;

			var tmp = this.valeur;
			this.valeur = other.valeur;
			other.valeur = tmp;
			
			this.refresh();
			other.refresh();
			
			this.transform.parent.GetComponent<Instruction> ().reset ();
			other.transform.parent.GetComponent<Instruction> ().reset ();
		}
	}	
	
	public override DataType getType(){ 
		return DataType.BOOLEAN ;
	}
}
