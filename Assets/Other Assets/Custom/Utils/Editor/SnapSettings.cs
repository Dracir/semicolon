using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SnapSettings {

	static public string fileName = "SnapSettings.asset";
	static public string directory = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/ProjectSettings/";
	
	static Dictionary<string, object> settingsDict;
	static Dictionary<string, object> SettingsDict {
		get { 
			if (settingsDict == null){
				if (!SettingsUtility.FileExists(directory + fileName)) CreateDefaultSettingsFile();
				settingsDict = SettingsUtility.GetSettingsDictFromFile(directory + fileName);
			}
			return settingsDict; 
		}
		set {
			if (settingsDict == null){
				if (!SettingsUtility.FileExists(directory + fileName)) CreateDefaultSettingsFile();
				settingsDict = SettingsUtility.GetSettingsDictFromFile(directory + fileName);
			}
			settingsDict = value;
		}
	}
	
	[PreferenceItem("Snap Settings")]
	static void PreferencesGUI(){
		EditorGUI.BeginChangeCheck();
		
		EditorGUILayout.Space();
		SetValue("MoveX", Mathf.Max(EditorGUILayout.FloatField("Move X", GetValue<float>("MoveX")), 0.001F));
		SetValue("MoveY", Mathf.Max(EditorGUILayout.FloatField("Move Y", GetValue<float>("MoveY")), 0.001F));
		SetValue("MoveZ", Mathf.Max(EditorGUILayout.FloatField("Move Z", GetValue<float>("MoveZ")), 0.001F));
		SetValue("Rotation", Mathf.Max(EditorGUILayout.FloatField("Rotation", GetValue<float>("Rotation")), 0.001F));
		SetValue("Scale", Mathf.Max(EditorGUILayout.FloatField("Scale", GetValue<float>("Scale")), 0.001F));
		SetValue("GridSize", EditorGUILayout.IntSlider("Grid Size", GetValue<int>("GridSize"), 0, 100));
		SetValue("ShowCubes", EditorGUILayout.Toggle("Show Grid Cubes", GetValue<bool>("ShowCubes")));
		SetValue("ShowLines", EditorGUILayout.Toggle("Show Grid Lines", GetValue<bool>("ShowLines")));
		SetValue("FadeWithDistance", EditorGUILayout.Toggle("Fade With Distance", GetValue<bool>("FadeWithDistance")));
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		if (GUILayout.Button("Reset", GUILayout.Width(50))) ResetToDefault();
		EditorGUILayout.EndHorizontal();
		
		if (EditorGUI.EndChangeCheck())	SceneView.RepaintAll();
	}

	static public T GetValue<T>(string key){
		if (HasKey(key)) if (SettingsDict[key].GetType() == typeof(T)) return (T) SettingsDict[key];
		return default(T);
	}
	
	static public void SetValue<T>(string key, T value){
		SettingsDict[key] = value;
		SettingsUtility.WriteSettingsDictToFile(SettingsDict, directory + fileName);
	}
	
	static public KeyValuePair<string, object>[] GetKeyValuePairs(){
		return new List<KeyValuePair<string, object>>(SettingsDict).ToArray();
	}
	
	static public string[] GetKeys(){
		return new List<string>(SettingsDict.Keys).ToArray();
	}
	
	static public object[] GetValues(){
		return new List<object>(SettingsDict.Values).ToArray();
	}
	
	static public bool HasKey(string key){
		return SettingsDict.ContainsKey(key);
	}
	
	static public void DeleteKey(string key){
		if (HasKey(key)){
			SettingsDict.Remove(key);
			SettingsUtility.WriteSettingsDictToFile(SettingsDict, directory + fileName);
		}
	}
	
	static public void DeleteKeys(){
		SettingsDict = new Dictionary<string, object>();
		SettingsUtility.WriteSettingsDictToFile(SettingsDict, directory + fileName);
	}
	
	static public void ResetToDefault(){
		CreateDefaultSettingsFile();
		SettingsDict = SettingsUtility.GetSettingsDictFromFile(directory + fileName);
	}
	
	static public void CleanUp(){
		foreach (string key in new List<string>(SettingsDict.Keys)){
			if (!key.StartsWith("Toggle")) continue;
			
			bool stillExists = false;
			foreach (Transform transform in Transform.FindObjectsOfType<Transform>()){
				if (key.Contains(transform.GetInstanceID().ToString())){
					stillExists = true;
					break;
				}
			}
			if (!stillExists) SettingsDict.Remove(key);
		}
		SettingsUtility.WriteSettingsDictToFile(SettingsDict, directory + fileName);
	}

	static void CreateDefaultSettingsFile(){
		List<string> settings = new List<string> ();
		settings.Add(SettingsUtility.FormatSetting("MoveX", 1F));
		settings.Add(SettingsUtility.FormatSetting("MoveY", 1F));
		settings.Add(SettingsUtility.FormatSetting("MoveZ", 1F));
		settings.Add(SettingsUtility.FormatSetting("Rotation", 15F));
		settings.Add(SettingsUtility.FormatSetting("Scale", 0.1F));
		settings.Add(SettingsUtility.FormatSetting("GridSize", 10));
		settings.Add(SettingsUtility.FormatSetting("ShowCubes", true));
		settings.Add(SettingsUtility.FormatSetting("ShowLines", true));
		settings.Add(SettingsUtility.FormatSetting("FadeWithDistance", false));
		SettingsUtility.WriteSettingsToFile(settings.ToArray(), directory + fileName);
	}
}
