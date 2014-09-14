#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextMesh))]
public class TextMeshEditor : Editor {
	
    public override void OnInspectorGUI(){
		TextMesh textMesh = (TextMesh) target;
        textMesh.text = EditorGUILayout.TextArea(textMesh.text);
        DrawDefaultInspector();
    }
}
#endif