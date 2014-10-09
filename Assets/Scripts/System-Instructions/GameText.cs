using UnityEngine;
using System.Collections;

public class GameText : MonoBehaviour, IDeletable {
	
	void IDeletable.Delete(Spike spike){
		int spikeX 	= (int) spike.transform.position.x;
		int parentX	= (int) this.transform.position.x;
		int x		= spikeX - parentX;
		EffectManager.AddGameEffect(new SplitTextEffect(this.GetComponent<TextCollider2D>(),x));
		//EffectManager.AddGameEffect(new WaveDeleteTextEffect(this,1,x));
	}
}
