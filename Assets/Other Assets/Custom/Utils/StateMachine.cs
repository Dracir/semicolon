using UnityEngine;
using System.Collections;

public class StateMachine : PooledObject {

	public string currentState;
	public string fixedCurrentState;
	public string lateCurrentState;
	
	public delegate void State();
	public State CurrentState;
	public State FixedCurrentState;
	public State LateCurrentState;
	
	public virtual void Awake() {
		CurrentState = Normal;
		FixedCurrentState = FixedNormal;
		LateCurrentState = LateNormal;
	}
	
	public virtual void Update() {
		currentState = CurrentState.Method.Name;
		CurrentState();
	}
	
	public virtual void FixedUpdate() {
		fixedCurrentState = FixedCurrentState.Method.Name;
		FixedCurrentState();
	}
	
	public virtual void LateUpdate() {
		lateCurrentState = LateCurrentState.Method.Name;
		LateCurrentState();
	}
	
	public virtual void Normal() {
	}
	
	public virtual void FixedNormal() {
	}
	
	public virtual void LateNormal() {
	}
	
}
