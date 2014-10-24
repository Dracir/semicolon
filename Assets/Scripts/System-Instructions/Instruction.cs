using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TextMesh))]
[RequireComponent(typeof(TextCollider2D))]
[System.Serializable]
public class Instruction : GameText {

	public List<Observer>		observers		= new List<Observer>();

	[SerializeField]
	private string 				instructionText;
	[SerializeField]
	private bool 				isComment;
	public bool					hasCompileSpot{get;private set;}
	[SerializeField]
	private string 				textToShow;


	public void setText(string text){
		instructionText = text;
		hasCompileSpot = this.instructionText.Contains("¶");
	}
	
	public void refresh(){
		resetTexts ();
		notifyObservers ();
	}
	
	public void compile(){
		refresh();
		flashCompileEffect();
		int childCount = this.GetChildCount();
		if(this.hasCompileSpot) childCount--;
		for (int i = 0; i < childCount; i++) {
			Parameter childParam = this.GetChild(i).GetComponent<Parameter>();
			if(childParam){
				childParam.notifyObservers();
			}
		}
	}

	

	public void addCompileSpot(){
		GameObject go = GameObjectFactory.createGameObject("Compile Spot", this.transform);
		go.layer =  LayerMask.NameToLayer("Parameter");
		TextCollider2D tc2d = go.AddComponent<TextCollider2D>();
		tc2d.Text = ";";
		tc2d.Color = GameConstantes.instance.currentTheme.compileSpotColor;
		tc2d.ColliderIsTrigger = true;
		tc2d.ColliderSize = Vector2.one * 2;
		go.AddComponent<CompileSpot>();
		this.hasCompileSpot = true;
	}
	
	
	public BooleanParameter addBooleanChild(){
		GameObject go = createChild();
		BooleanParameter parameter = go.AddComponent<BooleanParameter>();
		return parameter;
	}
	
	public IntegerParameter addIntegerChild(){
		GameObject go = createChild();
		IntegerParameter parameter = go.AddComponent<IntegerParameter>();
		return parameter;
	}
	
	private GameObject createChild(){
		GameObject go = GameObjectFactory.createGameObject ("Int", this.transform);
		go.layer =  LayerMask.NameToLayer("Parameter");
		TextMesh textMesh = go.AddComponent<TextMesh> ();
		TextCollider2D textCollider = go.AddComponent<TextCollider2D>();
		textCollider.TextMesh = textMesh;
		return go;
	}

	public Parameter addChild(DataType datatype){
		GameObject go = GameObjectFactory.createGameObject ("Parameter", this.transform);
		go.layer =  LayerMask.NameToLayer("Parameter");
		TextMesh textMesh = go.AddComponent<TextMesh> ();
		Parameter parameter = go.AddComponent<BooleanParameter>();
		TextCollider2D textCollider = go.AddComponent<TextCollider2D>();
		textCollider.TextMesh = textMesh;

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
		x+= remainingText.Length;
		
		if(hasCompileSpot){
			textToShow = textToShow.Substring(0,textToShow.IndexOf("¶"));
			x -= 3;
			GameObject compileSpot = this.GetChild(this.GetChildCount()-1);
			compileSpot.transform.SetPosition(new Vector3(x + this.transform.position.x,this.transform.position.y,0));
			
		}
		

		TextCollider2D instructionTC = this.GetComponent<TextCollider2D> ();
		instructionTC.Text = textToShow;
		if(isComment){
			instructionTC.Color = GameConstantes.instance.currentTheme.commentColor;
		}else{
			instructionTC.Color = GameConstantes.instance.currentTheme.instructionColor;
		}
		
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

	
	public void notifyObservers(){
		if(!Application.isPlaying) return;
		
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
		transform.DestroyChildren();
	}
	
	public void flashCompileEffect(){
		TextCollider2D instructionTC = this.GetComponent<TextCollider2D> ();
		Effect effect = GameConstantes.instance.currentTheme.createInstructionFlashEffect(instructionTC, true);
		EffectManager.AddGameEffect( effect );
	}
}
