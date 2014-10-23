using UnityEngine;
using System.Collections;

public class LevelLoaderMain {
	
	private static LevelLoaderMain instance = new LevelLoaderMain();
	private LevelLoaderMain(){}

	private GameObject world;
	public static bool arcade = false;
	
	public static void loadFromFile(string fileName){
		string text = System.IO.File.ReadAllText(fileName);
		instance.load(text);
	}


	private void load(string mapText) {
		deleteAndCreateEmptyWorld ();
		checkThings();
		FileToLevelLoader levelLoader = new FileToLevelLoader ();
		levelLoader.load (mapText, world);
	}


	private void deleteAndCreateEmptyWorld(){
		Object.DestroyImmediate(GameObject.Find ("World"));
		this.world = GameObjectFactory.createGameObject ("World");
		this.world.AddComponent<LevelReferences>();
	}


	void checkThings(){
		createIfShouldExistElseRemove(References.Prefabs.ArcadeManager, "ArcadeManager",arcade);
		createIfShouldExistElseRemove(References.Prefabs.ArcadeCanevas, "ArcadeCanevas",arcade);
	}
	
	void createIfShouldExistElseRemove(GameObject prefabObject, string name, bool shouldExist){
		GameObject go = GameObject.Find(name);
		if(shouldExist){
			if(go == null){
				GameObjectFactory.createCopyGameObject(prefabObject, name);
			}
		}else{
			if(go != null){
				go.Remove();
			}
		}
	}
}
