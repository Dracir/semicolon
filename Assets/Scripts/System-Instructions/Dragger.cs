﻿using UnityEngine;
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
				swap(hitedParameter,parameterInDrag);
			}

		}else{
			parameterInDrag.transform.SetPosition(this.parameterOldPosition);
		}
		this.parameterInDrag.gameObject.layer =  LayerMask.NameToLayer("Parameter");
	}

	void swap(Parameter hitedParameter, Parameter parameterDragged){
		if(!hitedParameter.isSameType(parameterDragged)){
			return;
		}
		TextCollider2D textColliderHited = hitedParameter.GetComponent<TextCollider2D>();
		TextCollider2D textColliderInDrag = parameterDragged.GetComponent<TextCollider2D>();
		Color c1t1 = textColliderHited.Color;
		Color c1t0 = new Color(c1t1.r, c1t1.g, c1t1.b, 0);
		Color c2t1 = textColliderInDrag.Color;
		Color c2t0 = new Color(c1t1.r, c1t1.g, c1t1.b, 0);
		
		EffectManager.AddGameEffect( new ColorChangeEffect(textColliderHited	,c1t0,c1t1, GameConstantes.instance.currentTheme.effetTimeOnInstructionSwap) );
		EffectManager.AddGameEffect( new ColorChangeEffect(textColliderInDrag	,c2t0,c2t1, GameConstantes.instance.currentTheme.effetTimeOnInstructionSwap) );
		
		
		//EffectManager.AddGameEffect( new MoveEffect(textColliderHited	,textColliderInDrag.transform.position, 0.5f, false) );
		//EffectManager.AddGameEffect( new MoveEffect(textColliderInDrag	,textColliderHited.transform.position , 0.5f, false) );
		
		
		TextCollider2D parentColiderHited = hitedParameter.transform.parent.GetComponent<TextCollider2D>();
		TextCollider2D parentColiderDraged = parameterDragged.transform.parent.GetComponent<TextCollider2D>();
		
		EffectManager.AddGameEffect( new GradientEffet(parentColiderHited ,GameConstantes.instance.currentTheme.instructionFlash, GameConstantes.instance.currentTheme.effetTimeOnInstructionSwap) );
		EffectManager.AddGameEffect( new GradientEffet(parentColiderDraged,GameConstantes.instance.currentTheme.instructionFlash, GameConstantes.instance.currentTheme.effetTimeOnInstructionSwap) );
		
		hitedParameter.swapWith(parameterDragged);
	}
}
