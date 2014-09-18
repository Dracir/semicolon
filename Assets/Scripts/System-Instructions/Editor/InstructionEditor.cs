using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Instruction))]
[System.Serializable]
public class InstructionEditor : Editor  {

	private Instruction instruction;

	[SerializeField]
	private List<bool> showParam = new List<bool>();

	public override void OnInspectorGUI()
	{
		this.instruction = (Instruction)target;

		instruction.instructionText = EditorGUILayout.TextField ("InstructionText", instruction.instructionText);
		if (GUILayout.Button ("Reset")) {
			instruction.reset();		
		}
		showParameters ();

	}


	void showParameters(){
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField("Parameters");
		EditorGUI.indentLevel++;
		int nbParameter = EditorGUILayout.IntField ("NomberOfParameter",instruction.parametersData.Count);
		EditorGUI.indentLevel++;
		fixNbParameter (nbParameter);

		for (int i = 0; i < instruction.parametersData.Count; i++) {
			//Parameter parameter = instruction.parameter[i];
			ParameterData parameterData = instruction.parametersData[i];
			showParam[i] = EditorGUILayout.Foldout(showParam[i], "Param " + i);
			if(showParam[i]){
				EditorGUI.indentLevel++;
				parameterData.datatype = (DataType) EditorGUILayout.EnumPopup(parameterData.datatype);

				
				EditorGUI.indentLevel--;
			}
		}
		EditorGUI.indentLevel--;
		EditorGUI.indentLevel--;

	}

	void fixNbParameter(int nb){
		if (instruction.parametersData.Count < nb) {
			for (int i = 0; i <= nb - instruction.parametersData.Count; i++) {
				instruction.parametersData.Add(new ParameterData());
				showParam.Add(false);
			}
		} else if (instruction.parametersData.Count > nb) {

		} else {
			//Egale on fait rien		
		}

		if (showParam.Count < instruction.parametersData.Count) {
			for (int i = 0; i <= instruction.parametersData.Count - showParam.Count; i++) {
				showParam.Add(false);
			}
		}
	}
}
