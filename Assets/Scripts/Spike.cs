using System.Collections;
using UnityEngine;

public class Spike : StateMachine {
	
	[Min] public float fallSpeed = 3;
	[Min] public float lifeTime = 3;
	
	[Separator]
	public SpikeManager spikeManager;
	public int index;
	public float lifeCounter;
	
	const float spawnAnimationSpeed = 5;
	
	TextCollider2D textCollider2D;
	GameObject target;
	
	public override void Awake() {
		base.Awake();
		
		textCollider2D = GetComponent<TextCollider2D>();
		target = gameObject.FindChild("Target");
	}
	
	public override IEnumerator Initialize() {
		yield return new WaitForSeconds(0);
		initialized = true;
		
		transform.position = spikeManager.transform.position - new Vector3(0, 2, 0);
	}
	
	public virtual void Fall() {
		textCollider2D.Color = new Color(textCollider2D.Color.r, textCollider2D.Color.g, textCollider2D.Color.b, 1);
		transform.position = spikeManager.transform.position;
		target.rigidbody2D.isKinematic = false;
		textCollider2D.ColliderIsTrigger = false;
		
		if (spikeManager.waitingSpike == this) {
			spikeManager.waitingSpike = null;
		}
		spikeManager.fallingSpikes.Add(this);
		
		CurrentState = Falling;
	}
	
	#region States
	public virtual void Spawning() {
		if (initialized) {
			textCollider2D.Color = Color.Lerp(textCollider2D.Color, new Color(textCollider2D.Color.r, textCollider2D.Color.g, textCollider2D.Color.b, 1), spawnAnimationSpeed * Time.deltaTime);
			transform.position = Vector3.Lerp(transform.position, spikeManager.transform.position, spawnAnimationSpeed);
		
			if ((spikeManager.transform.position.magnitude - transform.position.magnitude).Round(0.05) <= 0) {
				CurrentState = WaitingToFall;
			}
		}
	}
	
	public virtual void WaitingToFall() {
	}
	
	public virtual void Falling() {
		rigidbody2D.MovePosition((transform.position + (target.transform.position - transform.position).Round(1.66F)).Round(1.66F));
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
		target.transform.localPosition = Vector3.zero;
		target.rigidbody2D.isKinematic = true;
		target.rigidbody2D.gravityScale = fallSpeed;
		textCollider2D.ColliderIsTrigger = true;
		textCollider2D.Color = new Color(textCollider2D.Color.r, textCollider2D.Color.g, textCollider2D.Color.b, 0);
		CurrentState = Spawning;
	}
	
	public override void OnDespawned() {
		base.OnDespawned();
		
		CancelInvoke("Fall");
		if (spikeManager != null && spikeManager.fallingSpikes.Contains(this)) {
			spikeManager.fallingSpikes.Remove(this);
		}
		target.rigidbody2D.isKinematic = true;
		textCollider2D.ColliderIsTrigger = true;
	}
	
	public virtual void OnCollisionEnter2D(Collision2D collision) {
		IDeletable deletable = collision.gameObject.GetComponent(typeof(IDeletable)) as IDeletable;
		if (deletable != null) {
			deletable.Delete();
		}
		Despawn();
	}
	#endregion
}
