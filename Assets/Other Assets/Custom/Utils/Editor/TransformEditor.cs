#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Transform)), CanEditMultipleObjects]
public class TransformEditor : Editor {

	public override void OnInspectorGUI(){
		Transform transform = (Transform) target;
		
		EditorGUI.BeginChangeCheck();
		transform.localPosition = DrawSingleLineVectorWithButton(transform.localPosition, ". P  .", Vector3.zero);
		transform.localEulerAngles = DrawSingleLineVectorWithButton(transform.localEulerAngles, ". R  .", Vector3.zero);
		transform.localScale = DrawSingleLineVectorWithButton(transform.localScale, ". S  .", Vector3.one);
		if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
	}
	
	public Vector3 DrawSingleLineVectorWithButton(Vector3 vector, string buttonLabel, Vector3 resetValue) {
		float labelWidth = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 15;
		EditorGUILayout.BeginHorizontal();
		
		if (GUILayout.Button(buttonLabel, EditorStyles.miniButton, GUILayout.Width(20))){
			vector.x = resetValue.x;
			vector.y = resetValue.y;
			vector.z = resetValue.z;
		}
		vector.x = EditorGUILayout.FloatField("X", vector.x, GUILayout.MinWidth(30f));
		vector.y = EditorGUILayout.FloatField("Y", vector.y, GUILayout.MinWidth(30f));
		vector.z = EditorGUILayout.FloatField("Z", vector.z, GUILayout.MinWidth(30f));
		vector.x = float.IsNaN(vector.x) ? 0f : vector.x;
		vector.y = float.IsNaN(vector.y) ? 0f : vector.y;
		vector.z = float.IsNaN(vector.z) ? 0f : vector.z;
		
		EditorGUILayout.EndHorizontal();
		EditorGUIUtility.labelWidth = labelWidth;
		
		return vector;
	}
}
#endif