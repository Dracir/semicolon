using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Statement : MonoBehaviour {

	public string statementText;
	private bool isReset;

	public Observer[] observers = new Observer[]{};


	[Button(label:"Reset",methodName:"resetText", NoPrefixLabel=true)]
	public bool resetBtn;
	protected virtual void resetText(){
		isReset = true;
		setText (statementText);
		notifyObservers ();
	}

	protected void notifyObservers(){
		foreach (var observer in observers) {
			observer.update();	
		}
	}


	public virtual void setText(string text){
		statementText = text;
		deleteAllChild ();
		createTextChild (text);
		createCollider ();
	}
	
	void deleteAllChild(){
		var children = new List<GameObject>();
		foreach (Transform child in transform) children.Add(child.gameObject);
		children.ForEach(child => GameObjectUtils.Destroy(child));
	}

	protected virtual void createTextChild(string text){
		GameObject beforeArgumentChild = createText (Vector2.zero, text);

	}

	protected virtual void createCollider(){
		GameObject.DestroyImmediate (this.GetComponent<BoxCollider2D>());
		BoxCollider2D box = this.gameObject.AddComponent<BoxCollider2D>();		
		box.size 		= new Vector2(statementText.Length  ,1) ;
		box.center 		= new Vector2(statementText.Length/2, 1);
		box.isTrigger 	= false;
		//obj.layer = LayerMask.NameToLayer("NormalCollisions");
	}


	protected GameObject createText(Vector2 translate, string text){
		return createText (translate, text, GameConstantes.instance.statementColor);
	}

	protected GameObject createMoveableText(Vector2 translate, string text, Color color){
		GameObject obj = createText (translate, text, color);
		MoveableArgument ma = obj.AddComponent<MoveableArgument> ();
		BoxCollider bc = obj.AddComponent<BoxCollider> ();
		return obj;
	}

	protected GameObject createText(Vector2 translate, string text, Color color){
		GameObject obj = GameObjectFactory.createGameObject (text, this.transform);

		TextMesh textMesh = obj.AddComponent<TextMesh> ();
		textMesh.text 		= text;
		textMesh.anchor 	= TextAnchor.MiddleLeft;
		textMesh.font	 	= GameConstantes.instance.statementFont;
		textMesh.color = color;

		MeshRenderer mr = obj.GetComponent<MeshRenderer> ();
		mr.material 		= GameConstantes.instance.statementMaterial;
		
		obj.transform.Translate (translate);
		if (isReset) {
			obj.transform.Translate(this.transform.position);		
		}
		return obj;
	}

	public virtual bool isSameType(Statement statement){ return false;}
	
	public virtual void swapParam(Statement statement){  }

	
	


}
