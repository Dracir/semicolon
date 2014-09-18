﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(PopupSelectorAttribute))]
public class PopupSelectorDrawer : CustomPropertyDrawerBase {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
		position = Begin(position, property, label);
		string arrayName = ((PopupSelectorAttribute) attribute).arrayName;
		string onChangeCallback = ((PopupSelectorAttribute) attribute).onChangeCallback;
		SerializedProperty array = property.serializedObject.FindProperty(arrayName);
		
		int selectedIndex = 0;
		List<string> displayedOptions = new List<string>();
		
		if (array != null){
			for (int i = 0; i < array.arraySize; i++) {
				if (property.objectReferenceValue == array.GetArrayElementAtIndex(i).objectReferenceValue){
					selectedIndex = i;
				}
				if (array.GetArrayElementAtIndex(i).objectReferenceValue != null){
					displayedOptions.Add(string.Format("[{0}] ", i) + array.GetArrayElementAtIndex(i).objectReferenceValue.ToString());
				}
			}
		}
		
		EditorGUI.BeginChangeCheck();
		selectedIndex = Mathf.Clamp(EditorGUI.Popup(position, selectedIndex, displayedOptions.ToArray()), 0, array.arraySize - 1);
		if (array != null && array.arraySize > selectedIndex){
			property.objectReferenceValue = array.GetArrayElementAtIndex(selectedIndex).objectReferenceValue;
		}
		if (EditorGUI.EndChangeCheck()){
		    if (!string.IsNullOrEmpty(onChangeCallback)) ((MonoBehaviour) property.serializedObject.targetObject).Invoke(onChangeCallback, 0);
		}
		
		End(property);
	}
}
#endif