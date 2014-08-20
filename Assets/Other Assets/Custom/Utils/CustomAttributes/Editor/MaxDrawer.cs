#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MaxAttribute))]
public class MaxDrawer : CustomPropertyDrawerBase {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
		float max = ((MaxAttribute) attribute).max;
		
		noPrefixLabel = ((CustomAttributeBase) attribute).NoPrefixLabel;
		noFieldLabel = ((CustomAttributeBase) attribute).NoFieldLabel;
		
		if (noPrefixLabel || noFieldLabel) label = GUIContent.none;
		
		EditorGUI.BeginChangeCheck();
		EditorGUI.PropertyField(position, property, label, true);
		if (EditorGUI.EndChangeCheck()){
			switch (property.type)
			{
				default:
					Debug.LogError("MaxAttribute does not support type: " + property.type);
					break;
				case "int":
					property.intValue = (int)Mathf.Min(property.intValue, max);
					break;
				case "float":
					property.floatValue = Mathf.Min(property.floatValue, max);
					break;
				case "double":
					property.floatValue = Mathf.Min(property.floatValue, max);
					break;
				case "Vector2f":
					property.vector2Value = new Vector2(Mathf.Min(property.vector2Value.x, max), Mathf.Min(property.vector2Value.y, max));
					break;
				case "Vector3f":
					property.vector3Value = new Vector3(Mathf.Min(property.vector3Value.x, max), Mathf.Min(property.vector3Value.y, max), Mathf.Min(property.vector3Value.z, max));
					break;
				case "Vector4f":
					property.vector4Value = new Vector4(Mathf.Min(property.vector4Value.x, max), Mathf.Min(property.vector4Value.y, max), Mathf.Min(property.vector4Value.z, max), Mathf.Min(property.vector4Value.w, max));
					break;
				case "ColorRGBA":
					property.colorValue = new Color(Mathf.Min(property.colorValue.r, max), Mathf.Min(property.colorValue.g, max), Mathf.Min(property.colorValue.b, max), Mathf.Min(property.colorValue.a, max));
					break;
			}
		}
	}
}
#endif