using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SCObjectAttribute))]
public class SCObjectsDrawer : CustomPropertyDrawerBase {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
		drawPrefixLabel = true;
		Debug.Log ("KEVIN ENCORE");
		position = Begin(position, property, label);
		position = AttributeUtility.BeginIndentation(position);
		Parameter p = (Parameter)property.serializedObject.targetObject;

		if (p.value is SCBoolean) {
			SCBoolean boolean = (SCBoolean)p.value;
			boolean.value = EditorGUI.Toggle (position, boolean.value);

		} else {
			EditorGUI.LabelField(position, "kevin");
		}
		p.refresh();
		End(property);
	}
}
