using UnityEngine;
using System.Collections;

public class GameObjectUtils  {

	public static void Destroy(GameObject go){
		if(Application.isEditor){
			GameObject.DestroyImmediate(go);
		}else{
			GameObject.Destroy(go);
		}
	}

}
