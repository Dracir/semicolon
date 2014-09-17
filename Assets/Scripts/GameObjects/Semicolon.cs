using UnityEngine;
using System.Collections;

public class Semicolon : Movable {
	
	public float maxGroundSpeed;
	public float hopForce;
	public float extraHopFromMovement;
	public float hopAfter = 0.16f;
	
	
	public static Semicolon instance;
	
	protected override float GetMaxSpeed() {
		if (grounded){
			return maxGroundSpeed;
		}
		
		return maxSpeed;
	}
	
	private float GetHopForce (){
		if (velocity.x != 0){
			return hopForce + extraHopFromMovement;
		} else{
			return hopForce;
		}
	}
	
	float onGroundTimer = 0;
	
	bool hasJumped = false;
	
	protected void Awake () {
		instance = this;
	}
	// Use this for initialization
	protected override void Start () {
		base.Start();
		
	}
	
	// Update is called once per frame
	protected override void Update () {
		controller.GetInputs();
		
		if (hasGravity){
			HandleGravity();
		}
		
		HandleMovement(controller.hAxis);
		
		if (!hasJumped && (controller.getJumpDown || controller.getUDown)){
			Jump();
		}
		
		t.Translate(velocity * Time.deltaTime);
	}
	
	protected override void Grounded ()
	{
		base.Grounded ();
		hasJumped = false;
	}
	
	protected override void Jump ()
	{
		base.Jump ();
		hasJumped = true;
	}
	
	protected override void HandleMovement (float input)
	{
		base.HandleMovement (input);
		
		if (grounded && input != 0){
			onGroundTimer += Time.deltaTime;
			if (onGroundTimer > hopAfter){
				grounded = false;
				velocity = new Vector2(velocity.x, hopForce);
			}
		} else {
			onGroundTimer = 0;
		}
		
		
		
		
	}
	
	
	
	
}
