#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SingleLineVectorAttribute))]
public class SingleLineVectorDrawer : CustomPropertyDrawerBase {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		position = Begin(position, property, label);
		
		float x = property.FindPropertyRelative("x").floatValue;
		float y = property.FindPropertyRelative("y").floatValue;
		float z = 0;
		float w = 0;
		string xName = ((SingleLineVectorAttribute) attribute).x;
		string yName = ((SingleLineVectorAttribute) attribute).y;
		string zName = ((SingleLineVectorAttribute) attribute).z;
		string wName = ((SingleLineVectorAttribute) attribute).w;
		
		int nbOfFields = 2;
		if (property.FindPropertyRelative("z") != null){
			nbOfFields += 1;
			z = property.FindPropertyRelative("z").floatValue;
		}
		if (property.FindPropertyRelative("w") != null){
			nbOfFields += 1;
			w = property.FindPropertyRelative("w").floatValue;
		}
		
		float width = position.width;
		float maxLabelWidth = width / (nbOfFields * 2) - 3;
		EditorGUI.indentLevel = 0;
		
		position.width /= nbOfFields;
		
		if (noFieldLabel) x = EditorGUI.FloatField(position, x);
		else{
			EditorGUIUtility.labelWidth = Mathf.Min(xName.GetWidth(EditorStyles.standardFont) + 8, maxLabelWidth);
			x = EditorGUI.FloatField(position, xName, x);
		}
		property.FindPropertyRelative("x").floatValue = x;
		
		position.x += position.width;
		if (noFieldLabel) y = EditorGUI.FloatField(position, y);
		else{
			EditorGUIUtility.labelWidth = Mathf.Min(yName.GetWidth(EditorStyles.standardFont) + 8, maxLabelWidth);
			y = EditorGUI.FloatField(position, yName, y);
		}
		property.FindPropertyRelative("y").floatValue = y;
		
		if (property.FindPropertyRelative("z") != null){
			position.x += position.width;
			if (noFieldLabel) z = EditorGUI.FloatField(position, z);
			else{
				EditorGUIUtility.labelWidth = Mathf.Min(zName.GetWidth(EditorStyles.standardFont) + 8, maxLabelWidth);
				z = EditorGUI.FloatField(position, zName, z);
			}
			property.FindPropertyRelative("z").floatValue = z;
		}
		if (property.FindPropertyRelative("w") != null){
			position.x += position.width;
			if (noFieldLabel) w = EditorGUI.FloatField(position, w);
			else{
				EditorGUIUtility.labelWidth = Mathf.Min(wName.GetWidth(EditorStyles.standardFont) + 8, maxLabelWidth);
				w = EditorGUI.FloatField(position, wName, w);
			}
			property.FindPropertyRelative("w").floatValue = w;
		}
		
		End(property);
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return EditorGUIUtility.singleLineHeight;
	}
}
#endif