#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MaxAttribute))]
public class MaxDrawer : CustomPropertyDrawerBase {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
		drawPrefixLabel = false;
		position = Begin(position, property, label);
		float max = ((MaxAttribute) attribute).max;
		
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
				case "Quaternion":
					property.quaternionValue = new Quaternion(Mathf.Min(property.quaternionValue.x, max), Mathf.Min(property.quaternionValue.y, max), Mathf.Min(property.quaternionValue.z, max), Mathf.Min(property.quaternionValue.w, max));
					break;
				case "ColorRGBA":
					property.colorValue = new Color(Mathf.Min(property.colorValue.r, max), Mathf.Min(property.colorValue.g, max), Mathf.Min(property.colorValue.b, max), Mathf.Min(property.colorValue.a, max));
					break;
				case "Rectf":
					property.rectValue = new Rect(Mathf.Min(property.rectValue.x, max), Mathf.Min(property.rectValue.y, max), Mathf.Min(property.rectValue.width, max), Mathf.Min(property.rectValue.height, max));
					break;
				case "AABB":
					property.boundsValue = new Bounds(new Vector3(Mathf.Min(property.boundsValue.center.x, max), Mathf.Min(property.boundsValue.center.y, max), Mathf.Min(property.boundsValue.center.z, max)), new Vector3(Mathf.Min(property.boundsValue.size.x, max), Mathf.Min(property.boundsValue.size.y, max), Mathf.Min(property.boundsValue.size.z, max)));
					break;
				case "AnimationCurve":
					property.animationCurveValue = new AnimationCurve(property.animationCurveValue.Clamp(Mathf.Infinity, max, Mathf.Infinity, max).keys);
					break;
			}
		}
		End(property);
	}
}
#endif