using UnityEngine;
using System.Collections;

public class ColorChangeEffect : Effect {
	
	private Color fromColor;
	private Color toColor;
	
	private float time = 0;
	private float finalTime;
	public ColorChangeEffect(TextCollider2D textCollider, Color fromColor, Color toColor, float time):base(textCollider){
		this.fromColor = fromColor;
		this.toColor = toColor;
		this.finalTime = time;
	}
	
	public override void onStart(){
	}
	
	public override void update(float deltaTime){
		time += deltaTime;
		if(time >= finalTime){
			textCollider.Color = toColor;
			this.isDone = true;
		}else{
			Color newColor = Color32.Lerp(fromColor,toColor,time / finalTime);
			textCollider.Color = newColor;
		}

	}
	
	public override void onStop(){
		textCollider.Color = toColor;
	}

}
