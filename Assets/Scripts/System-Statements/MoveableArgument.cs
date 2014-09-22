using UnityEngine;
using System.Collections;

public class MoveableArgument : MonoBehaviour {

	private bool inDragMod = false;

	private Transform oldParent;
	private Vector3 oldPosition;

	private Statement statement;
	void Start () {
		this.statement = this.gameObject.transform.parent.GetComponent<Statement> ();
	}
	
	// Update is called once per frame
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
						MoveableArgument ma = other.GetComponent<MoveableArgument> ();
						if (ma) {
								Statement otherStatement = other.transform.parent.GetComponent<Statement> ();
								if (this.statement.isSameType (otherStatement)) {
										stopDragMod ();
										this.statement.swapParam (otherStatement);
								} else {
										stopDragMod ();
								}
						} else {
								stopDragMod ();
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
