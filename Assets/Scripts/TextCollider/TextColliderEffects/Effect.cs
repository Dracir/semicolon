using UnityEngine;
using System.Collections;

public abstract class Effect {

	protected TextCollider2D textCollider;
	public bool isDone = false;
	
	public Effect(TextCollider2D textCollider){
		this.textCollider = textCollider;
	}
	
	abstract public void onStart();
	abstract public void update(float deltaTime);
	abstract public void onStop();
}
