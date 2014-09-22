using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LookDirections {
	left = -1,
	centre = 0,
	right = 1
}

public enum CheckDirections {
	left, up, right, down
}
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Movable : MonoBehaviour {
	
	//---------------------------------------------------\\
	//-----------------Global Properties-----------------\\
	//---------------------------------------------------\\
	
	public const float gravity = 22f;
	
	//---------------------------------------------------\\
	//------------------Component Vars-------------------\\
	//---------------------------------------------------\\
	
	protected Transform t;
	protected Rigidbody2D rb;
	protected BoxCollider2D boxCol;
	
	protected Controller controller;
	
	//---------------------------------------------------\\
	//------------------Dependent Vars-------------------\\
	//---------------------------------------------------\\
	
	protected Vector2 velocity;
	protected LookDirections facing;
	
	protected bool falling;
	protected bool grounded;
	
	//collision detection
	
	protected readonly int vertRays = 5;
	protected readonly int horiRays = 6;
	protected readonly float marginPercent = 0.1f;
	
	
	//---------------------------------------------------\\
	//----------------Movement Variables-----------------\\
	//---------------------------------------------------\\
	
	public float maxSpeed;
	public float timeToMax;
	public float timeToStop;
	public float jumpForce;
	
	public bool hasGravity;
	
	protected float stopLeeway = 0.02f;
	
	//---------------------------------------------------\\
	//-------------------Properties----------------------\\
	//---------------------------------------------------\\
	
	protected Rect Box {
		get{
			if (!boxCol){
				boxCol = GetComponent<BoxCollider2D>();
				if (!boxCol){
					Debug.LogWarning("There's a problem with your bounds, this isn't a rectangle! or whatevs");
					return new Rect(0, 0, 0, 0);
				}
			}
			Vector2 pos = t.position;
			Vector2 size = boxCol.size;
			Vector2 centre = boxCol.center;
			return new Rect(pos.x + centre.x/2 - size.x/2, pos.y + centre.y - size.y/2, size.x, size.y);
		}
	}
	
	protected virtual int GravityLayers {
		get{
			return 1 >> LayerMask.NameToLayer("Block");
		}
	}
	protected virtual int MoveLayers {
		get{
			return 1 >> LayerMask.NameToLayer("Default");
		}
	}
	
	protected virtual int HeadLayers {
		get{
			return 1 >> LayerMask.NameToLayer("Default");
		}
	}
	
	//---------------------------------------------------\\
	//----------------Getters and setters----------------\\
	//---------------------------------------------------\\
	
	protected Vector2 VectorFromDirection(CheckDirections direction){
		switch (direction){
		case CheckDirections.left:
			return -Vector2.right;
		case CheckDirections.right:
			return Vector2.right;
		case CheckDirections.up:
			return Vector2.up;
		case CheckDirections.down:
			return -Vector2.up;
		default:
			Debug.LogWarning("Not a valid check direction!");
			return Vector2.zero;
		}
	}
	
	protected virtual float GetMargin (bool vertical) {
		float side = vertical? Box.height : Box.width;
		return side * marginPercent;
	}
	
	protected virtual float GetMaxSpeed () {
		return maxSpeed;
	}
	
	
	protected virtual float Accelerate (float xVelocity, float input) {
		float max = GetMaxSpeed();
		float newX = velocity.x + input * (max / timeToMax) * Time.deltaTime;
		newX = Mathf.Clamp(newX, -max, max);
		return newX;
	}
	
	protected virtual float Decelerate (float xVelocity){
		float max = GetMaxSpeed();
		float modifier = xVelocity > 0? -1 : 1;
		float newX = xVelocity + modifier * (max / timeToStop) * Time.deltaTime;
		return newX;
	}
	
	protected virtual float GetJumpForce() {
		return jumpForce;
	}
	
	//---------------------------------------------------\\
	//-----------------Other checkers--------------------\\
	//---------------------------------------------------\\
	
	protected bool IsAWall(Vector2 point){
		 
		
		return false;
	}
	
	//---------------------------------------------------\\
	//-------------------Delegates etc-------------------\\
	//---------------------------------------------------\\
	
	protected delegate void CollisionEvent();
	protected CollisionEvent sideCollide;
	protected CollisionEvent upCollide;
	protected CollisionEvent downCollide;
	
	//---------------------------------------------------\\
	//------------------------DEV------------------------\\
	//---------------------------------------------------\\
	
	public LayerMask myMask;
	private GameObject devBall;
	
	//---------------------------------------------------\\
	//-----------------Practical methods-----------------\\
	//---------------------------------------------------\\
	
	// Use this for initialization
	protected virtual void Start () {
		t = transform;
		rb = rigidbody2D;
		boxCol = GetComponent<BoxCollider2D>();
		controller = new Controller();
		
		rb.gravityScale = 0;
		
		//DEV
		devBall = Resources.Load("DevBall") as GameObject;
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		//controller.GetInputs();
		if (hasGravity)
			HandleGravity();
	
		//HandleJump();
		
		t.Translate(velocity * Time.deltaTime);
	}
	
	protected virtual void HandleGravity () {
		float newYVelocity = velocity.y;
		bool checkDown = false;
		float checkDistance = 0;
		if (!grounded){
			newYVelocity -= gravity * Time.deltaTime;
			if (newYVelocity < 0){
				checkDown = true;
				checkDistance = Mathf.Abs(newYVelocity * Time.deltaTime);
				if (!falling){
					falling = true;
					Instantiate(devBall, t.position, t.rotation);
				}
			}
		} else {
			checkDown = true;
			//checkDistance = GetMargin(true);
			checkDistance = 0;
		}
		
		if (checkDown){
			RaycastHit2D[] hits = CollisionCheck(checkDistance, myMask, CheckDirections.down, Grounded);
			RaycastHit2D useData = new RaycastHit2D();
			float shortest = Mathf.Infinity;
			
			foreach (RaycastHit2D hit in hits){
				if (hit.fraction < shortest && hit.fraction > 0){
					shortest = hit.fraction;
					useData = hit;
				}
			}
			if (useData.fraction > 0 && !grounded){
				newYVelocity = 0;
				t.position = new Vector3(t.position.x, useData.point.y + boxCol.size.y/2, t.position.z);
				grounded = true;
				falling = false;
			} else if (useData.fraction == 0){
				grounded = false;
			}
		}
		
		
		velocity = new Vector2(velocity.x, newYVelocity);
	}
	
	protected virtual void Jump () {
		grounded = false;
		velocity = new Vector2(velocity.x, GetJumpForce());
	}
	
	protected virtual void CheckHeadBump () {
		CollisionCheck(velocity.y * Time.deltaTime, HeadLayers, CheckDirections.up, HeadBump);
//		RaycastHit2D[] rays = CollisionCheck(velocity.y * Time.deltaTime, HeadLayers, CheckDirections.up, HeadBump);
		
	}
	
	protected virtual void HeadBump () {
		velocity = new Vector2(velocity.x, 0);
		//TODO play sound?
	}
	
	protected virtual void Grounded () {
		
	}
	
	protected virtual void HandleMovement (float input) {
		float newXVelocity = velocity.x;
		
		facing = (LookDirections) input;
		
		if (input != 0){
			newXVelocity = Accelerate(newXVelocity, input);
		}
		
		//apply deceleration if I'm not pressing anything or if I'm pressing the opposite from where I'm going (turning around)
		if ((input == 0 && Mathf.Abs(newXVelocity) > stopLeeway && grounded) || ((input > 0 && newXVelocity < 0) || (input < 0 && newXVelocity > 0))){
			newXVelocity = Decelerate(newXVelocity);
		} else if (Mathf.Abs(newXVelocity) <= stopLeeway){
			newXVelocity = 0;
		}
		
		if (newXVelocity != 0){
			CheckDirections checkDirection = newXVelocity > 0? CheckDirections.right : CheckDirections.left;
			RaycastHit2D[] hits = CollisionCheck(Mathf.Abs(newXVelocity * Time.deltaTime), MoveLayers, checkDirection, HitWall);
			//RaycastHit2D[] hits = CollisionCheck(Mathf.Abs(newXVelocity * Time.deltaTime), MoveLayers, CheckDirections.down, HitWall);
			
			if (hits.Length > 0){
				RaycastHit2D useData = new RaycastHit2D();
				foreach (RaycastHit2D hit in hits){
//					if (IsAWall(hit.normal)){
					if (hit.fraction > 0){
						useData = hit;
						break;
					}
				}
				
				
				if (useData.fraction > 0 && velocity.x != 0){
					t.position = new Vector3(useData.point.x - boxCol.size.x/2 * (velocity.x > 0? 1 : -1), t.position.y, t.position.z);
					newXVelocity = 0;
				}
			}
		}
		
		
		velocity = new Vector2(newXVelocity, velocity.y);
	}
	
	protected virtual void HitWall () {
	}
	
	public bool debug = false;
	protected void Log (object message){
		if (debug){
			Debug.Log(message);
			
		}
	}
	
	//generic collision check for any direction. Needs BoxCollider.
	protected virtual RaycastHit2D[] CollisionCheck (float distance, int mask, CheckDirections dir, CollisionEvent collideMethod){
		
		List<RaycastHit2D> hits = new List<RaycastHit2D>();
		Vector2 direction = VectorFromDirection(dir);
		Vector2 centre = Box.center;
		
		//in this case, start and end represent the first and last points from which we'll cast rays
		Vector2 startDirection = VectorFromDirection (dir == CheckDirections.left? CheckDirections.down : (dir - 1));
		Vector2 endDirection = VectorFromDirection (dir == CheckDirections.down? CheckDirections.left : (dir + 1));
		
		bool vertical = (dir == CheckDirections.down) || (dir == CheckDirections.up);
		float sideModifier = vertical ? Box.width - GetMargin(true): Box.height - GetMargin(false);
		float rayDistance = vertical? (distance + Box.height/2) : (distance + Box.width/2);
		
		Vector2 start = centre + startDirection * sideModifier/2;
		Vector2 end = centre + endDirection * sideModifier/2;
		int rayNumber = vertical? vertRays : horiRays;
		bool hit = false;
		
		for (int i = 0; i < rayNumber; i ++){
			float lerpAmount = (float) i / (float) (rayNumber - 1);
			Vector2 origin = Vector2.Lerp(start, end, lerpAmount);
			RaycastHit2D hitInfo;
			//Vector2 target = origin + direction * rayDistance; //for use in Linecast if that's better
			hitInfo = Physics2D.Raycast(origin, direction, rayDistance, MoveLayers);
			
			if (hitInfo.collider != null){
				hit = true;
				hits.Add(hitInfo);
			}
		}
		
		if (hit){
			if (collideMethod != null){
				collideMethod();
			}
		}
		return hits.ToArray();
		
	}
}
