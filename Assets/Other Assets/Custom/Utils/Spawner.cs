using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	[Button("Spawn", "SpawnButtonPressed", NoPrefixLabel = true)] public bool spawnButton;
	
	public enum SpawnMode {
		All,
		Sequential,
		Random
	}
	
	public SpawnMode spawnMode;
	[Min(-1)] public float spawnInterval = -1;
	public GameObject[] objectsToSpawn;
	
	float counter;
	int currentIndex;
	int[] poolIDs;
	Dictionary<string, int> spawnDict;
	
	public virtual void Start() {
		poolIDs = hObjectPool.Instance.Add(objectsToSpawn);
		BuildSpawnDict();
	}
	
	public virtual void Update() {
		if (objectsToSpawn == null || spawnInterval < 0)
			return;
		
		counter += Time.deltaTime;
		if (counter >= spawnInterval) {
			counter = Mathf.Min(counter - spawnInterval, spawnInterval);
			switch (spawnMode) {
				case SpawnMode.All:
					foreach (int id in poolIDs) {
						Spawn(id);
					}
					break;
				case SpawnMode.Sequential:
					Spawn(poolIDs[currentIndex]);
					currentIndex = (currentIndex + 1) % poolIDs.Length;
					break;
				case SpawnMode.Random:
					Spawn(poolIDs[Random.Range(0, poolIDs.Length)]);
					break;
			}
		}
	}
	
	void BuildSpawnDict() {
		spawnDict = new Dictionary<string, int>();
		for (int i = 0; i < objectsToSpawn.Length; i++) {
			spawnDict[objectsToSpawn[i].name] = poolIDs[i];
		}
	}
	
	public virtual GameObject Spawn(int idToSpawn) {
		return hObjectPool.Instance.Spawn(idToSpawn, transform.position, transform.rotation);
	}
	
	public virtual GameObject Spawn(int idToSpawn, Vector3 position, Quaternion rotation) {
		return hObjectPool.Instance.Spawn(idToSpawn, position, rotation);
	}
	
	public virtual GameObject Spawn(GameObject objectToSpawn) {
		return hObjectPool.Instance.Spawn(objectToSpawn, transform.position, transform.rotation);
	}
	
	public virtual GameObject Spawn(GameObject objectToSpawn, Vector3 position, Quaternion rotation) {
		return hObjectPool.Instance.Spawn(objectToSpawn, position, rotation);
	}
	
	public virtual GameObject Spawn(string nameToSpawn) {
		return Spawn(spawnDict[nameToSpawn]);
	}
	
	public virtual GameObject Spawn(string nameToSpawn, Vector3 position, Quaternion rotation) {
		return Spawn(spawnDict[nameToSpawn], position, rotation);
	}
	
	public virtual GameObject[] Spawn(){
		List<GameObject> gameObjects = new List<GameObject>();
		
		switch (spawnMode) {
			case SpawnMode.All:
				foreach (int id in poolIDs) {
					gameObjects.Add(Spawn(id));
				}
				break;
			case SpawnMode.Sequential:
				gameObjects.Add(Spawn(poolIDs[currentIndex]));
				currentIndex = (currentIndex + 1) % poolIDs.Length;
				break;
			case SpawnMode.Random:
				gameObjects.Add(Spawn(poolIDs[Random.Range(0, poolIDs.Length)]));
				break;
		}
		
		return gameObjects.ToArray();
	}
	
	void SpawnButtonPressed() {
		if (Application.isPlaying) {
			Spawn();
		}
	}
}
