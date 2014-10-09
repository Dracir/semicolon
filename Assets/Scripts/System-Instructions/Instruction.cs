using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TextMesh))]
[RequireComponent(typeof(TextCollider2D))]
[System.Serializable]
public class Instruction : MonoBehaviour, IDeletable {

	public List<Observer>		observers		= new List<Observer>();
	public List<DataType>		parameterType 	= new List<DataType> ();

	public string 				instructionText;
	private bool 				isComment;
	public bool					hasCompileSpot{get;private set;}
	private string 				textToShow;

	[Button(label:"Reset",methodName:"reset", NoPrefixLabel=true)]
	public bool resetBtn;

	public void setText(string text){
		instructionText = text;
		reset ();
	}
	
	public void reset(){
		this.name = instructionText;
		isComment = instructionText.StartsWith("//") || instructionText.StartsWith("/*");
		fixChildAmount();
		checkChildTypes();
		resetTexts ();
		notifyObservers ();
	}
	
	public void refresh(){
		resetTexts ();
		notifyObservers ();
	}
	
	public void compile(){
		refresh();
		EffectManager.AddGameEffect( GameConstantes.instance.currentTheme.createInstructionFlashEffect(this.GetComponent<TextCollider2D>()) );
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
		
		hasCompileSpot = this.instructionText.Contains("¶");
		if(hasCompileSpot){
			addCompileSpot();
		}
	}

	void addCompileSpot(){
		GameObject go = GameObjectFactory.createGameObject("Compile Spot", this.transform);
		go.layer =  LayerMask.NameToLayer("Parameter");
		TextCollider2D tc2d = go.AddComponent<TextCollider2D>();
		tc2d.Text = ";";
		tc2d.Color = GameConstantes.instance.currentTheme.compileSpotColor;
		tc2d.ColliderIsTrigger = true;
		tc2d.ColliderSize = Vector2.one * 2;
		go.AddComponent<CompileSpot>();
	}
	
	void checkChildTypes(){
		var children = this.GetChildren();
		int index = 0;
		foreach(var child in children){
			if(hasCompileSpot && index == children.Length - 1){
				break;
			}
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
		go.layer =  LayerMask.NameToLayer("Parameter");
		TextMesh textMesh = go.AddComponent<TextMesh> ();
		Parameter parameter = go.AddComponent<BooleanParameter>();
		TextCollider2D textCollider = go.GetComponent<TextCollider2D>();
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
	
	

	void IDeletable.Delete(Spike spike){
		int spikeX 	= (int) spike.transform.position.x;
		int parentX	= (int) this.transform.position.x;
		int x		= spikeX - parentX;
		EffectManager.AddGameEffect(new WaveDeleteTextEffect(this,1,x));
	}
	
	public void flashCompileEffect(){
		/*TextCollider2D textColliderHited = hitedParameter.GetComponent<TextCollider2D>();
		TextCollider2D textColliderInDrag = parameterDragged.GetComponent<TextCollider2D>();
		Color c1t1 = textColliderHited.Color;
		Color c1t0 = new Color(c1t1.r, c1t1.g, c1t1.b, 0);
		Color c2t1 = textColliderInDrag.Color;
		Color c2t0 = new Color(c1t1.r, c1t1.g, c1t1.b, 0);*/
		
		//EffectManager.AddGameEffect( new ColorChangeEffect(textColliderHited	,c1t0,c1t1, GameConstantes.instance.currentTheme.effetTimeOnInstructionSwap) );
		TextCollider2D instructionTC = this.GetComponent<TextCollider2D> ();
		
		EffectManager.AddGameEffect( new GradientEffet(instructionTC ,GameConstantes.instance.currentTheme.instructionFlash, GameConstantes.instance.currentTheme.effetTimeOnInstructionSwap) );
	}
}
