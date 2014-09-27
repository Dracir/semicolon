using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TextCollider2D))]
[System.Serializable]
public class Parameter : MonoBehaviour, IDeletable {

	public List<Observer>		observers		= new List<Observer>();
	
	protected void notifyObservers(){
		foreach (var observer in observers) {
			observer.notify();	
		}
	}

	public virtual void refresh(){
		TextCollider2D tc = this.GetComponent<TextCollider2D> ();
		tc.Text = "!NUULLLL!!!";
	}

	void IDeletable.Delete(){
		//int clickedX = (int) Camera.main.camera.ScreenToWorldPoint(Input.mousePosition).x;
		//int x = clickedX - (int) hitedParameter.transform.parent.transform.position.x;
		Instruction instruction = this.transform.parent.GetComponent<Instruction>();
		EffectManager.AddGameEffect(new WaveDeleteTextEffect(instruction,1,0));
	}

	public virtual void swapWith(Parameter otherParameter){}
	public virtual DataType getType(){ return DataType.BOOLEAN ;}
	
	public bool isOfType(DataType dataType){ 
		return getType().Equals(dataType);
	}
	public bool isSameType(Parameter parameter){ 
		return this.isOfType( parameter.getType() );
	}
	
}