using UnityEngine;
using System.Collections;

public class SpikeTarget : MonoBehaviour {

	public Spike parent;
	
	public virtual void OnCollisionEnter2D(Collision2D collision) {
		IDeletable deletable = collision.gameObject.GetComponent(typeof(IDeletable)) as IDeletable;
		if (deletable != null) {
			bool deleted = deletable.Delete(parent);
			if(deleted){
				parent.Despawn();	
			}
		}else{
			parent.Despawn();
		}
		
	}
}
