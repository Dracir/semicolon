using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TextMesh))]
[RequireComponent(typeof(TextCollider2D))]
[System.Serializable]
public class Instruction : MonoBehaviour {

	public List<Observer>		observers		= new List<Observer>();
	public List<DataType>		parameterType 	= new List<DataType> ();

	public string 				instructionText;
	private string 				textToShow;

	[Button(label:"Reset",methodName:"reset", NoPrefixLabel=true)]
	public bool resetBtn;

	public void setText(string text){
		instructionText = text;
		reset ();
	}
	
	public void reset(){
		this.name = instructionText;
		fixChildAmount();
		resetTexts ();
		notifyObservers ();
	}


	void fixChildAmount(){

		int childCount = this.GetChildCount();
		int nbParam = this.instructionText.Split (new char[]{'$'}).Length - 1;
		if (nbParam > childCount) {
			var newchildren = new List<Parameter>();
			for (int i = childCount; i < nbParam; i++) {
				var newChild = addChild();
				newchildren.Add(newChild);
 			}
			foreach (Parameter newC in newchildren) {
				newC.reset();
			}
		} else if (nbParam < childCount) {
			GameObject[] children = this.GetChildren();
			for (int i = nbParam; i < childCount; i++) {

				GameObjectUtils.Destroy( children[i] );
			}
		}
		/*
		children.ForEach(child => GameObjectUtils.Destroy(child));*/
	}


	Parameter addChild(){
		GameObject go = GameObjectFactory.createGameObject ("Parameter", this.transform);
		Parameter parameter = go.AddComponent<Parameter> ();
		TextMesh textMesh = go.GetComponent<TextMesh> ();
		
		TextCollider2D textCollider = go.GetComponent<TextCollider2D>();
		textCollider.textMesh = textMesh;

		return parameter;
	}

	public void resetTexts (){
		string remainingText = instructionText;
		int indexOfValueTag = remainingText.IndexOf("$v");
		int x = 0;
		int childIndex = 0;
		textToShow = "";
		
		while(indexOfValueTag != -1){
			textToShow += remainingText.Substring(0,indexOfValueTag);
			remainingText 		= remainingText.Substring(indexOfValueTag+2);
			x += indexOfValueTag;
			indexOfValueTag 	= remainingText.IndexOf("$v");
			
			GameObject go = this.GetChild(childIndex);
			go.transform.SetPosition(new Vector3(x + this.transform.position.x,this.transform.position.y,0));

			TextCollider2D tc = go.GetComponent<TextCollider2D>();
			if(tc != null){
				x+= tc.text.Length;
				string spaces = createSpaces(tc.text.Length);
				textToShow += spaces;
			}else{
				Debug.LogWarning("Instruction wierd stuff");
			}
			childIndex++;
		}
		
		textToShow += remainingText;

		TextCollider2D instructionTC = this.GetComponent<TextCollider2D> ();
		instructionTC.text = textToShow;
		instructionTC.color = GameConstantes.instance.statementColor;
	}

	
	protected void notifyObservers(){
		foreach (var observer in observers) {
			observer.update();	
		}
	}

	string createSpaces(int nb){
		string space = "";
		for (int i = 0; i < nb; i++) {
			space+= " ";
		}
		return space;
	}
}
