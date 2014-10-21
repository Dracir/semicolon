using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TextCollider2D))]
[System.Serializable]
public class Parameter : MonoBehaviour {

	public List<Observer>		observers		= new List<Observer>();
	public bool canBeChanged = true;
	
	public void notifyObservers(){
		if(!Application.isPlaying) return;
		
		foreach (var observer in observers) {
			observer.notify();
		}
	}

	public virtual void refresh(){
		TextCollider2D tc = this.GetComponent<TextCollider2D> ();
		tc.Text = "!NUULLLL!!!";
	}

	public virtual void swapWith(Parameter otherParameter){}
	public virtual DataType getType(){ return DataType.BOOLEAN ;}
	public virtual Color getColor(){ return Color.white ;}
	public virtual Gradient getHighLightedGradient(){ return null;}
	
	public bool isOfType(DataType dataType){ 
		return getType().Equals(dataType);
	}
	public bool isSameType(Parameter parameter){ 
		return this.isOfType( parameter.getType() );
	}
	
}