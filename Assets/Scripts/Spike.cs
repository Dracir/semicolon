using UnityEngine;

public class Spike : MonoBehaviour
{

	[Min] public float fallSpeed = 3;
	[Min] public float lifeTime = 3;
	[HideInInspector] public int index;
	
	Animator animator;
	Rigidbody2D rigid2D;
	TextCollider2D textCollider2D;
	
	delegate void State();
	State CurrentState;
	float lifeCounter = 0;
	
	void Awake()
	{
		animator = GetComponent<Animator>();
		rigid2D = rigidbody2D;
		textCollider2D = GetComponent<TextCollider2D>();
		CurrentState = Idle;
	}
	
	void OnEnable()
	{
		rigid2D.isKinematic = true;
		textCollider2D.colliderIsTrigger = true;
		textCollider2D.color.a = 0;
		animator.Play("Spike");
		CurrentState = Idle;
	}
	
	void Update()
	{
		CurrentState();
	}
	
	public void Fall()
	{
		rigid2D.isKinematic = false;
		textCollider2D.colliderIsTrigger = false;
		CurrentState = Falling;
	}
	
	public void Despawn()
	{
		rigid2D.isKinematic = true;
		textCollider2D.colliderIsTrigger = true;
		lifeCounter = 0;
		hObjectPool.Instance.Despawn(gameObject);
	}
	
	// States
	void Idle()
	{
		if (References.SpikeManager.spikes[index] != this)
		{
			Invoke("Fall", 3);
			CurrentState = WaitingToFall;
		}
	}
	
	void WaitingToFall()
	{
		
	}
	
	void Falling()
	{
		rigid2D.gravityScale = fallSpeed;
		lifeCounter += Time.deltaTime;
		if (lifeCounter >= lifeTime)
			Despawn();
	}
	
	
	
	void OnCollisionEnter2D(Collision2D collision)
	{
		Despawn();
	}
}
