using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CodeGenerator : MonoBehaviour {
	
	private List<string>[] instructionLines = new List<string>[4] {
		new List<string>(){"$as integer 0 addScore", "¶cs compile"},
		new List<string>(){"$rs integer 0 removeScore", "¶cs compile"},
		new List<string>(){"$at integer 0 addTime", "¶cs compile"},
		new List<string>(){"$rt integer 0 removeTime", "¶cs compile"}
	};
	
	private string[] instructionText = new string[] {
		"AddScore_($as)¶cs",
		"RemoveScore_($rs)¶cs",
		"AddTime_($at)¶cs",
		"RemoveTime_($rt)¶cs"
	};
//	private Dictionary<string, string> instructionLines = new Dictionary<string, string>() {
//		{"$as", "integer 0 addScore"},
//		{"$rs", "integer 0 removeScore"},
//		{"$at", "integer 0 addTime"},
//		{"$rt", "integer 0 removeTime"}
//	}
	
	private int[] values = new int[] { 1, 2, 3, 5, 8, 13, 21, 34 };
	
	private float generatorTimer = 0;
	private float maxRange = 6f;
	private float minRange = 4f;
	
	private float speedUpAt = 12f;
	private float speedUpBy = 0.2f;
	private float speedUpTimer = 0;
	
	public InstructionSpawner[] nodes;
	
	public GameObject[] instructions;
	
	private MTRandom timeRand = new MTRandom();
	private MTRandom nodeRand = new MTRandom();
	
	List<int> nodeList = new List<int>();
	float GetNewTime () {
		return timeRand.Range(2f, 4.8f);
	}
	
	void Start () {
		generatorTimer = GetNewTime();
		if (nodes.Length == 0){
			nodes = GameObject.FindObjectsOfType<InstructionSpawner>();
		}
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
		if (nodes.Length <= 0){
			Debug.LogWarning ("There's no nodes!");
			return;
		}
		int nodeIndex = nodeRand.Range (0, nodes.Length - 1, true);
		while (nodeList.Contains (nodeIndex)){
			nodeIndex = nodeRand.Range (0, nodes.Length - 1, true);
		}
		nodeList.Add (nodeIndex);
		if (nodeList.Count >= nodes.Length - 1){
			Debug.Log("Getting rid of " + nodeList[0]);
			nodeList.RemoveAt (0);
		}
		
		int instIndex = timeRand.Range (0, instructionLines.Length - 1, true);
		
		//Instantiate(instructions[instIndex], nodes[nodeIndex].position, nodes[nodeIndex].rotation);
		
		string instruction = "addTime($at, $bt);";
//		Dictionary<string,string> param = new Dictionary<string, string>();
//		param.Add(instructionLines[instIndex]);
		
		Instruction newDude = InstructionFactoryRuntime.createInstruction(instructionText[instIndex], 1,1, gameObject, instructionLines[instIndex]);
		newDude.transform.position = nodes[nodeIndex].transform.position;
		newDude.AddComponent<InstructionCrawl>();
		
		generatorTimer = GetNewTime();
	}
	
	
}
