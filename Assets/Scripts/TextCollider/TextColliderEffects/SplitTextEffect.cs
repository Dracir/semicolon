using UnityEngine;
using System.Collections;

public class SplitTextEffect : Effect {

	private int splitOnIndex;
	
	public SplitTextEffect(TextCollider2D textCollider2D, int splitOnIndex) : base(textCollider2D){
		this.splitOnIndex = splitOnIndex;
	}
	
	public override void onStart(){
		if(textCollider == null || splitOnIndex < 0){
			isDone = true;
			return;
		}
		Transform parent = textCollider.gameObject.transform.parent;
		float x = textCollider.gameObject.transform.position.x;
		float y = textCollider.gameObject.transform.position.y;
		
		string fullText = getText();
		textCollider.gameObject.Remove();
		
		string textGauche = fullText.Substring(0,splitOnIndex);
		TextCollider2D colliderLeft = TextColliderFactory.createTextCollider2D(textGauche, parent,new Vector3(x,y,0));
		colliderLeft.AddComponent<GameText>();
		
		if(splitOnIndex < fullText.Length){
			string textDroite = fullText.Substring(splitOnIndex+1);
			TextCollider2D colliderRight 	= TextColliderFactory.createTextCollider2D(textDroite, parent,new Vector3(x + splitOnIndex + 1,y,0));
			colliderRight.AddComponent<GameText>();
		}
		
	}

	string getText(){
		Instruction instruction = this.textCollider.GetComponent<Instruction>();
		if(instruction){
			return instruction.getFullText();
		}else{
			return textCollider.Text;
		}
	}
	public override void update(float deltaTime){
		isDone = true;
	}
	
	public override void onStop(){
		
	}
}
