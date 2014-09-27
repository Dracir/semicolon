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
	SpikeTarget target;
	BoxCollider2D targetCollider;
	ColorChangeEffect colorEffect;
	MoveEffect moveEffect;
	
	public override void Awake() {
		base.Awake();
		
		textCollider2D = GetComponent<TextCollider2D>();
		textCollider2D.Color = GameConstantes.instance.statementColor;
		textCollider2D.Font = GameConstantes.instance.statementFont;
		target = gameObject.FindChild("Target").GetComponent<SpikeTarget>();
		target.parent = this;
		targetCollider = target.gameObject.AddCopiedComponent(GetComponent<BoxCollider2D>());
	}
	
	public override IEnumerator Initialize() {
		yield return new WaitForSeconds(0);
		initialized = true;
		
		if (spikeManager != null) {
			transform.position = spikeManager.transform.position + new Vector3(0, 2, 0);
			colorEffect = new ColorChangeEffect(textCollider2D, textCollider2D.Color, new Color(textCollider2D.Color.r, textCollider2D.Color.g, textCollider2D.Color.b, 1), 2);
			EffectManager.AddGameEffect(colorEffect);
			moveEffect = new MoveEffect(textCollider2D, spikeManager.transform.position, 2, true);
			EffectManager.AddGameEffect(moveEffect);
		}
	}
	
	public virtual void Fall() {
		textCollider2D.Color = new Color(textCollider2D.Color.r, textCollider2D.Color.g, textCollider2D.Color.b, 1);
		transform.position = spikeManager.transform.position;
		target.rigidbody2D.isKinematic = false;
		targetCollider.isTrigger = false;
		
		if (spikeManager.waitingSpike == this) {
			spikeManager.waitingSpike = null;
		}
		spikeManager.fallingSpikes.Add(this);
		
		if (colorEffect != null) {
			colorEffect.isDone = true;
		}
		if (moveEffect != null){
			moveEffect.isDone = true;
		}
		
		CurrentState = Falling;
	}
	
	#region States
	public virtual void WaitingToFall() {
	}
	
	public virtual void Falling() {
		rigidbody2D.MovePosition(new Vector3(target.transform.position.x.Round(1), target.transform.position.y.Round(1.66), target.transform.position.z));
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
		
		targetCollider.isTrigger = true;
		textCollider2D.Color = new Color(textCollider2D.Color.r, textCollider2D.Color.g, textCollider2D.Color.b, 0);
		
		CurrentState = WaitingToFall;
	}
	
	public override void OnDespawned() {
		base.OnDespawned();
		
		CancelInvoke("Fall");
		if (spikeManager != null && spikeManager.fallingSpikes.Contains(this)) {
			spikeManager.fallingSpikes.Remove(this);
		}
		target.rigidbody2D.isKinematic = true;
		targetCollider.isTrigger = true;
	}
	#endregion
}
