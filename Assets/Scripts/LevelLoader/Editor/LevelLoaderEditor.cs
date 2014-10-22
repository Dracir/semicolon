using UnityEngine;
using System.Collections;
using UnityEditor;

public class LevelLoaderEditor : EditorWindow {

	public string fileName = "";
	
	void OnGUI(){
		GUILayout.BeginHorizontal ();
		fileName = GUILayout.TextField (fileName);
		if (GUILayout.Button ("Find Level File")) {
			fileName = EditorUtility.OpenFilePanel("Open Level File","levels","txt");
		}
		GUILayout.EndHorizontal ();

		LevelLoaderMain.arcade = EditorGUILayout.Toggle("ArcadeGame", LevelLoaderMain.arcade);
		
		if (fileName.Length == 0) {
			GUI.enabled = false;
			GUILayout.Button("Load Level");
			GUI.enabled = true;
		}else{
			if(GUILayout.Button("Load Level")) {
				LevelLoaderMain.loadFromFile(fileName);
			}
		}
	}

	[MenuItem ("FruitsUtils/Level Loader")]
	public static void ShowWindow(){
		EditorWindow.GetWindow(typeof(LevelLoaderEditor), true, "Level Loader");
	}
}
