using UnityEngine;
using System.Collections;

public class InstructionCrawl : MonoBehaviour {
	
	float crawlSpeed;
	static MTRandom rand = new MTRandom();
	
	float effectivePosition;
	Transform t;
	// Use this for initialization
	void Start () {
		crawlSpeed = rand.Range(3, 5);
		t = transform;
		effectivePosition = t.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		effectivePosition += crawlSpeed * Time.deltaTime;
		
		t.position = new Vector3(Mathf.Floor(effectivePosition), t.position.y, t.position.z);
	}
}
