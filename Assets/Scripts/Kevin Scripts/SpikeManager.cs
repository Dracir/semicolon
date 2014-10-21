using System.Collections.Generic;
using UnityEngine;
using Magicolo.GeneralTools;

public class SpikeManager : Spawner {
	
	[Min] public float spawnMinDelay = 1;
	[Min] public float spawnMaxDelay = 3;
	
	[Separator]
	public List<Spike> fallingSpikes = new List<Spike>();
	public Spike spawningSpike;
	public Spike waitingSpike;
	
	public bool autoSpawn = false;
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
		if(autoSpawn){
			spawnDelay = randomGenerator.Range(spawnMinDelay, spawnMaxDelay);
			SpawnSpikeIn(spawnDelay);
		}
	}
	
	public void SpawnSpikeIn(float time){
		Invoke("SpawnSpike", time);
	}
	
	public void SpawnSpike() {
		if (spawningSpike != null) {
			waitingSpike = spawningSpike;
			waitingSpike.Invoke("Fall", 1.5F);
		}
		
		spawningSpike = Spawn("Spike", transform.position, Quaternion.identity).GetComponent<Spike>();
		spawningSpike.transform.parent = transform;
		spawningSpike.spikeManager = this;
		
		if(autoSpawn){
			spawnDelay = randomGenerator.Range(spawnMinDelay, spawnMaxDelay);
			SpawnSpikeIn(spawnDelay);
		}
	}
	
	
}
