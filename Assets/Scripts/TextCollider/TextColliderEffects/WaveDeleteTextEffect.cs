using UnityEngine;
using System.Collections;

public class WaveDeleteTextEffect : Effect {

	private Instruction instruction;
	private float acumulatedTime;
	private float speed;
	private int distanceFromCenter = 0;
	private int centerCharacter;
	private int nSpaces = 0;
	public WaveDeleteTextEffect(Instruction instruction, float speed, int startingCharacter) : base(instruction.GetComponent<TextCollider2D>()){
		this.instruction = instruction;
		this.speed = speed / Mathf.Max(startingCharacter, instruction.getFullText().Length - startingCharacter);
		this.acumulatedTime = 0;
		this.centerCharacter = startingCharacter;
	}
	
	public override void onStart(){
		string fullText = this.instruction.getFullText();
		this.instruction.Remove();
		this.textCollider.Text = fullText;
	}

	public override void update(float deltaTime){
		acumulatedTime +=deltaTime;
		if(acumulatedTime > speed){
			acumulatedTime -= speed;
			removeCharacter();
		}
	}
	
	void removeCharacter(){
		string text = this.textCollider.Text;
		distanceFromCenter++;
		int beforelenght = Mathf.Max(this.centerCharacter - distanceFromCenter,0);
		int afterStartIndex = Mathf.Min(this.centerCharacter + distanceFromCenter,text.Length);
		if(beforelenght <= 0 && afterStartIndex >= text.Length){
			this.isDone = true;
		}else{
			if(this.centerCharacter - distanceFromCenter >= 0){
				this.nSpaces++;
			}
			if(this.centerCharacter + distanceFromCenter <= text.Length){
				this.nSpaces++;
			}
			string beforeStr		= text.Substring(0,beforelenght);
			string centerStr 		= createSpace(nSpaces);
			string afterStr 		= text.Substring(afterStartIndex);
			this.textCollider.Text 	= beforeStr + centerStr + afterStr;
		}
	}
	
	private string createSpace(int nb){
		string str = "";
		for (int i = 0; i < nb; i++) {
			str+= " ";
		}
		return str;
	}
	
	public override void onStop(){
		Object.Destroy(this.textCollider.gameObject);
	}
}
