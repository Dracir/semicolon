using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

//[CustomEditor(typeof(Instruction))]
[System.Serializable]
public class InstructionEditor : Editor  {

	private Instruction instruction;

	public override void OnInspectorGUI()
	{
		this.instruction = (Instruction)target;
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("parametersData"), true);
		serializedObject.ApplyModifiedProperties();

		instruction.instructionText = EditorGUILayout.TextField ("InstructionText", instruction.instructionText);
		if (GUILayout.Button ("Reset")) {
			instruction.reset();
		}
		//showParameters ();

	}

}
