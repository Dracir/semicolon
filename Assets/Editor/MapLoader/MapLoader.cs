using UnityEngine;
using System.Collections;

public class MapLoader {
	
	private static MapLoader instance = new MapLoader();
	private MapLoader(){}
	

	private GameObject world;



	public static void loadFromFile(string fileName){
		string text = System.IO.File.ReadAllText(fileName);
		instance.load(text);
	}


	private void load(string mapText) {
		deleteAndCreateEmptyWorld ();
		LevelLoader levelLoader = new LevelLoader ();
		levelLoader.load (mapText, world);
	}


	private void deleteAndCreateEmptyWorld(){
		GameObject.DestroyImmediate(GameObject.Find ("World"));
		this.world = GameObjectFactory.createGameObject ("World");

	}


}
