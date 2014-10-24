using UnityEngine;
using System.Collections;

public class MoveToMagical : MonoBehaviour {

	private Semicolon semicolon;
	private Instruction callingInstruction;
	
	private bool moveToActive = false;
	private bool flashedOnce = false;
	private Vector3 ToPosition;
	private Vector3 fromPosition;
	private float t;
	private float timeToMove;
	private float timeFrozen;

	void Start () {
		semicolon = this.GetComponent<Semicolon>();
	}
	
	
	void Update () {
		if(moveToActive){
			t+=Time.deltaTime;
			if(t <= timeToMove ){
				this.transform.position = Vector3.Lerp(fromPosition, ToPosition, t/timeToMove);
			}else {
				if(!flashedOnce){
					flashedOnce = true;
					callingInstruction.compile();
					callingInstruction.Remove();
				}
				if(t >= timeFrozen + timeToMove){
					moveToActive= false;
					semicolon.igniorePlayerInput = false;
					semicolon.ignioreVelocity = false;
					flashedOnce = false;
				}
				
			}
		}
	}
	
	
	public void startMoveTo(Instruction instruction, Vector3 positionTo, float timeToMove, float timeFrozen){
		this.ToPosition = positionTo;
		this.callingInstruction = instruction;
		this.fromPosition = this.transform.position;
		semicolon.igniorePlayerInput = true;
		semicolon.ignioreVelocity = true;
		moveToActive= true;
		this.timeToMove = timeToMove;
		this.timeFrozen = timeFrozen;
		t = 0;
		flashedOnce = false;
	}
}
