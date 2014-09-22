using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CodeGenerator : MonoBehaviour {
	
	private float generatorTimer = 0;
	private float maxRange = 6f;
	private float minRange = 4f;
	
	private float speedUpAt = 12f;
	private float speedUpBy = 0.2f;
	private float speedUpTimer = 0;
	
	public Transform[] nodes;
	
	public GameObject[] instructions;
	
	private MTRandom timeRand = new MTRandom();
	private MTRandom nodeRand = new MTRandom();
	
	List<int> nodeList = new List<int>();
	float GetNewTime () {
		return timeRand.Range(2f, 4.8f);
	}
	
	void Start () {
		generatorTimer = GetNewTime();
	}
	
	// Update is called once per frame
	void Update () {
		generatorTimer -= Time.deltaTime;
		if (generatorTimer < 0){
			Spawn();
		}
		
		speedUpTimer += Time.deltaTime;
		if (speedUpTimer > speedUpAt){
			minRange -= speedUpBy;
			maxRange -= speedUpBy;
			speedUpTimer = 0;
		}
	}
	
	void Spawn () {
		int nodeIndex = nodeRand.Range (0, nodes.Length - 1);
		while (nodeList.Contains (nodeIndex)){
			nodeIndex = nodeRand.Range (0, nodes.Length - 1);
		}
		nodeList.Add (nodeIndex);
		if (nodeList.Count >= nodes.Length - 1){
			Debug.Log("Getting rid of " + nodeList[0]);
			nodeList.RemoveAt (0);
		}
		
		int instIndex = timeRand.Range (0, instructions.Length - 1);
		
		Instantiate(instructions[instIndex], nodes[nodeIndex].position, nodes[nodeIndex].rotation);
		generatorTimer = GetNewTime();
	}
}
