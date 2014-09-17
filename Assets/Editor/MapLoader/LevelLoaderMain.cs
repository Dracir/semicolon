using UnityEngine;
using System.Collections;

public class LevelLoaderMain {
	
	private static LevelLoaderMain instance = new LevelLoaderMain();
	private LevelLoaderMain(){}
	

	private GameObject world;



	public static void loadFromFile(string fileName){
		string text = System.IO.File.ReadAllText(fileName);
		instance.load(text);
	}


	private void load(string mapText) {
		deleteAndCreateEmptyWorld ();
		FileToLevelLoader levelLoader = new FileToLevelLoader ();
		levelLoader.load (mapText, world);
	}


	private void deleteAndCreateEmptyWorld(){
		GameObject.DestroyImmediate(GameObject.Find ("World"));
		this.world = GameObjectFactory.createGameObject ("World");

	}


}
