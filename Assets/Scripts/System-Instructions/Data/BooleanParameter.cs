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
	
	public bool autoCompile = false;

	public override void refresh(){
		TextCollider2D tc = this.GetComponent<TextCollider2D> ();

		
		if(canBeChanged){
			tc.Color = GameConstantes.instance.currentTheme.booleanValueColor;
		}else{
			tc.Color = GameConstantes.instance.currentTheme.instructionColor;
		}
		
		tc.Text = this.Valeur ? "TRUE" : "FALSE";
		this.name = "Bool";
		
		if(autoCompile){
			this.GetComponentInParent<Instruction>().compile();
		}
	}

	public override void swapWith(Parameter otherParameter){
		if (otherParameter is BooleanParameter) {
			BooleanParameter other = (BooleanParameter) otherParameter;

			var tmp = this.Valeur;
			this.Valeur = other.Valeur;
			other.Valeur = tmp;
			
			this.transform.parent.GetComponent<Instruction> ().refresh ();
			other.transform.parent.GetComponent<Instruction> ().refresh();
		}
	}	
	
	public override DataType getType(){ 
		return DataType.BOOLEAN ;
	}
	
		
	public override Color getColor(){ 
		return GameConstantes.instance.currentTheme.booleanValueColor;
	}
	
	public override Gradient getHighLightedGradient(){ 
		return GameConstantes.instance.currentTheme.booleanValueHighlightedGradient;
	}
}
