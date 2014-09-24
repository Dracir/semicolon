using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TextCollider2D))]
[System.Serializable]
public class Parameter : MonoBehaviour {

	public List<Observer>		observers		= new List<Observer>();
	
	protected void notifyObservers(){
		foreach (var observer in observers) {
			observer.notify();	
		}
	}

	public virtual void refresh(){
		TextCollider2D tc = this.GetComponent<TextCollider2D> ();
		tc.text = "!NUULLLL!!!";
	}


	public virtual void swapWith(Parameter otherParameter){}
	public virtual bool isOfType(DataType dataType){ return false;}
	
}