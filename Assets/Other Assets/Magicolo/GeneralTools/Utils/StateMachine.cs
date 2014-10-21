using UnityEngine;
using System.Collections;

namespace Magicolo.GeneralTools {
	public class StateMachine : PooledObject {

		public string state;
		public string fixedState;
		public string lateState;
	
		public delegate void State();
		State currentState;
		public State CurrentState {
			get {
				return currentState;
			}
			set {
				currentState = value;
				state = currentState.Method.Name;
			}
		}

		State fixedCurrentState;
		public State FixedCurrentState {
			get {
				return fixedCurrentState;
			}
			set {
				fixedCurrentState = value;
				fixedState = fixedCurrentState.Method.Name;
			}
		}
	
		State lateCurrentState;
		public State LateCurrentState {
			get {
				return lateCurrentState;
			}
			set {
				lateCurrentState = value;
				lateState = lateCurrentState.Method.Name;
			}
		}
	
		public virtual void Awake() {
			CurrentState = Normal;
			FixedCurrentState = FixedNormal;
			LateCurrentState = LateNormal;
		}
	
		public virtual void Update() {
			CurrentState();
		}
	
		public virtual void FixedUpdate() {
			FixedCurrentState();
		}
	
		public virtual void LateUpdate() {
			LateCurrentState();
		}
	
		public virtual void Normal() {
		}
	
		public virtual void FixedNormal() {
		}
	
		public virtual void LateNormal() {
		}
	
	}
}
