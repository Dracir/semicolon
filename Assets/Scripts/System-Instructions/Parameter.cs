using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TextMesh))]
[RequireComponent(typeof(TextCollider2D))]
public class Parameter : MonoBehaviour {

	public DataType 	dataType;
	public DataType 	DataType{
		get{ return dataType; }
		set{
			dataType = value;
			reset();
		}
	}
	public SCObject value;

	private bool 		inDragMod = false;
	private Transform 	oldParent;
	private Vector3 	oldPosition;

	public List<Observer>		observers		= new List<Observer>();


	[Button(label:"Reset",methodName:"reset", NoPrefixLabel=true)]
	public bool resetBtn;

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

		tc.text = this.value.ToString ();
	}



	void Start () {
		//this.statement = this.gameObject.transform.parent.GetComponent<Statement> ();
	}
	

	void Update () {
		if (inDragMod) {
			Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			this.transform.SetPosition (new Vector3(p.x,p.y,0));		
		}
	}

	void OnMouseDown () {
		startDragMod ();
	}
	
	void OnMouseUp () {
		
		this.gameObject.layer =  LayerMask.NameToLayer("Ignore Raycast");
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 100)) {
			GameObject other = hit.collider.gameObject;
			Parameter otherParameter = other.GetComponent<Parameter> ();

			stopDragMod ();
			if (otherParameter && this.dataType.Equals(otherParameter.dataType)) {
				//this.statement.swapParam (otherStatement);
			}
		} else {
			stopDragMod ();		
		}
	}

	
	public void startDragMod(){
		this.oldPosition = this.transform.position;
		this.oldParent = this.transform.parent;
		this.transform.parent = null;
		inDragMod = true;
	}
	
	public void stopDragMod(){
		inDragMod = false;
		this.transform.SetPosition (oldPosition);
		this.transform.parent = oldParent;
		this.gameObject.layer =  LayerMask.NameToLayer("Default");
		
	}

}
