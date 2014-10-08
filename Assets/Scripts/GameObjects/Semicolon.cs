using UnityEngine;
using System.Collections;

public class Semicolon : Movable {
	
	public float maxGroundSpeed;
	public float hopForce;
	public float extraHopFromMovement;
	public float hopAfter = 0.16f;
	
	private float jumpParamModifier = 1f;
	
	public ArgumentSensor tetherObject;
	
	public static Semicolon instance;
	
	public bool igniorePlayerInput = false;
	public bool ignioreVelocity = false;
	
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

	
	protected override void Start () {
		base.Start();
	}
	

	protected override void Update () {
		if(ignioreVelocity) return;
		
		controller.GetInputs();
		
		if (hasGravity){
			HandleGravity();
		}
		if (velocity.y > 0){
			CheckHeadBump();
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
		if(igniorePlayerInput) return ; 
		base.Jump ();
		hasJumped = true;
	}
	
	public void UpdateTetherPosition (Vector2 position){
		tetherObject.transform.position = position;
	}
	
	protected override void HandleMovement (float input)
	{
		if(igniorePlayerInput) return ; 
		
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
	
	
	public void SetJumpHeight (int param){
		jumpForce = param * jumpParamModifier;
	}
}
