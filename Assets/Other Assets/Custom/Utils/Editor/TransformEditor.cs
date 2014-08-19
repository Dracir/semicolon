#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Transform)), CanEditMultipleObjects]
public class TransformEditor : Editor {

	Transform transform;
	Vector3 pLocalPosition;
	Vector3 pLocalRotation;
	Vector3 pLocalScale;
	bool snap;
	bool grid;
	bool resetBreak;
	
	// Snap Settings
	float moveX;
	float moveY;
	float moveZ;
	float rotation;
	float scale;
	int gridSize;
	bool showCubes;
	bool showLines;
	bool fadeWithDistance;
	
	void Awake(){
		SnapSettings.CleanUp();
	}
	
	public override void OnInspectorGUI(){
		transform = (Transform) target;
		resetBreak = false;
		
		GetSnapSettings();
		
		if (!resetBreak) DrawUtilityButtons();
		if (!resetBreak) DrawVectors();
		if (!resetBreak) DrawGrid();
	}
	
	void OnSceneGUI(){
		transform = (Transform) target;
		GetSnapSettings();
		DrawGrid();
		
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
		moveX = SnapSettings.GetValue<float>("MoveX");
		moveY = SnapSettings.GetValue<float>("MoveY");
		moveZ = SnapSettings.GetValue<float>("MoveZ");
		rotation = SnapSettings.GetValue<float>("Rotation");
		scale = SnapSettings.GetValue<float>("Scale");
		gridSize = SnapSettings.GetValue<int>("GridSize");
		showCubes = SnapSettings.GetValue<bool>("ShowCubes");
		showLines = SnapSettings.GetValue<bool>("ShowLines");
		fadeWithDistance = SnapSettings.GetValue<bool>("FadeWithDistance");
	}
	
	void DrawUtilityButtons(){
		EditorGUILayout.BeginHorizontal();
		
		// Reset Button
		if (GUILayout.Button(new GUIContent(". Reset  .", "Resets the transform to it's original state."), EditorStyles.miniButton, GUILayout.Width("Reset".GetWidth(EditorStyles.miniFont) + 12))){
			Undo.RegisterCompleteObjectUndo(Selection.GetTransforms(SelectionMode.Editable), "Transform Reset");
			foreach (var t in Selection.GetTransforms(SelectionMode.Editable)){
				t.Reset();
				SnapSettings.DeleteKey("Toggle" + "Snap" + t.GetInstanceID());
				SnapSettings.DeleteKey("Toggle" + "Grid" + t.GetInstanceID());
				EditorUtility.SetDirty(t);
				resetBreak = true;
			}
			return;
		}
		
		DrawSeparator();
		
		// Snap Button
		snap = DrawToggleButton("Snap", "Toggles the snapping of the transform to the grid. See Edit/Preferences -> Snap Settings to change the snap settings.", EditorStyles.miniButtonLeft);
		EditorGUI.BeginChangeCheck();
		grid = DrawToggleButton("Grid", "Toggles the display of the grid. See Edit/Preferences -> Snap Settings to change the grid display settings.", EditorStyles.miniButtonRight);
		if (EditorGUI.EndChangeCheck())	SceneView.RepaintAll();
		
		// Add Button
		if (GUILayout.Button(new GUIContent(". Add  .", "Adds a child to the transform."), EditorStyles.miniButtonLeft, GUILayout.Width("Add".GetWidth(EditorStyles.miniFont) + 12))){
			foreach (var t in Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab)){
				Undo.RegisterCreatedObjectUndo(t.AddChild().gameObject, "New Child");
				EditorUtility.SetDirty(t);
			}
		}
		
