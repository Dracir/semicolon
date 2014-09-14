using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Statement : MonoBehaviour {

	public string statementText;
	//public DataTypeEnum datatypeEnum;

	public DataType datatype;
	
	private GameObject beforeArgumentChild;
	//private GameObject argumentChild;
	//private GameObject afterArgumentChild;



	[Button(label:"Reset",methodName:"resetText", NoPrefixLabel=true)]
	public bool resetBtn;
	protected virtual void resetText(){
		setText (statementText);
	}




	void Start () {
	}


	void Update () {
	
	}



	public void setText(string text){
		deleteAllChild ();
		GameObject.DestroyImmediate (this.GetComponent<BoxCollider2D>());

		if (text.Contains ("%v")) {
			int indexOfValueTag = text.IndexOf("%v");
			
			string textBefore = text.Substring(0,indexOfValueTag);
			string textAfter = text.Substring(indexOfValueTag+2, text.Length - indexOfValueTag -2);
			
			this.beforeArgumentChild = createText (Vector2.zero,textBefore);
			
			Vector2 translate = new Vector2(indexOfValueTag, 0);
			this.beforeArgumentChild = createText (translate, "");
			
			Vector2 translate2 = new Vector2(indexOfValueTag, 0);
			this.beforeArgumentChild = createText (translate2, textAfter);
		} else {
			this.beforeArgumentChild = createText (Vector2.zero, text);		
		}
		
		BoxCollider2D box = this.gameObject.AddComponent<BoxCollider2D>();		
		box.size 		= new Vector2(text.Length,1) ;
		box.center 		= new Vector2(text.Length/2, 1);
		box.isTrigger 	= false;
		//obj.layer = LayerMask.NameToLayer("NormalCollisions");
	}



	GameObject createText(Vector2 translate, string text){
		GameObject obj = GameObjectFactory.createGameObject ("Text", this.transform);
		obj.transform.Translate (translate);

		TextMesh textMesh = obj.AddComponent<TextMesh> ();
		textMesh.text 		= text;
		textMesh.anchor 	= TextAnchor.MiddleLeft;
		textMesh.font	 	= GameConstantes.instance.statementFont;

		MeshRenderer mr = obj.GetComponent<MeshRenderer> ();
		mr.material 		= GameConstantes.instance.statementMaterial;

		return obj;
	}

	void deleteAllChild(){
		var children = new List<GameObject>();
		foreach (Transform child in transform) children.Add(child.gameObject);
		children.ForEach(child => DestroyImmediate(child));
	}



}


