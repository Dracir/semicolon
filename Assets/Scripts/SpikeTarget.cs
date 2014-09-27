using UnityEngine;
using System.Collections;

public class SpikeTarget : MonoBehaviour {

	public Spike parent;
	
	public virtual void OnCollisionEnter2D(Collision2D collision) {
		IDeletable deletable = collision.gameObject.GetComponent(typeof(IDeletable)) as IDeletable;
		if (deletable != null) {
			deletable.Delete(parent);
		}
		parent.Despawn();
	}
}
