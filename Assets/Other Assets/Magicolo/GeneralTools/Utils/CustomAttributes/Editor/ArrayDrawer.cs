#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ArrayAttribute))]
public class ArrayDrawer : CustomPropertyDrawerBase {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
		if (!fieldInfo.FieldType.IsArray){
			Debug.LogError("ArrayAttribute can only be applied to arrays.");
			return;
		}
		drawPrefixLabel = false;
		position = Begin(position, property, label);
		
		EditorGUI.PropertyField(position, property, label, true);
		
		position.x -= 4;
		DeleteElementButtonWithArrows(position, arrayProperty, index);
		
		if (index == 0){
			position.y -= 18;
			AddElementButton(position, arrayProperty);
		}
		
		End(property);
	}
}
#endif