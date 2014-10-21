using UnityEngine;
using System.Collections;

public class ParameterTether : MonoBehaviour {

	public Color color = Color.white;
	public float tetherLength = 3f;
	public float slerpAmount = 0.2f;
	
	#pragma warning disable 0108
	private LineRenderer renderer;
	
	private Parameter collidedParameter;
	
	private bool inDragMod = false;
	private Parameter parameterInDrag;
	private Vector3 parameterOldPosition;
	
	void Start () {
		getRenderer();
	}

	void OnTriggerEnter2D(Collider2D other){
		Parameter param = other.GetComponent<Parameter>();
		if(param != null){
			Effect e = GameConstantes.instance.currentTheme.createParameterHighlightEffect(param);
			EffectManager.AddGameEffect(e);
			this.collidedParameter = param;
		}
	}
	
	void OnTriggerExit2D(Collider2D other){
		Parameter param = other.GetComponent<Parameter>();
		if(param != null){
			if(param.Equals(this.collidedParameter)){
				this.collidedParameter = null;
			}
		}
	}
	
	void OnDrawGizmos(){
		getRenderer();
		moveTetherLine();
	}
	
	void Update () {
		moveTether();
		moveTetherLine();
		
		if (inDragMod) {
			parameterInDrag.transform.SetPosition ( this.transform.position );
			if (!Input.GetMouseButton (0)) {
				stopDrag ();
			}
		} else {
			if (Input.GetMouseButton (0)) {
				startDrag();
			}
		}
	}
	
	
	void moveTether(){
		Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 target = Vector3.Slerp(this.transform.position, mouse, slerpAmount);
		Transform body = Semicolon.instance.transform;
		
		target = new Vector3(target.x, target.y, body.position.z);
		Vector3 diff = target - body.position;
		this.transform.position = diff.normalized * tetherLength + body.position;
	}
	

	void moveTetherLine(){
		Vector3 tetherLineEndPoint = this.transform.position;
		if(this.collidedParameter != null && !this.inDragMod){
			Vector3 vt = this.collidedParameter.transform.position;
			tetherLineEndPoint = new Vector3(vt.x + 0.5f, vt.y - 0.5f, vt.z);
		}
		
		renderer.SetPosition(0, this.transform.parent.position);
		renderer.SetPosition(1, tetherLineEndPoint);
	}
	
	
	void startDrag(){
		if(collidedParameter != null){
			this.inDragMod = true;
			this.parameterInDrag = collidedParameter;
			this.parameterOldPosition = collidedParameter.transform.position;
			
			parameterInDrag.gameObject.layer = LayerMask.NameToLayer("No Collisions");
		}
	}

	void stopDrag(){
		inDragMod = false;
		parameterInDrag.gameObject.layer = LayerMask.NameToLayer("Parameter");
		
		if(collidedParameter != null){
			swap(collidedParameter,parameterInDrag);
		}else{
			parameterInDrag.transform.SetPosition(this.parameterOldPosition);
		}
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
		
		EffectManager.AddGameEffect( new ColorChangeEffect(textColliderHited	,c1t0,c1t1, GameConstantes.instance.currentTheme.timeOnInstructionSwap) );
		EffectManager.AddGameEffect( new ColorChangeEffect(textColliderInDrag	,c2t0,c2t1, GameConstantes.instance.currentTheme.timeOnInstructionSwap) );
		
		hitedParameter.swapWith(parameterDragged);
	}
	
	
	
	void getRenderer(){
		if(renderer == null){
			this.renderer = GetComponent<LineRenderer>();
			if(renderer == null){
				this.renderer = this.AddComponent<LineRenderer>();
				this.renderer.material = (Material) Resources.Load("Material/TetherMaterial");	
			}
		}
	}
}

