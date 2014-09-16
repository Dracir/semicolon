using UnityEngine;
using System.Collections;

public class PDPlayerExample : MonoBehaviour {

	void OnGUI(){
		if (GUILayout.Button(" Play ")){
			PDPlayer.Play("Wind");
		}
		
		if (GUILayout.Button(" Stop ")){
			PDPlayer.Stop("Wind");
		}
	}
}
