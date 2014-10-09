#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ClampAttribute))]
public class ClampDrawer : CustomPropertyDrawerBase {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
		drawPrefixLabel = false;
		position = Begin(position, property, label);
		float min = ((ClampAttribute) attribute).min;
		float max = ((ClampAttribute) attribute).max;
		
		EditorGUI.BeginChangeCheck();
		EditorGUI.PropertyField(position, property, label, true);
		if (EditorGUI.EndChangeCheck()){
			switch (property.type)
			{
				default:
					Debug.LogError("MinAttribute does not support type: " + property.type);
					break;
				case "int":
					property.intValue = (int)Mathf.Clamp(property.intValue, min, max);
					break;
				case "float":
					property.floatValue = Mathf.Clamp(property.floatValue, min, max);
					break;
				case "double":
					property.floatValue = Mathf.Clamp(property.floatValue, min, max);
					break;
				case "Vector2f":
					property.vector2Value = new Vector2(Mathf.Clamp(property.vector2Value.x, min, max), Mathf.Clamp(property.vector2Value.y, min, max));
					break;
				case "Vector3f":
					property.vector3Value = new Vector3(Mathf.Clamp(property.vector3Value.x, min, max), Mathf.Clamp(property.vector3Value.y, min, max), Mathf.Clamp(property.vector3Value.z, min, max));
					break;
				case "Vector4f":
					property.vector4Value = new Vector4(Mathf.Clamp(property.vector4Value.x, min, max), Mathf.Clamp(property.vector4Value.y, min, max), Mathf.Clamp(property.vector4Value.z, min, max), Mathf.Clamp(property.vector4Value.w, min, max));
					break;
				case "Quaternion":
					property.quaternionValue = new Quaternion(Mathf.Clamp(property.quaternionValue.x, min, max), Mathf.Clamp(property.quaternionValue.y, min, max), Mathf.Clamp(property.quaternionValue.z, min, max), Mathf.Clamp(property.quaternionValue.w, min, max));
					break;
				case "ColorRGBA":
					property.colorValue = new Color(Mathf.Clamp(property.colorValue.r, min, max), Mathf.Clamp(property.colorValue.g, min, max), Mathf.Clamp(property.colorValue.b, min, max), Mathf.Clamp(property.colorValue.a, min, max));
					break;
				case "Rectf":
					property.rectValue = new Rect(Mathf.Clamp(property.rectValue.x, min, max), Mathf.Clamp(property.rectValue.y, min, max), Mathf.Clamp(property.rectValue.width, min, max), Mathf.Clamp(property.rectValue.height, min, max));
					break;
				case "AABB":
					property.boundsValue = new Bounds(new Vector3(Mathf.Clamp(property.boundsValue.center.x, min, max), Mathf.Clamp(property.boundsValue.center.y, min, max), Mathf.Clamp(property.boundsValue.center.z, min, max)), new Vector3(Mathf.Clamp(property.boundsValue.size.x, min, max), Mathf.Clamp(property.boundsValue.size.y, min, max), Mathf.Clamp(property.boundsValue.size.z, min, max)));
					break;
				case "AnimationCurve":
					property.animationCurveValue = new AnimationCurve(property.animationCurveValue.Clamp(min, max, min, max).keys);
					break;
			}
		
		}
		End(property);
	}
}
#endif