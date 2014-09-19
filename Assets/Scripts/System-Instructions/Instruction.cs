using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TextMesh))]
[RequireComponent(typeof(TextCollider2D))]
public class Instruction : MonoBehaviour {

	[SerializeField]
	public string 				instructionText;
	public string 				textToShow;

	[SerializeField]
	public List<ParameterData>  parametersData 	= new List<ParameterData>();
	[SerializeField]
	public List<Parameter>		parameter 		= new List<Parameter>();

	public void setText(string text){
		instructionText = text;
		reset ();
	}
	
	public void reset(){
		this.name = instructionText;
		deleteAllChild ();
		//tmpFix ();
		createParameterGameObjects ();
		setTextToShow ();
		
	}

	void setTextToShow(){
		TextCollider2D tc = this.GetComponent<TextCollider2D> ();
		tc.text = textToShow;
	}

	string createSpaces(int nb){
		string space = "";
		for (int i = 0; i < nb; i++) {
			space+= " ";
		}
		return space;
	}
	
	void deleteAllChild(){
		var children = new List<GameObject>();
		foreach (Transform child in transform) children.Add(child.gameObject);
		children.ForEach(child => GameObjectUtils.Destroy(child));
	}

	/*void tmpFix(){
		GameObject obj = GameObjectFactory.createGameObject ("TMP FIX", this.transform);
		
		TextMesh textMesh = obj.AddComponent<TextMesh> ();
		
		TextCollider2D textCollider = gameObject.GetComponent<TextCollider2D>();
		textCollider.text = instructionText;
		textCollider.fontSize = 166;
		textCollider.textMesh = textMesh;
	}*/

	void createParameterGameObjects(){
		string remainingText = instructionText;
		int indexOfValueTag = remainingText.IndexOf("%v");
		int x = 0;
		int parameterIndex = 0;
		textToShow = "";

		while(indexOfValueTag != -1){
			textToShow += remainingText.Substring(0,indexOfValueTag);
			remainingText 		= remainingText.Substring(indexOfValueTag+2);
			x += indexOfValueTag;
			indexOfValueTag 	= remainingText.IndexOf("%v");

			GameObject go = createParameterGameObject(parameterIndex, x);
			TextCollider2D tc = go.GetComponent<TextCollider2D>();
			x+= tc.text.Length;
			string spaces = createSpaces(tc.text.Length);
			textToShow += spaces;

			parameterIndex++;

		}

		textToShow += remainingText;
	}

	GameObject createParameterGameObject (int parameterIndex, int x){
		if (parameterIndex >= parametersData.Count) {
			Debug.LogError("Pas assez de parameters NO00b");
			return null;
		}
		GameObject go = null;
		switch(parametersData[parameterIndex].datatype){
			case DataType.BOOLEAN:
			go = createBooleanParameter(parametersData[parameterIndex]);
			break;
			case DataType.INTEGER:
			go = createIntegerParameter(parametersData[parameterIndex]);
			break;
		}
		if(go != null){
			go.transform.Translate(x,0,0);                                       
		}
		return go;
	}

	GameObject createBooleanParameter(ParameterData data){
		GameObject go = GameObjectFactory.createGameObject ("Boolean", this.transform);
		
		TextMesh textMesh = go.AddComponent<TextMesh> ();
		
		TextCollider2D textCollider = go.AddComponent<TextCollider2D>();
		textCollider.text = "TRUE";
		textCollider.fontSize = 166;
		textCollider.textMesh = textMesh;

		return go;
	}
	
	GameObject createIntegerParameter(ParameterData data){
		GameObject go = GameObjectFactory.createGameObject ("Integer", this.transform);
	
		TextMesh textMesh = go.AddComponent<TextMesh> ();
		
		TextCollider2D textCollider = go.AddComponent<TextCollider2D>();
		textCollider.text = "1";
		textCollider.fontSize = 166;
		textCollider.textMesh = textMesh;
		
		return go;
	}


	// Use this for initialization
	void Start () {
	
	}


	
	// Update is called once per frame
	void Update () {
	
	}
}
