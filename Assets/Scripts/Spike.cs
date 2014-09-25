using System.Collections;
using UnityEngine;

public class Spike : StateMachine {
	
	[Min] public float fallSpeed = 3;
	[Min] public float lifeTime = 3;
	
	[Separator]
	public int index;
	public float lifeCounter = 0;
	
	Animator animator;
	TextCollider2D textCollider2D;
	GameObject target;
	
	public override void Awake() {
		base.Awake();
		
		animator = GetComponent<Animator>();
		textCollider2D = GetComponent<TextCollider2D>();
		target = gameObject.FindChild("Target");
	}
	
	public virtual void Fall() {
		target.transform.position = Vector3.zero;
		target.rigidbody2D.isKinematic = false;
		textCollider2D.ColliderIsTrigger = false;
		CurrentState = Falling;
	}
	
	#region States
	public virtual void Idle() {
		if (References.SpikeManager.spikes[index] != this) {
			Invoke("Fall", 2.5F);
			CurrentState = WaitingToFall;
		}
	}
	
	public virtual void WaitingToFall() {
		target.rigidbody2D.gravityScale = fallSpeed;
	}
	
	public virtual void Falling() {
		
		lifeCounter += Time.deltaTime;
		if (lifeCounter >= lifeTime) {
			Despawn();
		}
	}
	#endregion
	
	#region Messages
	public override void OnSpawned() {
		base.OnSpawned();
		
		lifeCounter = 0;
		target.rigidbody2D.isKinematic = true;
		textCollider2D.ColliderIsTrigger = true;
		textCollider2D.Color = new Color(textCollider2D.Color.r, textCollider2D.Color.g, textCollider2D.Color.b, 0);
		animator.Play("Spike");
		CurrentState = Idle;
	}
	
	public override void OnDespawned() {
		base.OnDespawned();
		
		CancelInvoke("Fall");
		target.rigidbody2D.isKinematic = true;
		textCollider2D.ColliderIsTrigger = true;
	}
	
	public virtual void OnCollisionEnter2D(Collision2D collision) {
		IDeletable deletable = collision.gameObject.GetComponent(typeof(IDeletable)) as IDeletable;
		if (deletable != null){
			deletable.Delete();
		}
		Despawn();
	}
	#endregion
}
