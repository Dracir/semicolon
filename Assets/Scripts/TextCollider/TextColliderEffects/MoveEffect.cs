using UnityEngine;
using System.Collections;

public class MoveEffect : Effect {

	private Vector3 fromPosition;
	private Vector3 toPosition;
	
	private float time = 0;
	private float finalTime;
	
	private bool smooth;
	
	
	public MoveEffect(TextCollider2D textCollider, Vector3 to, float time, bool smooth):base(textCollider){
		this.fromPosition = textCollider.transform.position;
		this.toPosition = to;
		this.finalTime = time;
		this.smooth = smooth;
	}
	
	public override void onStart(){
	}
	
	public override void update(float deltaTime){
		time += deltaTime;
		if(time >= finalTime){
			textCollider.transform.SetPosition(toPosition);
			this.isDone = true;
		}else{
			if(smooth){
				textCollider.transform.SetPosition(Vector3.Lerp(fromPosition, toPosition, time/finalTime));
			}else{
				Vector3 smoothPosition = Vector3.Lerp(fromPosition, toPosition, time/finalTime);
				textCollider.transform.SetPosition(new Vector3(smoothPosition.x.Round(1), smoothPosition.y.Round(1.66), smoothPosition.z));
			}
			
		}
	}
	
	public override void onStop(){
			textCollider.transform.SetPosition(toPosition);
	}
}