		// Sort Button
		if (GUILayout.Button(new GUIContent(". Sort  .", "Sorts the immediate children of the transform alphabetically."), EditorStyles.miniButtonRight, GUILayout.Width("Sort".GetWidth(EditorStyles.miniFont) + 12))){
			Undo.RegisterCompleteObjectUndo(Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab), "Transform Sort");
			foreach (var t in Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab)){
				t.SortChildren();
				EditorUtility.SetDirty(t);
			}
		}
		
		EditorGUILayout.EndHorizontal();
	}
	
	void DrawVectors(){
		bool changedX = false;
		bool changedY = false;
		bool changedZ = false;
		
		// Local Position
		Vector3 localPosition = DrawVectorWithButton(transform.localPosition, ". P  .", "Resets the transform's local position to it's initial state.", Vector3.zero, ref changedX, ref changedY, ref changedZ);
		Vector3 parentScale = Vector3.one;
		if (transform.parent != null) parentScale = transform.parent.lossyScale;
		if (changedX){
			Undo.RegisterCompleteObjectUndo(Selection.GetTransforms(SelectionMode.Editable), "Transform Move X");
			if (snap && !resetBreak && !Application.isPlaying) localPosition.x = (transform.localPosition.x + (localPosition.x - transform.localPosition.x) * (moveX / parentScale.x) * 10).Round(moveX / parentScale.x);
			transform.SetLocalPosition(localPosition, "X");
			foreach (var t in Selection.GetTransforms(SelectionMode.Editable)){
				t.SetLocalPosition(transform.localPosition, "X");
				EditorUtility.SetDirty(t);
			}
		}
		if (changedY){
			Undo.RegisterCompleteObjectUndo(Selection.GetTransforms(SelectionMode.Editable), "Transform Move Y");
			if (snap && !resetBreak && !Application.isPlaying) localPosition.y = (transform.localPosition.y + (localPosition.y - transform.localPosition.y) * (moveY / parentScale.y) * 10).Round(moveY / parentScale.y);
			transform.SetLocalPosition(localPosition, "Y");
			foreach (var t in Selection.GetTransforms(SelectionMode.Editable)){
				t.SetLocalPosition(transform.localPosition, "Y");
				EditorUtility.SetDirty(t);
			}
		}
		if (changedZ){
			Undo.RegisterCompleteObjectUndo(Selection.GetTransforms(SelectionMode.Editable), "Transform Move Z");
			if (snap && !resetBreak && !Application.isPlaying) localPosition.z = (transform.localPosition.z + (localPosition.z - transform.localPosition.z) * (moveZ / parentScale.z) * 10).Round(moveZ / parentScale.z);
			transform.SetLocalPosition(localPosition, "Z");
			foreach (var t in Selection.GetTransforms(SelectionMode.Editable)){
				t.SetLocalPosition(transform.localPosition, "Z");
				EditorUtility.SetDirty(t);
			}
		}
		
		// Local Rotation
		Vector3 localEulerAngles = DrawVectorWithButton(transform.localEulerAngles, ". R  .", "Resets the transform's local rotation to it's initial state.", Vector3.zero, ref changedX, ref changedY, ref changedZ);
		if (changedX){
			Undo.RegisterCompleteObjectUndo(Selection.GetTransforms(SelectionMode.Editable), "Transform Rotate X");
			if (snap && !resetBreak && !Application.isPlaying) localEulerAngles.x = (transform.localEulerAngles.x + (localEulerAngles.x - transform.localEulerAngles.x) * rotation * 10).Round(rotation) % 360;
			transform.SetLocalEulerAngles(localEulerAngles, "X");
			foreach (var t in Selection.GetTransforms(SelectionMode.Editable)){
				t.SetLocalEulerAngles(transform.localEulerAngles, "X");
				EditorUtility.SetDirty(t);
			}
		}
		if (changedY){
			Undo.RegisterCompleteObjectUndo(Selection.GetTransforms(SelectionMode.Editable), "Transform Rotate Y");
			if (snap && !resetBreak && !Application.isPlaying) localEulerAngles.y = (transform.localEulerAngles.y + (localEulerAngles.y - transform.localEulerAngles.y) * rotation * 10).Round(rotation) % 360;
			transform.SetLocalEulerAngles(localEulerAngles, "Y");
			foreach (var t in Selection.GetTransforms(SelectionMode.Editable)){
				t.SetLocalEulerAngles(transform.localEulerAngles, "Y");
				EditorUtility.SetDirty(t);
			}
		}
		if (changedZ){
			Undo.RegisterCompleteObjectUndo(Selection.GetTransforms(SelectionMode.Editable), "Transform Rotate Z");
			if (snap && !resetBreak && !Application.isPlaying) localEulerAngles.z = (transform.localEulerAngles.z + (localEulerAngles.z - transform.localEulerAngles.z) * rotation * 10).Round(rotation) % 360;
			transform.SetLocalEulerAngles(localEulerAngles, "Z");
			foreach (var t in Selection.GetTransforms(SelectionMode.Editable)){
				t.SetLocalEulerAngles(transform.localEulerAngles, "Z");
				EditorUtility.SetDirty(t);
			}
		}
		
		// Local Scale
		EditorGUI.BeginChangeCheck();
		Vector3 localScale = DrawVectorWithButton(transform.localScale, ". S  .", "Resets the transform's local scale to it's initial state.", Vector3.one, ref changedX, ref changedY, ref changedZ);
		if (changedX){
			Undo.RegisterCompleteObjectUndo(Selection.GetTransforms(SelectionMode.Editable), "Transform Scale X");
			if (snap && !resetBreak && !Application.isPlaying) localScale.x = (transform.localScale.x + (localScale.x - transform.localScale.x) * scale * 10).Round(scale);
			transform.SetLocalScale(localScale, "X");
			foreach (var t in Selection.GetTransforms(SelectionMode.Editable)){
				t.SetLocalScale(transform.localScale, "X");
				EditorUtility.SetDirty(t);
			}
		}
		if (changedY){
			Undo.RegisterCompleteObjectUndo(Selection.GetTransforms(SelectionMode.Editable), "Transform Scale Y");
			if (snap && !resetBreak && !Application.isPlaying) localScale.y = (transform.localScale.y + (localScale.y - transform.localScale.y) * scale * 10).Round(scale);
			transform.SetLocalScale(localScale, "Y");
			foreach (var t in Selection.GetTransforms(SelectionMode.Editable)){
				t.SetLocalScale(transform.localScale, "Y");
				EditorUtility.SetDirty(t);
			}
		}
		if (changedZ){
			Undo.RegisterCompleteObjectUndo(Selection.GetTransforms(SelectionMode.Editable), "Transform Scale Z");
			if (snap && !resetBreak && !Application.isPlaying) localScale.z = (transform.localScale.z + (localScale.z - transform.localScale.z) * scale * 10).Round(scale);
			transform.SetLocalScale(localScale, "Z");
			foreach (var t in Selection.GetTransforms(SelectionMode.Editable)){
				t.SetLocalScale(transform.localScale, "Z");
				EditorUtility.SetDirty(t);
			}
		}
	}
	
	Vector3 DrawVectorWithButton(Vector3 vector, string buttonLabel, string tooltip, Vector3 resetValue, ref bool changedX, ref bool changedY, ref bool changedZ) {
		float labelWidth = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 15;
		
		EditorGUILayout.BeginHorizontal();
		
		if (GUILayout.Button(new GUIContent(buttonLabel, tooltip), EditorStyles.miniButton, GUILayout.Width(20))){
			changedX = true;
			changedY = true;
			changedZ = true;
			resetBreak = true;
			return resetValue;
		}
		
		EditorGUI.BeginChangeCheck();
		vector.x = EditorGUILayout.FloatField("X", vector.x, GUILayout.Height(EditorGUIUtility.singleLineHeight));
		if (EditorGUI.EndChangeCheck()) changedX = true;
		else changedX = false;
		EditorGUI.BeginChangeCheck();
		vector.y = EditorGUILayout.FloatField("Y", vector.y, GUILayout.Height(EditorGUIUtility.singleLineHeight));
		if (EditorGUI.EndChangeCheck()) changedY = true;
		else changedY = false;
		EditorGUI.BeginChangeCheck();
		vector.z = EditorGUILayout.FloatField("Z", vector.z, GUILayout.Height(EditorGUIUtility.singleLineHeight));
		if (EditorGUI.EndChangeCheck()) changedZ = true;
		else changedZ = false;
		
		EditorGUILayout.EndHorizontal();
		
		if (float.IsNaN(vector.x)) vector.x = 0;
		if (float.IsNaN(vector.y)) vector.y = 0;
		if (float.IsNaN(vector.z)) vector.z = 0;
		
		EditorGUIUtility.labelWidth = labelWidth;
		return vector;
	}
	
	void DrawGrid(){
		if (grid && !Application.isPlaying){
			if (Selection.activeTransform == transform){
				bool squares = true;
				bool is3D = true;
				if (SceneView.currentDrawingSceneView != null) is3D = !SceneView.currentDrawingSceneView.in2DMode;
				float size = 0.1F * ((moveX + moveY + moveZ) / 3);
				float alphaFactor = 1.25F;
				float alphaFade = 2;
				float alpha;
				float xAlpha = 0.5F;
				float yAlpha = 0.5F;
				
				for (int y = -gridSize; y <= gridSize; y++){
					yAlpha = Mathf.Pow((1 - Mathf.Abs((float) y) / gridSize) * alphaFactor, alphaFade) / 2;
					for (int x = -gridSize; x <= gridSize; x++){
						xAlpha = Mathf.Pow((1 - Mathf.Abs((float) x) / gridSize) * alphaFactor, alphaFade) / 2;
						if (fadeWithDistance) alpha = xAlpha + yAlpha;
						else alpha = 0.5F * alphaFactor;
						
						if (alpha.Round(0.1) != 0){
							// X Squares
							Handles.lighting = false;
							Handles.color = new Color(0.1F, 0.25F, 0.75F, alpha);
							Vector3 offset = new Vector3(x * moveX, y * moveY, 0);
							Vector3 position = transform.position + offset;
							position.x = position.x.Round(moveX);
							position.y = position.y.Round(moveY);
							position.z = position.z.Round(moveZ);
							
							if (showCubes){
								if (SceneView.currentDrawingSceneView != null){
									if (SceneView.currentDrawingSceneView.camera.WorldPointInView(position)){
										if (squares) Handles.CubeCap(0, position, Quaternion.identity, size);
									}
								}
								else if (squares) Handles.CubeCap(0, position, Quaternion.identity, size);
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
											if (squares) Handles.CubeCap(0, position, Quaternion.identity, size);
										}
									}
									else if (squares) Handles.CubeCap(0, position, Quaternion.identity, size);
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
											if (squares) Handles.CubeCap(0, position, Quaternion.identity, size);
										}
									}
									else if (squares) Handles.CubeCap(0, position, Quaternion.identity, size);
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
	
	void DrawSeparator(){
		Rect position = EditorGUILayout.BeginVertical();
		position.y += 7;
		EditorGUI.LabelField(position, GUIContent.none, new GUIStyle("RL DragHandle"));
		EditorGUILayout.EndVertical();
	}
	
	bool DrawToggleButton(string buttonLabel, string tooltip, GUIStyle buttonStyle){
		bool pressed = SnapSettings.GetValue<bool>("Toggle" + buttonLabel + transform.GetInstanceID());
		float width = buttonLabel.GetWidth(EditorStyles.miniFont) + 12;
		int spacing = 0;
		
		if (buttonStyle == EditorStyles.miniButtonLeft) spacing = -6;
		
		Rect position = EditorGUILayout.BeginVertical(GUILayout.Width(buttonLabel.GetWidth(EditorStyles.miniFont) + 16 + spacing), GUILayout.Height(EditorGUIUtility.singleLineHeight));
		EditorGUILayout.Space();
		pressed = EditorGUI.Toggle(new Rect(position.x + 4, position.y + 2, width, position.height - 1), pressed, buttonStyle);
		if (pressed) EditorGUI.LabelField(new Rect(position.x + 8, position.y + 1, width, position.height - 1), new GUIContent(buttonLabel, tooltip), EditorStyles.whiteMiniLabel);
		else EditorGUI.LabelField(new Rect(position.x + 8, position.y + 1, width, position.height - 1), new GUIContent(buttonLabel, tooltip), EditorStyles.miniLabel);
		if (pressed != SnapSettings.GetValue<bool>("Toggle" + buttonLabel + transform.GetInstanceID())){
			foreach (var t in Selection.GetTransforms(SelectionMode.Editable)){
				SnapSettings.SetValue("Toggle" + buttonLabel + t.GetInstanceID(), pressed);
				EditorUtility.SetDirty(t);
			}
		}
		EditorGUILayout.EndVertical();
		
		return SnapSettings.GetValue<bool>("Toggle" + buttonLabel + transform.GetInstanceID());
	}
	
}
#endif