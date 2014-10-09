using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour {

	public string fileName = "default.sav";
	public string directory = Application.dataPath + "/";
	
	Dictionary<string, object> dataDict;
	Dictionary<string, object> DataDict {
		get { 
			if (dataDict == null) {
				dataDict = new Dictionary<string, object>();
				if (!FileExists(directory + fileName)) {
					UpdateDataDict();
				}
				else {
					DataDict = DeserializeData(ReadDataFromFile(directory + fileName));
				}
			}
			return dataDict; 
		}
		set {
			if (dataDict == null) {
				dataDict = new Dictionary<string, object>();
				if (!FileExists(directory + fileName)) {
					UpdateDataDict();
				}
				else {
					DataDict = DeserializeData(ReadDataFromFile(directory + fileName));
				}
			}
			dataDict = value;
		}
	}
	
	static char KeyValueSeparator = '§';
	static char ObjectSeparator = '¶';
	static SaveSystem instance;
	public static SaveSystem Instance {
		get { 
			if (instance == null) {
				instance = FindObjectOfType<SaveSystem>();
			}
			return instance;
		}
	}
	
	void UpdateDataDict() {
		WriteDataToFile(DataDict, directory + fileName);
		DataDict = DeserializeData(ReadDataFromFile(directory + fileName));
	}
	
	public T GetValue<T>(string key) {
		if (HasKey(key)) {
			object value = DataDict[key];
			if (value is T) {
				return (T)value;
			}
		}
		return default(T);
	}
	
	public object GetValue(string key) {
		return GetValue<object>(key);
	}
	
	public void SetValue<T>(string key, T value) {
		DataDict[key] = value;
		UpdateDataDict();
	}
	
	public string[] GetAllKeys() {
		return new List<string>(DataDict.Keys).ToArray();
	}
	
	public object[] GetValues(params string[] keys) {
		List<object> values = new List<object>();
		foreach (string key in keys) {
			values.Add(GetValue(key));
		}
		return values.ToArray();
	}
	
	public object[] GetAllValues() {
		return new List<object>(DataDict.Values).ToArray();
	}
	
	public void RemoveKey(string key) {
		if (HasKey(key)) {
			DataDict.Remove(key);
			UpdateDataDict();
		}
	}
	
	public void RemoveKeys(params string[] keys) {
		foreach (string key in keys) {
			RemoveKey(key);
		}
	}
	
	public void RemoveAllKeys() {
		DataDict = new Dictionary<string, object>();
		UpdateDataDict();
	}
	
	public bool HasKey(string key) {
		return DataDict.ContainsKey(key);
	}
	
	public bool FileExists() {
		return File.Exists(directory + fileName);
	}
	
	public static bool FileExists(string path) {
		return File.Exists(path);
	}
	
	public static string[] ReadDataFromFile(string path) {
		List<string> data = new List<string>();
		string currentData = "";
		
		foreach (string line in File.ReadAllLines(path)) {
			if (line == ObjectSeparator.ToString()) {
				data.Add(currentData);
				currentData = "";
			}
			else {
				currentData += line + '\n';
			}
		}
		return data.ToArray();
	}
	
	public static void WriteDataToFile(string[] data, string path) {
		File.WriteAllLines(path, data);
	}
	
	public static void WriteDataToFile(List<string> data, string path) {
		File.WriteAllLines(path, data.ToArray());
	}
	
	public static void WriteDataToFile(Dictionary<string, object> dataDict, string path) {
		File.WriteAllLines(path, SerializeData(dataDict));
	}
	
	public static string[] SerializeData(Dictionary<string, object> dataDict) {
		List<string> keys = new List<string>(dataDict.Keys);
		string[] data = new string[keys.Count];
		
		for (int i = 0; i < keys.Count; i++) {
			data[i] = dataDict[keys[i]].GetType().AssemblyQualifiedName + KeyValueSeparator + keys[i] + KeyValueSeparator + dataDict[keys[i]].SerializeXml() + '\n' + ObjectSeparator;
		}
		return data;
	}
	
	public static Dictionary<string, object> DeserializeData(string[] data) {
		Dictionary<string, object> dataDict = new Dictionary<string, object>();
		
		for (int i = 0; i < data.Length; i++) {
			string[] typeKeyValue = data[i].Split(KeyValueSeparator);
			dataDict[typeKeyValue[1]] = XmlSupport.DeserializeXml(typeKeyValue[2], Type.GetType(typeKeyValue[0]));
		}
		return dataDict;
	}
}
