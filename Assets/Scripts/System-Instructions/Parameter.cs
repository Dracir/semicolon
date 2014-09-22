using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TextMesh))]
[RequireComponent(typeof(TextCollider2D))]
[System.Serializable]
public class Parameter : MonoBehaviour {

	public DataType 	dataType;
	public DataType 	DataType{
		get{ return dataType; }
		set{
			dataType = value;
			reset();
		}
	}

	[SCObjectAttribute()]
	[SerializeField]
	public SCObject value;
	private Instruction instruction;

	public List<Observer>		observers		= new List<Observer>();
	
	[Button(label:"Reset",methodName:"reset", NoPrefixLabel=true)]
	public bool resetBtn;
	[Button(label:"Refresh",methodName:"refresh", NoPrefixLabel=true)]
	public bool refreshBtn;

	protected void notifyObservers(){
		foreach (var observer in observers) {
			observer.update();	
		}
	}

	public void reset(){
		switch (dataType) {
		case DataType.BOOLEAN: 
			this.value = new SCBoolean(false);
			break;
		case DataType.INTEGER:
			this.value = new SCInteger(0);
			break;
		}

		refresh ();
	}

	public void refresh(){
		TextCollider2D tc = this.GetComponent<TextCollider2D> ();

		switch (dataType) {
		case DataType.BOOLEAN: 
			tc.color = GameConstantes.instance.booleanValueColor;
			this.name = "Bool";
			break;
		case DataType.INTEGER:
			tc.color = GameConstantes.instance.integerValueColor;
			this.name = "Int";
			break;
		}

		if (this.value != null) {
			tc.text = this.value.ToString ();		
		} else {
			tc.text = "!NUULLLL!!!";		
		}

	}



	void Start () {
		this.instruction = this.gameObject.transform.parent.GetComponent<Instruction> ();
	}
	

	void Update () {
	
	}


	public void swapWith(Parameter otherParameter){
		var tmp = this.value;
		this.value = otherParameter.value;
		otherParameter.value = tmp;
		this.refresh();
		otherParameter.refresh();
		instruction.reset ();
		otherParameter.transform.parent.GetComponent<Instruction> ().reset ();
	}
	
}