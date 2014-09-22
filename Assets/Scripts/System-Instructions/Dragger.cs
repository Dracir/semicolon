using UnityEngine;
using System.Collections;

public class Dragger : MonoBehaviour {

	private bool inDragMod = false;
	private Parameter parameterInDrag;
	private Vector3 parameterOldPosition;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (inDragMod) {
			Vector3 p = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			parameterInDrag.transform.SetPosition (new Vector3 (p.x, p.y, 0));
			if (!Input.GetMouseButton (0)) {
				stopDrag ();
			}
		} else {
			if (Input.GetMouseButton (0)) {
				startDrag();
			}
		}




	}

	void startDrag(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
		if(hit.collider != null){
			GameObject hitedObject = hit.collider.gameObject;
			Parameter hitedParameter = hitedObject.GetComponent<Parameter> ();
			
			if(hitedParameter){
				this.inDragMod = true;
				this.parameterOldPosition = hitedObject.transform.position;
				this.parameterInDrag = hitedParameter;
			}
		}
	}

	void stopDrag(){
		inDragMod = false;
		this.parameterInDrag.gameObject.layer =  LayerMask.NameToLayer("Ignore Raycast");
		parameterInDrag.transform.SetPosition(this.parameterOldPosition);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
		if(hit.collider != null){
			GameObject hitedObject = hit.collider.gameObject;
			Parameter hitedParameter = hitedObject.GetComponent<Parameter> ();
			
			if(hitedParameter){
				hitedParameter.swapWith(parameterInDrag);
			}

		}else{
			parameterInDrag.transform.SetPosition(this.parameterOldPosition);
		}
		this.parameterInDrag.gameObject.layer =  LayerMask.NameToLayer("Parameter");
	}
}
