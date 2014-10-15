using UnityEngine;
using System.Collections;

public class GameText : MonoBehaviour, IDeletable {
	
	public bool invulnerable = false;
	
	bool IDeletable.Delete(Spike spike){
		if(!invulnerable){
			int spikeX 	= (int) spike.transform.position.x;
			int parentX	= (int) this.transform.position.x;
			int x		= spikeX - parentX;
			EffectManager.AddGameEffect(new SplitTextEffect(this.GetComponent<TextCollider2D>(),x));
		}
		return !invulnerable;
		
	}
}
