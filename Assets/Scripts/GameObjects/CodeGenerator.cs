using UnityEngine;
using System.Collections;

public class CodeGenerator : MonoBehaviour {
	
	private float generatorTimer = 0;
	
	public Transform[] nodes;
	
	public GameObject[] instructions;
	
	private MTRandom rand = new MTRandom();
	
	float GetNewTime () {
		return rand.Range(2f, 4.8f);
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
	}
	
	void Spawn () {
		int nodeIndex = rand.Range (0, nodes.Length - 1);
		int instIndex = rand.Range (0, instructions.Length - 1);
		
		Instantiate(instructions[instIndex], nodes[nodeIndex].position, nodes[nodeIndex].rotation);
		generatorTimer = GetNewTime();
	}
}
