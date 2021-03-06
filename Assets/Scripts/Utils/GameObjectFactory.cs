﻿using UnityEngine;
using System.Collections;

public class GameObjectFactory  {


	public static GameObject createGameObject(string name, GameObject parent) {
		return createGameObject (name, parent.transform);
	}

    public static GameObject createGameObject(string name, Transform parent = null) {
        GameObject newObject = new GameObject();
        newObject.name = name;
        if (parent != null) {
            newObject.transform.parent = parent;
        }
        return newObject;
    }

	public static GameObject createCopyGameObject(GameObject original , string name, GameObject parent){
		return createCopyGameObject (original, name, parent.transform);
	}

	public static GameObject createCopyGameObject(GameObject original , string name, Transform parent = null){
		GameObject newObject = (GameObject)GameObject.Instantiate (original);
		newObject.name = name;
		if (parent != null) {
			newObject.transform.parent = parent.transform;		
		}

		return newObject;
	}
	
	public static GameObject createInstancePrefab(string prefabPath, string name, Transform parent){
		GameObject prefabGameObject = (GameObject) Resources.Load(prefabPath);
		return createCopyGameObject(prefabGameObject, name, parent);
	}
}
