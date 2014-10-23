using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class LevelReferences : MonoBehaviour {

	public static LevelReferences instance;
	
	public List<SpikeManager> spikeManagers;
	
	
	void Awake(){
		LevelReferences.instance = this;
	}
	
}
