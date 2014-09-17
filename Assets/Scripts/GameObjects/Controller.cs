using UnityEngine;
using System.Collections;


public class Controller {
	public bool getRun;
	public bool getRunDown;
	public bool getRunUp;
	
	public float lastRunDownTime;
	private bool getRunLast;
	public bool isSpammingRun;
	private readonly float spamResetTimer = 0.3f;
	
	public bool getJump;
	public bool getJumpDown;
	public bool getJumpUp;
	
	private bool getJumpLast;
	private float lastJumpTime;
	private float jumpInputLeeway = 0.2f;
	
	public bool getL;
	public bool getLUp;
	public bool getLDown;
	
	public bool getR;
	public bool getRUp;
	public bool getRDown;
	
	public bool getD;
	public bool getDDown;
	public bool getDUp;
	
	public bool getU;
	public bool getUUp;
	public bool getUDown;
	
	public bool doubleTap;
	public bool aboutFace;

	public bool locked;
	
	public float hAxis;
	public float vAxis;
	
	private float hAxisLast = 0.0f;
	private float vAxisLast = 0.0f;
	private float getLLastTime = 0f;
	private float getRLastTime = 0f;
	private float doubleTapTime = 0.188f;
	
//	private static KeyCode jump;
//	private static KeyCode run;
	
	public Controller(){
		/*try{
			jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Button0", "Z"));
			run = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Button1", "X"));
		}catch(System.Exception){
			jump = KeyCode.Z;
			run = KeyCode.X;
		}*/

		 
	}
	
	public void GetInputs(){
		doubleTap = false;
		if (locked) return;
		
//		getRun = Input.GetButton("Run") || Input.GetKey(run);
//		getRunDown = Input.GetButtonDown("Run") || Input.GetKeyDown(run);
//		getRunUp = Input.GetButtonUp("Run") || Input.GetKeyUp(run);
		getRun = Input.GetButton("Run");
		getRunDown = Input.GetButtonDown("Run");
		getRunUp = Input.GetButtonUp("Run");
		
		if (getRunDown){
			isSpammingRun = Time.time - lastRunDownTime < spamResetTimer;
			
			lastRunDownTime = Time.time;
			
		}
		
//		getJump = Input.GetButton("Jump") || Input.GetKey(jump);
//		getJumpUp = Input.GetButtonUp("Jump") || Input.GetKeyUp(jump);
		getJump = Input.GetButton("Jump");
		getJumpUp = Input.GetButtonUp("Jump");
		
		if (getJump && !getJumpLast){
			lastJumpTime = Time.time;
		}
		
		getJumpDown = Time.time - lastJumpTime < jumpInputLeeway;
		
		getJumpLast = getJump;
		
		hAxis = Input.GetAxis("Horizontal");
		
		getL = hAxis < -0.3f;
		getR = hAxis > 0.3f;
		
		getLDown = (hAxisLast < 0.3f && hAxisLast > -0.3f) && getL;
		getRDown = (hAxisLast < 0.3f && hAxisLast > -0.3f) && getR;
		
		getLUp = hAxisLast < -0.3f && !getL;
		getRUp = hAxisLast > 0.3f && !getR;
		
		if (getLDown){						//check for doubleTap
			if (Time.time - getLLastTime < doubleTapTime){
				doubleTap = true;
			}
			getLLastTime = Time.time;
		}
		
		if (getRDown){
			if (Time.time - getRLastTime < doubleTapTime){
				doubleTap = true;
			}
			getRLastTime = Time.time;
		}
		hAxisLast = hAxis;
		
		VerticalInputs();
	}
	
	public void CursorInput(){
		VerticalInputs();
		
	}
	
	private void VerticalInputs(){
		vAxis = Input.GetAxis("Vertical");
		
		getD = vAxis < -0.3f;
		getU = vAxis > 0.3f;
		
		getDDown = (vAxisLast < 0.3f && vAxisLast > -0.3f) && getD;
		getUDown = (vAxisLast < 0.3f && vAxisLast > -0.3f) && getU;
		
		getDUp = vAxisLast < -0.3f && !getD;
		getUUp = vAxisLast > 0.3f && !getU;
		vAxisLast = vAxis;
	}
	
	public void ResetJumpInput(){
		lastJumpTime = 0;
		getJumpDown = false;
	}
	
}