using UnityEngine;
using System.Collections;


public class CompileSpot : MonoBehaviour {

	private Instruction parentInstruction;
	void Start () {
		this.parentInstruction = this.GetComponentInParent<Instruction>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		Player player = other.GetComponent<Player>();
		if(player){
			Vector3 v = this.transform.position;
			Vector3 moveToVector = new Vector3(v.x + 0.5f, v.y - 1.66f/2,0f);
			Semicolon.instance.GetComponent<MoveToMagical>().startMoveTo(parentInstruction, moveToVector,0.5f,1);
		}else if(other.GetComponent<CompileSemiColon>() != null){
			parentInstruction.compile();
		}
		
		
    }
	
	 void OnTriggerExit2D(Collider2D other) {
    }
	
}
