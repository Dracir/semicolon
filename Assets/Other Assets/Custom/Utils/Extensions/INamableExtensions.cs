using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

public static class INamableExtensions {
	
	public static string GetUniqueName<T>(this INamable namable, string newName, IList<T> array) where T : INamable {
		int suffix = 0;
		bool uniqueName = false;
		string currentName = "";
		string oldName = namable.Name;
		
		while (!uniqueName) {
			uniqueName = true;
			currentName = newName;
			if (suffix > 0) currentName += suffix.ToString();
			
			foreach (INamable element in array) {
				if (element.Name == currentName && element.Name != oldName) {
					uniqueName = false;
					break;
				}
			}
			suffix += 1;
		}
		return currentName;
	}
	
	public static string GetUniqueName<T>(this INamable namable, string newName, string emptyName, IList<T> array) where T : INamable {
		string name = namable.GetUniqueName(newName, array);
		if (string.IsNullOrEmpty(newName)) {
			name = namable.GetUniqueName(emptyName, array);
		}
		return name;
	}
	
	public static void SetUniqueName<T>(this INamable namable, string newName, string emptyName, IList<T> array) where T : INamable {
		namable.Name = namable.GetUniqueName(newName, emptyName, array);
	}
	
	public static void SetUniqueName<T>(this INamable namable, string newName, IList<T> array) where T : INamable {
		namable.Name = namable.GetUniqueName(newName, array);
	}
}