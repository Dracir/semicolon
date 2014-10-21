﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Magicolo.EditorTools {
	[CustomEditor(typeof(Transform), true), CanEditMultipleObjects]
	public class TransformEditor : CustomEditorBase {
	
		Transform transform;
		Vector3 pLocalPosition;
		Vector3 pLocalRotation;
		Vector3 pLocalScale;
		bool snap;
		bool grid;
		
		// Snap Settings
		float moveX;
		float moveY;
		float moveZ;
		float rotation;
		float scale;
		int gridSize;
		bool showCubes;
		bool showLines;
		
		void OnEnable(){
			transform = (Transform) target;
			Magicolo.EditorTools.SnapSettings.CleanUp();
		}
		
		public override void OnInspectorGUI(){
			GetSnapSettings();
			
			Begin(false);
			
			if (!deleteBreak) ShowUtilityButtons();
			if (!deleteBreak) ShowVectors();
			if (!deleteBreak) ShowGrid();
			
			End(false);
		}
		
		void OnSceneGUI(){
			GetSnapSettings();
			ShowGrid();
			
			if (snap){
				if (Selection.GetTransforms(SelectionMode.Editable).Contains(transform)){
					if (pLocalPosition != transform.localPosition && !Application.isPlaying){
						Vector3 localPosition = transform.localPosition;
						Vector3 parentScale = Vector3.one;
						if (transform.parent != null) parentScale = transform.parent.lossyScale;
						localPosition.x = localPosition.x.Round(moveX / parentScale.x);
						localPosition.y = localPosition.y.Round(moveY / parentScale.y);
						localPosition.z = localPosition.z.Round(moveZ / parentScale.z);
						if (float.IsNaN(localPosition.x)) localPosition.x = 0;
						if (float.IsNaN(localPosition.y)) localPosition.y = 0;
						if (float.IsNaN(localPosition.z)) localPosition.z = 0;
						transform.localPosition = localPosition;
						pLocalPosition = transform.localPosition;
					}
					if (pLocalRotation != transform.localEulerAngles && !Application.isPlaying){
						Vector3 localRotation = transform.localEulerAngles;
						localRotation.x = localRotation.x.Round(rotation);
						localRotation.y = localRotation.y.Round(rotation);
						localRotation.z = localRotation.z.Round(rotation);
						if (float.IsNaN(localRotation.x)) localRotation.x = 0;
						if (float.IsNaN(localRotation.y)) localRotation.y = 0;
						if (float.IsNaN(localRotation.z)) localRotation.z = 0;
						transform.localEulerAngles = localRotation;
						pLocalRotation = transform.localEulerAngles;
					}
					if (pLocalScale != transform.localScale && !Application.isPlaying){
						Vector3 localScale = transform.localScale;
						localScale.x = localScale.x.Round(scale);
						localScale.y = localScale.y.Round(scale);
						localScale.z = localScale.z.Round(scale);
						if (float.IsNaN(localScale.x)) localScale.x = 0;
						if (float.IsNaN(localScale.y)) localScale.y = 0;
						if (float.IsNaN(localScale.z)) localScale.z = 0;
						transform.localScale = localScale;
						pLocalScale = transform.localScale;
					}
				}
			}
		}
		
		void GetSnapSettings(){
			moveX = Magicolo.EditorTools.SnapSettings.GetValue<float>("MoveX");
			moveY = Magicolo.EditorTools.SnapSettings.GetValue<float>("MoveY");
			moveZ = Magicolo.EditorTools.SnapSettings.GetValue<float>("MoveZ");
			rotation = Magicolo.EditorTools.SnapSettings.GetValue<float>("Rotation");
			scale = Magicolo.EditorTools.SnapSettings.GetValue<float>("Scale");
			gridSize = Magicolo.EditorTools.SnapSettings.GetValue<int>("GridSize");
			showCubes = Magicolo.EditorTools.SnapSettings.GetValue<bool>("ShowCubes");
			showLines = Magicolo.EditorTools.SnapSettings.GetValue<bool>("ShowLines");
		}
		
		void ShowUtilityButtons(){
			EditorGUILayout.BeginHorizontal();
			
			// Reset Button
			if (GUILayout.Button(new GUIContent(". Reset  .", "Resets the transform to it's original state."), EditorStyles.miniButton, GUILayout.Width("Reset".GetWidth(EditorStyles.miniFont) + 12))){
				Undo.RegisterCompleteObjectUndo(targets, "Transform Reset");
				foreach (Transform t in targets){
					t.Reset();
					Magicolo.EditorTools.SnapSettings.RemoveKey("Toggle" + "Snap" + t.GetInstanceID());
					Magicolo.EditorTools.SnapSettings.RemoveKey("Toggle" + "Grid" + t.GetInstanceID());
					EditorUtility.SetDirty(t);
					deleteBreak = true;
				}
				return;
			}
			
			Separator(false);
			
			// Snap Button
			snap = ShowToggleButton("Snap", "Toggles the snapping of the transform to the grid. See Magicolo's Tools/Snap Settings to change the snap settings.", EditorStyles.miniButtonLeft);
			EditorGUI.BeginChangeCheck();
			grid = ShowToggleButton("Grid", "Toggles the display of the grid. See Magicolo's Tools/Snap Settings to change the grid display settings.", EditorStyles.miniButtonRight);
			if (EditorGUI.EndChangeCheck())	SceneView.RepaintAll();
			
			// Add Button
			if (GUILayout.Button(new GUIContent(". Add  .", "Adds a child to the transform."), EditorStyles.miniButtonLeft, GUILayout.Width("Add".GetWidth(EditorStyles.miniFont) + 12))){
				foreach (Transform t in targets){
					Undo.RegisterCreatedObjectUndo(t.AddChild().gameObject, "New Child");
					EditorUtility.SetDirty(t);
				}
			}
			
			// Sort Button
			if (GUILayout.Button(new GUIContent(". Sort  .", "Sorts the immediate children of the transform alphabetically."), EditorStyles.miniButtonRight, GUILayout.Width("Sort".GetWidth(EditorStyles.miniFont) + 12))){
				Undo.RegisterCompleteObjectUndo(targets, "Transform Sort");
				foreach (Transform t in targets){
					t.SortChildren();
					EditorUtility.SetDirty(t);
				}
			}
			
			EditorGUILayout.EndHorizontal();
		}
		
		void ShowVectors(){
			const float sensibility = 0.15F;
			
			Vector3 parentScale = Vector3.one;
			if (transform.parent != null) parentScale = transform.parent.lossyScale;
			ShowVectorWithButton(serializedObject.FindProperty("m_LocalPosition"), ". P  .", "Resets the transform's local position to it's initial state.", Vector3.zero, new Vector3(moveX / parentScale.x, moveY / parentScale.y, moveZ / parentScale.z), sensibility);
			ShowQuaternionWithButton(serializedObject.FindProperty("m_LocalRotation"), ". R  .", "Resets the transform's local rotation to it's initial state.", Quaternion.identity, new Vector3(rotation, rotation, rotation), sensibility);
			ShowVectorWithButton(serializedObject.FindProperty("m_LocalScale"), ". S  .", "Resets the transform's local scale to it's initial state.", Vector3.one, new Vector3(scale, scale, scale), sensibility);
		}
		
		void ShowVectorWithButton(SerializedProperty vectorProperty, string buttonLabel, string tooltip, Vector3 resetValue, Vector3 roundValue, float sensibility) {
			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 15;
			
			EditorGUILayout.BeginHorizontal();
			
			if (GUILayout.Button(new GUIContent(buttonLabel, tooltip), EditorStyles.miniButton, GUILayout.Width(20))){
				vectorProperty.vector3Value = resetValue;
				serializedObject.ApplyModifiedProperties();
				deleteBreak = true;
				return;
			}
			
			Vector3 previousValue = vectorProperty.vector3Value;
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(vectorProperty.FindPropertyRelative("x"));
			if (EditorGUI.EndChangeCheck()){
				if (snap && !deleteBreak && !Application.isPlaying) {
					if (Event.current.delta.x != 0) vectorProperty.FindPropertyRelative("x").floatValue = (previousValue.x + Event.current.delta.x * roundValue.x * sensibility);
					vectorProperty.FindPropertyRelative("x").floatValue = vectorProperty.FindPropertyRelative("x").floatValue.Round(roundValue.x);
					serializedObject.ApplyModifiedProperties();
				}
			}
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(vectorProperty.FindPropertyRelative("y"));
			if (EditorGUI.EndChangeCheck()){
				if (snap && !deleteBreak && !Application.isPlaying) {
					if (Event.current.delta.x != 0) vectorProperty.FindPropertyRelative("y").floatValue = (previousValue.y + Event.current.delta.x * roundValue.y * sensibility);
					vectorProperty.FindPropertyRelative("y").floatValue = vectorProperty.FindPropertyRelative("y").floatValue.Round(roundValue.y);
					serializedObject.ApplyModifiedProperties();
				}
			}
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(vectorProperty.FindPropertyRelative("z"));
			if (EditorGUI.EndChangeCheck()){
				if (snap && !deleteBreak && !Application.isPlaying) {
					if (Event.current.delta.x != 0) vectorProperty.FindPropertyRelative("z").floatValue = (previousValue.z + Event.current.delta.x * roundValue.z * sensibility);
					vectorProperty.FindPropertyRelative("z").floatValue = vectorProperty.FindPropertyRelative("z").floatValue.Round(roundValue.z);
					serializedObject.ApplyModifiedProperties();
				}
			}
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUIUtility.labelWidth = labelWidth;
		}
		
		void ShowQuaternionWithButton(SerializedProperty quaternionProperty, string buttonLabel, string tooltip, Quaternion resetValue, Vector3 roundValue, float sensibility){
			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 15;
			
			EditorGUILayout.BeginHorizontal();
			
			if (GUILayout.Button(new GUIContent(buttonLabel, tooltip), EditorStyles.miniButton, GUILayout.Width(20))){
				quaternionProperty.quaternionValue = resetValue;
				serializedObject.ApplyModifiedProperties();
				deleteBreak = true;
				return;
			}
			
			Vector3 localEulerAngles = transform.localEulerAngles;
			
			EditorGUI.BeginChangeCheck();
			localEulerAngles.x = EditorGUILayout.FloatField("X", localEulerAngles.x % 360) % 360;
			if (EditorGUI.EndChangeCheck()){
				Undo.RegisterCompleteObjectUndo(targets, "Transform Rotate X");
				if (snap && !deleteBreak && !Application.isPlaying) {
					if (Event.current.delta.x != 0) localEulerAngles.x = (transform.localEulerAngles.x + Event.current.delta.x * roundValue.x * sensibility);
					localEulerAngles.x = localEulerAngles.x.Round(roundValue.x) % 360;
				}
				transform.SetLocalEulerAngles(localEulerAngles, "X");
				foreach (Transform t in targets){
					t.SetLocalEulerAngles(transform.localEulerAngles, "X");
					EditorUtility.SetDirty(t);
				}
			}
			
			EditorGUI.BeginChangeCheck();
			localEulerAngles.y = EditorGUILayout.FloatField("Y", localEulerAngles.y % 360) % 360;
			if (EditorGUI.EndChangeCheck()){
				Undo.RegisterCompleteObjectUndo(targets, "Transform Rotate Y");
				if (snap && !deleteBreak && !Application.isPlaying) {
					if (Event.current.delta.x != 0) localEulerAngles.y = (transform.localEulerAngles.y + Event.current.delta.x * roundValue.y * sensibility);
					localEulerAngles.y = localEulerAngles.y.Round(roundValue.y) % 360;
				}
				transform.SetLocalEulerAngles(localEulerAngles, "Y");
				foreach (Transform t in targets){
					t.SetLocalEulerAngles(transform.localEulerAngles, "Y");
					EditorUtility.SetDirty(t);
				}
			}
			
			EditorGUI.BeginChangeCheck();
			localEulerAngles.z = EditorGUILayout.FloatField("Z", localEulerAngles.z % 360) % 360;
			if (EditorGUI.EndChangeCheck()){
				Undo.RegisterCompleteObjectUndo(targets, "Transform Rotate Z");
				if (snap && !deleteBreak && !Application.isPlaying) {
					if (Event.current.delta.x != 0) localEulerAngles.z = (transform.localEulerAngles.z + Event.current.delta.x * roundValue.z * sensibility);
					localEulerAngles.z = localEulerAngles.z.Round(roundValue.z) % 360;
				}
				transform.SetLocalEulerAngles(localEulerAngles, "Z");
				foreach (Transform t in targets){
					t.SetLocalEulerAngles(transform.localEulerAngles, "Z");
					EditorUtility.SetDirty(t);
				}
			}
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUIUtility.labelWidth = labelWidth;
		}
		
		void ShowGrid(){
			if (grid && !Application.isPlaying){
				if (Selection.activeTransform == transform){
					bool is3D = true;
					if (SceneView.currentDrawingSceneView != null) is3D = !SceneView.currentDrawingSceneView.in2DMode;
					float size = 0.1F * ((moveX + moveY + moveZ) / 3);
					const float alphaFactor = 1.25F;
					float alpha = alphaFactor / 2;
					
					for (int y = -gridSize; y <= gridSize; y++){
						for (int x = -gridSize; x <= gridSize; x++){
							if (alpha.Round(0.1) != 0){
								// X Squares
								Handles.lighting = false;
								Handles.color = new Color(0.1F, 0.25F, 0.75F, alpha);
								var offset = new Vector3(x * moveX, y * moveY, 0);
								Vector3 position = transform.position + offset;
								position.x = position.x.Round(moveX);
								position.y = position.y.Round(moveY);
								position.z = position.z.Round(moveZ);
								
								if (showCubes){
									if (SceneView.currentDrawingSceneView != null){
										if (SceneView.currentDrawingSceneView.camera.WorldPointInView(position)){
											Handles.CubeCap(0, position, Quaternion.identity, size);
										}
									}
									else Handles.CubeCap(0, position, Quaternion.identity, size);
								}
								
								// X Lines
								if (showLines){
									Handles.color = new Color(0.1F, 0.25F, 0.75F, alpha / 2);
									if (x == gridSize) Handles.DrawLine(new Vector3(transform.position.x - offset.x, position.y, position.z), position);
									if (y == gridSize) Handles.DrawLine(new Vector3(position.x, transform.position.y - offset.y, position.z), position);
								}
								
								if (!is3D) alpha = 1.5F;
								if (is3D || (!is3D && y == 0)){
									// Y Squares
									Handles.color = new Color(0.75F, 0.35F, 0.1F, alpha);
									offset = new Vector3(x * moveX, 0, y * moveZ);
									position = transform.position + offset;
									position.x = position.x.Round(moveX);
									position.y = position.y.Round(moveY);
									position.z = position.z.Round(moveZ);
									
									if (showCubes){
										if (SceneView.currentDrawingSceneView != null){
											if (SceneView.currentDrawingSceneView.camera.WorldPointInView(position)){
												Handles.CubeCap(0, position, Quaternion.identity, size);
											}
										}
										else Handles.CubeCap(0, position, Quaternion.identity, size);
									}
									
									// Y Lines
									if (showLines){
										Handles.color = new Color(0.75F, 0.35F, 0.1F, alpha / 2);
										if (x == gridSize) Handles.DrawLine(new Vector3(transform.position.x - offset.x, position.y, position.z), position);
										if (y == gridSize) Handles.DrawLine(new Vector3(position.x, position.y, transform.position.z - offset.z), position);
									}
								}
								
								if (is3D || (!is3D && x == 0)){
									// Z Squares
									Handles.color = new Color(0.75F, 0, 0.25F, alpha);
									offset = new Vector3(0, y * moveY, x * moveZ);
									position = transform.position + offset;
									position.x = position.x.Round(moveX);
									position.y = position.y.Round(moveY);
									position.z = position.z.Round(moveZ);
									
									if (showCubes){
										if (SceneView.currentDrawingSceneView != null){
											if (SceneView.currentDrawingSceneView.camera.WorldPointInView(position)){
												Handles.CubeCap(0, position, Quaternion.identity, size);
											}
										}
										else Handles.CubeCap(0, position, Quaternion.identity, size);
									}
									
									// Z Lines
									if (showLines){
										Handles.color = new Color(0.75F, 0, 0.25F, alpha / 2);
										if (y == gridSize) Handles.DrawLine(new Vector3(position.x, transform.position.y - offset.y, position.z), position);
										if (x == gridSize) Handles.DrawLine(new Vector3(position.x, position.y, transform.position.z - offset.z), position);
									}
								}
							}
						}
					}
					SceneView.RepaintAll();
				}
			}
		}
		
		bool ShowToggleButton(string buttonLabel, string tooltip, GUIStyle buttonStyle){
			bool pressed = Magicolo.EditorTools.SnapSettings.GetValue<bool>("Toggle" + buttonLabel + transform.GetInstanceID());
			float width = buttonLabel.GetWidth(EditorStyles.miniFont) + 12;
			int spacing = 0;
			
			if (buttonStyle == EditorStyles.miniButtonLeft) spacing = -6;
			
			Rect position = EditorGUILayout.BeginVertical(GUILayout.Width(buttonLabel.GetWidth(EditorStyles.miniFont) + 16 + spacing), GUILayout.Height(EditorGUIUtility.singleLineHeight));
			EditorGUILayout.Space();
			pressed = EditorGUI.Toggle(new Rect(position.x + 4, position.y + 2, width, position.height - 1), pressed, buttonStyle);
			if (pressed) EditorGUI.LabelField(new Rect(position.x + 8, position.y + 1, width, position.height - 1), new GUIContent(buttonLabel, tooltip), EditorStyles.whiteMiniLabel);
			else EditorGUI.LabelField(new Rect(position.x + 8, position.y + 1, width, position.height - 1), new GUIContent(buttonLabel, tooltip), EditorStyles.miniLabel);
			if (pressed != Magicolo.EditorTools.SnapSettings.GetValue<bool>("Toggle" + buttonLabel + transform.GetInstanceID())){
				foreach (Transform t in targets){
					Magicolo.EditorTools.SnapSettings.SetValue("Toggle" + buttonLabel + t.GetInstanceID(), pressed);
					EditorUtility.SetDirty(t);
				}
			}
			EditorGUILayout.EndVertical();
			
			return Magicolo.EditorTools.SnapSettings.GetValue<bool>("Toggle" + buttonLabel + transform.GetInstanceID());
		}
	}
}
#endif