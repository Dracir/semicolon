using System.Collections.Generic;
using UnityEngine;

public class SpikeManager : Spawner {
	
	[Min] public float spawnMinDelay = 1;
	[Min] public float spawnMaxDelay = 3;
	
	[Separator]
	public List<Spike> fallingSpikes = new List<Spike>();
	public Spike spawningSpike;
	public Spike waitingSpike;
	
	float spawnDelay;
	MTRandom randomGenerator;
	TextCollider2D textCollider2D;
	
	public override void Awake() {
		objectsToSpawn = new [] { References.Prefabs.Spike };
		base.Awake();
		
		textCollider2D = GetComponent<TextCollider2D>();
		if (textCollider2D != null){
			textCollider2D.Text = string.Empty;
		}
		
		randomGenerator = new MTRandom(Random.value.ToString());
		spawnDelay = randomGenerator.Range(spawnMinDelay, spawnMaxDelay);
		Invoke("SpawnSpike", spawnDelay);
	}
	
	void SpawnSpike() {
		if (spawningSpike != null) {
			waitingSpike = spawningSpike;
			waitingSpike.Invoke("Fall", 2.5F);
		}
		
		spawningSpike = Spawn("Spike", transform.position, Quaternion.identity).GetComponent<Spike>();
		spawningSpike.spikeManager = this;
		
		spawnDelay = randomGenerator.Range(spawnMinDelay, spawnMaxDelay);
		Invoke("SpawnSpike", spawnDelay);
	}
}
