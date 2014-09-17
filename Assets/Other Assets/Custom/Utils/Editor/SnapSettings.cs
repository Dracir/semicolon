using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

static public class SnapSettings {

	static public string fileName = "SnapSettings.asset";
	static public string directory = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/ProjectSettings/";
	
	static Dictionary<string, object> dataDict;
	static Dictionary<string, object> DataDict {
		get { 
			if (dataDict == null) {
				dataDict = new Dictionary<string, object>();
				if (!FileExists(directory + fileName)) {
					CreateDefaultDataFile();
				}
				else {
					DataDict = SaveSystem.DeserializeData(SaveSystem.ReadDataFromFile(directory + fileName));
				}
			}
			return dataDict; 
		}
		set {
			if (dataDict == null) {
				dataDict = new Dictionary<string, object>();
				if (!FileExists(directory + fileName)) {
					CreateDefaultDataFile();
				}
				else {
					DataDict = SaveSystem.DeserializeData(SaveSystem.ReadDataFromFile(directory + fileName));
				}
			}
			dataDict = value;
		}
	}
	
	[PreferenceItem("Snap Settings")]
	static void PreferencesGUI() {
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
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		if (GUILayout.Button("Reset", GUILayout.Width(50)))
			ResetToDefault();
		EditorGUILayout.EndHorizontal();
		
		if (EditorGUI.EndChangeCheck()) {
			UpdateDataDict();
			SceneView.RepaintAll();
		}
	}

	static void UpdateDataDict() {
		SaveSystem.WriteDataToFile(DataDict, directory + fileName);
		DataDict = SaveSystem.DeserializeData(SaveSystem.ReadDataFromFile(directory + fileName));
	}
	
	static void CreateDefaultDataFile() {
		DataDict = new Dictionary<string, object>() {
			{ "MoveX", 1F },
			{ "MoveY", 1F },
			{ "MoveZ", 1F },
			{ "Rotation", 15F },
			{ "Scale", 0.1F },
			{ "GridSize", 10 },
			{ "ShowCubes", true },
			{ "ShowLines", true }
		};
		UpdateDataDict();
	}
	
	static public void ResetToDefault() {
		CreateDefaultDataFile();
	}
	
	static public void CleanUp() {
		foreach (string key in new List<string>(DataDict.Keys)) {
			if (!key.StartsWith("Toggle"))
				continue;
			
			bool stillExists = false;
			foreach (Transform transform in Object.FindObjectsOfType<Transform>()) {
				if (key.Contains(transform.GetInstanceID().ToString())) {
					stillExists = true;
					break;
				}
			}
			if (!stillExists)
				RemoveKey(key);
		}
	}
	
	static public T GetValue<T>(string key) {
		if (HasKey(key)) {
			object value = DataDict[key];
			if (value is T) {
				return (T)value;
			}
		}
		return default(T);
	}
	
	static public object GetValue(string key) {
		return GetValue<object>(key);
	}
	
	static public void SetValue<T>(string key, T value) {
		if (!HasKey(key) || !DataDict[key].Equals(value)) {
			DataDict[key] = value;
			UpdateDataDict();
		}
	}
	
	static public string[] GetAllKeys() {
		return new List<string>(DataDict.Keys).ToArray();
	}
	
	static public object[] GetValues(params string[] keys) {
		List<object> values = new List<object>();
		foreach (string key in keys) {
			values.Add(GetValue(key));
		}
		return values.ToArray();
	}
	
	static public object[] GetAllValues() {
		return new List<object>(DataDict.Values).ToArray();
	}
	
	static public void RemoveKey(string key) {
		if (HasKey(key)) {
			DataDict.Remove(key);
			UpdateDataDict();
		}
	}
	
	static public void RemoveKeys(params string[] keys) {
		foreach (string key in keys) {
			RemoveKey(key);
		}
	}
	
	static public void RemoveAllKeys() {
		DataDict = new Dictionary<string, object>();
		UpdateDataDict();
	}
	
	static public bool HasKey(string key) {
		return DataDict.ContainsKey(key);
	}
	
	static public bool FileExists() {
		return File.Exists(directory + fileName);
	}

	static public bool FileExists(string path) {
		return File.Exists(path);
	}
}
