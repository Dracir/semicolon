using UnityEngine;
using System.Collections;
using UnityEditor;

public class MapLoaderEditor : EditorWindow {

	public string fileName = "";
	
	void OnGUI(){
		GUILayout.BeginHorizontal ();
		fileName = GUILayout.TextField (fileName);
		if (GUILayout.Button ("Find Map File")) {
			fileName = EditorUtility.OpenFilePanel("Open Map file","maps","txt");
		}
		GUILayout.EndHorizontal ();
		
		//MapLoader.inDebugMode = GUILayout.Toggle(MapLoader.inDebugMode, "Is in debug mod (show Plateform Gizmos)");
		//MapLoader.verbose = GUILayout.Toggle(MapLoader.verbose, "Verbose");
		//MapLoader.loadGameElement = !GUILayout.Toggle(!MapLoader.loadGameElement, "Load only tiles and AI");
		if (fileName.Length == 0) {
			GUI.enabled = false;
			GUILayout.Button("Load Map");
			GUI.enabled = true;
		}else{
			if(GUILayout.Button("Load Map")) {
				MapLoader.loadFromFile(fileName);
			}
		}
	}

	[MenuItem ("FruitsUtils/MapLoader")]
	public static void ShowWindow(){
		EditorWindow.GetWindow(typeof(MapLoaderEditor), true, "MapLoader");
	}
}
