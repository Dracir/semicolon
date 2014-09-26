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
		checkChildTypes();
		resetTexts ();
		notifyObservers ();
	}
	
	public void refresh(){
		resetTexts ();
		notifyObservers ();
	}


	void fixChildAmount(){

		int childCount = this.GetChildCount();
		int nbParam = this.instructionText.Split (new char[]{'$'}).Length - 1;
		if (nbParam > childCount) {
			var newchildren = new List<Parameter>();
			for (int i = this.parameterType.Count; i < nbParam; i++) {
				this.parameterType.Add(DataType.BOOLEAN);
			}
			for (int i = childCount; i < nbParam; i++) {
				var newChild = addChild(this.parameterType[i]);
				newchildren.Add(newChild);
 			}
			foreach (var newC in newchildren) {
				newC.refresh();
			}
		} else if (nbParam < childCount) {
			GameObject[] children = this.GetChildren();
			for (int i = this.parameterType.Count -1; i >= nbParam; i--) {
				this.parameterType.RemoveAt(i);
			}
			for (int i = nbParam; i < childCount; i++) {
				GameObjectUtils.Destroy( children[i] );
			}
		}
	}
	
	void checkChildTypes(){
		var children = this.GetChildren();
		int index = 0;
		foreach(var child in children){
			Parameter p = child.GetComponent<Parameter>();
			DataType dataType = this.parameterType[index];
			if(!p.isOfType(dataType)){
				changeDataTypeOfChild(child, dataType);
			}
			p = child.GetComponent<Parameter>();
			p.refresh();
			index++;
		}
	}
	
	public void setParameterTo(int index, DataType dataType){
		this.parameterType[index] = dataType;
		GameObject child = this.GetChild(index);
		changeDataTypeOfChild(child,dataType);
	}

	void changeDataTypeOfChild(GameObject child, DataType dataType){
		child.RemoveComponent<BooleanParameter>();
		child.RemoveComponent<IntegerParameter>();
		switch(dataType){
				case DataType.BOOLEAN : child.AddComponent<BooleanParameter>();
				break;
				case DataType.INTEGER : child.AddComponent<IntegerParameter>();
				break;
		}
	}

	Parameter addChild(DataType datatype){
		GameObject go = GameObjectFactory.createGameObject ("Parameter", this.transform);
		TextMesh textMesh = go.AddComponent<TextMesh> ();
		Parameter parameter = go.AddComponent<BooleanParameter>();
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
				x+= tc.Text.Length;
				string spaces = createSpaces(tc.Text.Length);
				textToShow += spaces;
			}else{
				Debug.LogWarning("Instruction wierd stuff");
			}
			childIndex++;
		}
		
		textToShow += remainingText;

		TextCollider2D instructionTC = this.GetComponent<TextCollider2D> ();
		instructionTC.Text = textToShow;
		instructionTC.Color = GameConstantes.instance.currentTheme.statementColor;
	}
	
	public string getFullText(){
		string remainingText = instructionText;
		int indexOfValueTag = remainingText.IndexOf("$v");
		int x = 0;
		int childIndex = 0;
		string fullText = "";
		
		while(indexOfValueTag != -1){
			fullText += remainingText.Substring(0,indexOfValueTag);
			remainingText 		= remainingText.Substring(indexOfValueTag+2);
			x += indexOfValueTag;
			indexOfValueTag 	= remainingText.IndexOf("$v");
			
			GameObject go = this.GetChild(childIndex);
			go.transform.SetPosition(new Vector3(x + this.transform.position.x,this.transform.position.y,0));

			TextCollider2D tc = go.GetComponent<TextCollider2D>();
			if(tc != null){
				fullText += tc.Text;
				x+= tc.Text.Length;
			}else{
				Debug.LogWarning("Instruction wierd stuff");
			}
			childIndex++;
		}
		
		fullText += remainingText;
		return fullText;
	}

	
	protected void notifyObservers(){
		foreach (var observer in observers) {
			observer.notify();	
		}
	}

	string createSpaces(int nb){
		string space = "";
		for (int i = 0; i < nb; i++) {
			space+= " ";
		}
		return space;
	}
	
	
	void OnDestroy(){
		Extensions.DestroyChildren(transform);
	}
}
