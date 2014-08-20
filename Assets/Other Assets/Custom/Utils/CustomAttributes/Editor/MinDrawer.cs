﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinAttribute))]
public class MinDrawer : CustomPropertyDrawerBase {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
		float min = ((MinAttribute) attribute).min;
		
		noPrefixLabel = ((CustomAttributeBase) attribute).NoPrefixLabel;
		noFieldLabel = ((CustomAttributeBase) attribute).NoFieldLabel;
		
		if (noPrefixLabel || noFieldLabel) label = GUIContent.none;
		
		EditorGUI.BeginChangeCheck();
		EditorGUI.PropertyField(position, property, label, true);
		if (EditorGUI.EndChangeCheck()){
			switch (property.type)
			{
				default:
					Debug.LogError("MinAttribute does not support type: " + property.type);
					break;
				case "int":
					property.intValue = (int)Mathf.Max(property.intValue, min);
					break;
				case "float":
					property.floatValue = Mathf.Max(property.floatValue, min);
					break;
				case "double":
					property.floatValue = Mathf.Max(property.floatValue, min);
					break;
				case "Vector2f":
					property.vector2Value = new Vector2(Mathf.Max(property.vector2Value.x, min), Mathf.Max(property.vector2Value.y, min));
					break;
				case "Vector3f":
					property.vector3Value = new Vector3(Mathf.Max(property.vector3Value.x, min), Mathf.Max(property.vector3Value.y, min), Mathf.Max(property.vector3Value.z, min));
					break;
				case "Vector4f":
					property.vector4Value = new Vector4(Mathf.Max(property.vector4Value.x, min), Mathf.Max(property.vector4Value.y, min), Mathf.Max(property.vector4Value.z, min), Mathf.Max(property.vector4Value.w, min));
					break;
				case "ColorRGBA":
					property.colorValue = new Color(Mathf.Max(property.colorValue.r, min), Mathf.Max(property.colorValue.g, min), Mathf.Max(property.colorValue.b, min), Mathf.Max(property.colorValue.a, min));
					break;
			}
		}
	}
}
#endif