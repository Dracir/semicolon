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
	

	public bool autoCompile = false;
	
	
	public override void refresh(){
		TextCollider2D tc = this.GetComponent<TextCollider2D> ();

		tc.Color = GameConstantes.instance.currentTheme.integerValueColor;
		tc.Text = "" + this.valeur;
		this.name = "Int";
		
		this.transform.parent.GetComponent<Instruction>().refresh();
		if(autoCompile){
			this.notifyObservers();
		}
		
	}

	public override void swapWith(Parameter otherParameter){
		var other = otherParameter as IntegerParameter;
		if (other != null) {

			var tmp = this.Valeur;
			this.Valeur = other.Valeur;
			other.Valeur = tmp;
			
			this.transform.parent.GetComponent<Instruction> ().refresh ();
			other.transform.parent.GetComponent<Instruction> ().refresh();
		}
	}
	
	public override DataType getType(){ 
		return DataType.INTEGER ;
	}
	
	public override Color getColor(){ 
		return GameConstantes.instance.currentTheme.integerValueColor;
	}
	
	public override Gradient getHighLightedGradient(){ 
		return GameConstantes.instance.currentTheme.integerValueHighlightedGradient;
	}
}
