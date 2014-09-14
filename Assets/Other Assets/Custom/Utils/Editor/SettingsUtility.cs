using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SettingsUtility : MonoBehaviour {

	static public bool FileExists(string path){
		return File.Exists(path);
	}
	
	static protected string[] GetSettingsFromFile(string path){
		return File.ReadAllLines(path);
	}
	
	static public Dictionary<string, object> GetSettingsDictFromFile(string path){
		if (File.Exists(path)){
			return ParseSettings(File.ReadAllLines(path));
		}
		return new Dictionary<string, object>();
	}
	
	static public void WriteSettingsToFile(string[] settings, string path){
		File.WriteAllLines (path, settings);
	}
	
	static public void WriteSettingsToFile(List<string> settings, string path){
		File.WriteAllLines (path, settings.ToArray());
	}
	
	static public void WriteSettingsDictToFile(Dictionary<string, object> settingsDict, string path){
		File.WriteAllLines(path, FormatSettings(new List<string>(settingsDict.Keys), new List<object>(settingsDict.Values)));
	}
	
	static public Dictionary<string, object> ParseSettings(string[] settings){
		Dictionary<string, object> settingsDict = new Dictionary<string, object>();
		
		foreach (string setting in settings){
			KeyValuePair<string, object> pair = ParseSetting(setting);
			if (pair.Value != null) settingsDict[pair.Key] = pair.Value;
		}
		return settingsDict;
	}
	
	static public KeyValuePair<string, object> ParseSetting(string setting){
		string[] info = setting.Split(':');
		
		if (info[1] == "Int32"){
			return new KeyValuePair<string, object>(info[0], int.Parse(info[2]));
		}
		else if (info[1] == "Int32[]"){
			List<int> ints = new List<int>();
			for (int i = 2; i < info.Length; i++) ints.Add(int.Parse(info[i]));
			return new KeyValuePair<string, object>(info[0], ints.ToArray());
		}
		
		else if (info[1] == "Single"){
			return new KeyValuePair<string, object>(info[0], float.Parse(info[2]));
		}
		else if (info[1] == "Single[]"){
			List<float> floats = new List<float>();
			for (int i = 2; i < info.Length; i++) floats.Add(float.Parse(info[i]));
			return new KeyValuePair<string, object>(info[0], floats.ToArray());
		}
		
		else if (info[1] == "Double"){
			return new KeyValuePair<string, object>(info[0], double.Parse(info[2]));
		}
		else if (info[1] == "Double[]"){
			List<double> doubles = new List<double>();
			for (int i = 2; i < info.Length; i++) doubles.Add(double.Parse(info[i]));
			return new KeyValuePair<string, object>(info[0], doubles.ToArray());
		}
		
		else if (info[1] == "Boolean"){
			return new KeyValuePair<string, object>(info[0], bool.Parse(info[2]));
		}
		else if (info[1] == "Boolean[]"){
			List<bool> bools = new List<bool>();
			for (int i = 2; i < info.Length; i++) bools.Add(bool.Parse(info[i]));
			return new KeyValuePair<string, object>(info[0], bools.ToArray());
		}
		
		else if (info[1] == "String"){
			return new KeyValuePair<string, object>(info[0], info[2]);
		}
		else if (info[1] == "String[]"){
			List<string> strings = new List<string>();
			for (int i = 2; i < info.Length; i++) strings.Add(info[i]);
			return new KeyValuePair<string, object>(info[0], strings.ToArray());
		}
		
		else if (info[1] == "Vector2"){
			return new KeyValuePair<string, object>(info[0], new Vector2(float.Parse(info[2]), float.Parse(info[3])));
		}
		else if (info[1] == "Vector2[]"){
			List<Vector2> vectors = new List<Vector2>();
			for (int i = 2; i < info.Length; i += 2) vectors.Add(new Vector2(float.Parse(info[i]), float.Parse(info[i + 1])));
			return new KeyValuePair<string, object>(info[0], vectors.ToArray());
		}
		
		else if (info[1] == "Vector3"){
			return new KeyValuePair<string, object>(info[0], new Vector3(float.Parse(info[2]), float.Parse(info[3]), float.Parse(info[4])));
		}
		else if (info[1] == "Vector3[]"){
			List<Vector3> vectors = new List<Vector3>();
			for (int i = 2; i < info.Length; i += 3) vectors.Add(new Vector3(float.Parse(info[i]), float.Parse(info[i + 1]), float.Parse(info[i + 2])));
			return new KeyValuePair<string, object>(info[0], vectors.ToArray());
		}
		
		else if (info[1] == "Vector4"){
			return new KeyValuePair<string, object>(info[0], new Vector4(float.Parse(info[2]), float.Parse(info[3]), float.Parse(info[4]), float.Parse(info[5])));
		}
		else if (info[1] == "Vector4[]"){
			List<Vector4> vectors = new List<Vector4>();
			for (int i = 2; i < info.Length; i += 4) vectors.Add(new Vector4(float.Parse(info[i]), float.Parse(info[i + 1]), float.Parse(info[i + 2]), float.Parse(info[i + 3])));
			return new KeyValuePair<string, object>(info[0], vectors.ToArray());
		}
		
		else if (info[1] == "Quaternion"){
			return new KeyValuePair<string, object>(info[0], new Quaternion(float.Parse(info[2]), float.Parse(info[3]), float.Parse(info[4]), float.Parse(info[5])));
		}
		else if (info[1] == "Quaternion[]"){
			List<Quaternion> quaternions = new List<Quaternion>();
			for (int i = 2; i < info.Length; i += 4) quaternions.Add(new Quaternion(float.Parse(info[i]), float.Parse(info[i + 1]), float.Parse(info[i + 2]), float.Parse(info[i + 3])));
			return new KeyValuePair<string, object>(info[0], quaternions.ToArray());
		}
		
		else if (info[1] == "Color"){
			return new KeyValuePair<string, object>(info[0], new Color(float.Parse(info[2]), float.Parse(info[3]), float.Parse(info[4]), float.Parse(info[5])));
		}
		else if (info[1] == "Color[]"){
			List<Color> colors = new List<Color>();
			for (int i = 2; i < info.Length; i += 4) colors.Add(new Color(float.Parse(info[i]), float.Parse(info[i + 1]), float.Parse(info[i + 2]), float.Parse(info[i + 3])));
			return new KeyValuePair<string, object>(info[0], colors.ToArray());
		}
		
		else if (info[1] == "Rect"){
			return new KeyValuePair<string, object>(info[0], new Rect(float.Parse(info[2]), float.Parse(info[3]), float.Parse(info[4]), float.Parse(info[5])));
		}
		else if (info[1] == "Rect[]"){
			List<Rect> rects = new List<Rect>();
			for (int i = 2; i < info.Length; i += 4) rects.Add(new Rect(float.Parse(info[i]), float.Parse(info[i + 1]), float.Parse(info[i + 2]), float.Parse(info[i + 3])));
			return new KeyValuePair<string, object>(info[0], rects.ToArray());
		}
		
		Debug.LogError("Could not parse setting: " + setting);
		return new KeyValuePair<string, object>();
	}
	
	static public string[] FormatSettings(List<string> keys, List<object> values){
		List<string> settings = new List<string>();
		
		for (int i = 0; i < keys.Count; i++){
			settings.Add(keys[i] + ":" + values[i].GetType().Name + FormatObject(values[i]));
		}
		return settings.ToArray();
	}
	
	static public string[] FormatSettings(string[] keys, object[] values){
		List<string> settings = new List<string>();
		
		for (int i = 0; i < keys.Length; i++){
			settings.Add(keys[i] + ":" + values[i].GetType().Name + FormatObject(values[i]));
		}
		return settings.ToArray();
	}
	
	static public string FormatSetting(string key, object value){
		return key + ":" + value.GetType().Name + FormatObject(value);
	}
	
	static string FormatObject(object toFormat){
		string str = "";
		
		if (toFormat is System.Array){
			foreach (object obj in (ICollection) toFormat) str += FormatObject(obj);
		}
		else if (toFormat is Vector2){
			str += ":" + ((Vector2) toFormat).x + ":" + ((Vector2) toFormat).y;
		}
		else if (toFormat is Vector3){
			str += ":" + ((Vector3) toFormat).x + ":" + ((Vector3) toFormat).y + ":" + ((Vector3) toFormat).z;
		}
		else if (toFormat is Vector4){
			str += ":" + ((Vector4) toFormat).x + ":" + ((Vector4) toFormat).y + ":" + ((Vector4) toFormat).z + ":" + ((Vector4) toFormat).w;
		}
		else if (toFormat is Quaternion){
			str += ":" + ((Quaternion) toFormat).x + ":" + ((Quaternion) toFormat).y + ":" + ((Quaternion) toFormat).z + ":" + ((Quaternion) toFormat).w;
		}
		else if (toFormat is Color){
			str += ":" + ((Color) toFormat).r + ":" + ((Color) toFormat).g + ":" + ((Color) toFormat).b + ":" + ((Color) toFormat).a;
		}
		else if (toFormat is Rect){
			str += ":" + ((Rect) toFormat).x + ":" + ((Rect) toFormat).y + ":" + ((Rect) toFormat).width + ":" + ((Rect) toFormat).height;
		}
		else str += ":" + toFormat.ToString();
		
		return str;
	}
	
}
