using UnityEngine;
using System.Collections;

public class GradientEffet : Effect {

	private Gradient gradient;
	
	private float time = 0;
	private float finalTime;
	private bool removeInstructionAfterEffect;
	
	public GradientEffet(TextCollider2D textCollider, Gradient gradient, float time, bool removeInstructionAfterEffect):base(textCollider){
		this.gradient = gradient;
		this.removeInstructionAfterEffect = removeInstructionAfterEffect;
		this.finalTime = time;
	}
	
	public override void onStart(){
	}
	
	public override void update(float deltaTime){
		time += deltaTime;
		if(time >= finalTime){
			textCollider.Color = gradient.Evaluate(1);
			this.isDone = true;
		}else{
			textCollider.Color = gradient.Evaluate(time / finalTime);
		}
	}
	
	public override void onStop(){
		textCollider.Color = gradient.Evaluate(1);
		if(removeInstructionAfterEffect){
			textCollider.gameObject.Remove();
		}
	}
}
