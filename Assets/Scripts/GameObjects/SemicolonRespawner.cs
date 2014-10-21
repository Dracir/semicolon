using UnityEngine;
using System.Collections;

public class SemicolonRespawner : MonoBehaviour {
	
	private CompileSemiColon[] possibles;
	private int index = 0;
	
	// Use this for initialization
	void Start () {
		possibles = GameObject.FindObjectsOfType<CompileSemiColon>();
		Debug.Log("possibles" + possibles.Length);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.F11)){
			Dead();
		}
	}
	
	public void Dead () {
		Transform replacement = possibles[index].transform;
		transform.position = replacement.position;
		Destroy(replacement.gameObject);
		index ++;
	}
}
