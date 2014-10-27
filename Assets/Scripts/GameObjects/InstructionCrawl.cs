using UnityEngine;
using System.Collections;

public class InstructionCrawl : MonoBehaviour {
	
	float crawlSpeed;
	static MTRandom rand = new MTRandom();
	
	float effectivePosition;
	Transform t;
	
	
	void Start () {
		crawlSpeed = rand.Range(0.5f, 1f);
		t = transform;
		effectivePosition = t.position.x;
	}
	
	
	
	void Update () {
		effectivePosition += crawlSpeed * Time.deltaTime;
		
		t.position = new Vector3(Mathf.Floor(effectivePosition), t.position.y, t.position.z);
	}
}
