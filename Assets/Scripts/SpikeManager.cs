using UnityEngine;

public class SpikeManager : Spawner {
	
	[Min] public int spawnRangeX = 5;
	[Min] public int spawnSpacing = 5;
	[Min] public float spawnMinDelay = 1;
	[Min] public float spawnMaxDelay = 3;
	public Spike[] spikes;
	
	float spawnDelay;
	MTRandom randomGenerator;
	
	public override void Awake() {
		base.Awake();
		
		spikes = new Spike[spawnRangeX * 2 + 1];
		randomGenerator = new MTRandom();
		spawnDelay = randomGenerator.Range(spawnMinDelay, spawnMaxDelay);
		Invoke("SpawnSpike", spawnDelay);
	}
	
	void SpawnSpike() {
		int xPosition = randomGenerator.Range(-spawnRangeX, spawnRangeX);
		spikes[xPosition + spawnRangeX] = Spawn("Spike", new Vector3(transform.position.x + xPosition * spawnSpacing, transform.position.y, 0), Quaternion.identity).GetComponent<Spike>();
		spikes[xPosition + spawnRangeX].index = xPosition + spawnRangeX;
		
		spawnDelay = randomGenerator.Range(spawnMinDelay, spawnMaxDelay);
		Invoke("SpawnSpike", spawnDelay);
	}
}
