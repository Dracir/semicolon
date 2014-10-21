using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public static class UnitySerializerExtensions {

	public static string SerializeXml<T, U>(this IDictionary<T, U> dictionary) {
		T[] keys;
		U[] values;
		dictionary.GetOrderedKeysValues(out keys, out values);
		return keys.SerializeXml() + '£' + values.SerializeXml();
	}
	
	public static Dictionary<T, U> DeserializeXml<T, U>(this string data) {
		string[] keysValuesData = data.Split('£');
		T[] keys = keysValuesData[0].DeserializeXml<T[]>();
		U[] values = keysValuesData[1].DeserializeXml<U[]>();
		Dictionary<T, U> dictionary = new Dictionary<T, U>();
		
		for (int i = 0; i < keys.Length; i++) {
			dictionary[keys[i]] = values[i];
		}
		return dictionary;
	}
}
