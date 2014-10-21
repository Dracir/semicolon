#if UNITY_EDITOR
using Magicolo.EditorTools;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Sampler))]
public class SamplerEditor : CustomEditorBase {
	
	Sampler sampler;
	SerializedProperty instrumentsProperty;
	Instrument currentInstrument;
	SerializedProperty currentInstrumentProperty;
	
	public override void OnInspectorGUI(){
		sampler = (Sampler) target;
		instrumentsProperty = serializedObject.FindProperty("instruments");
		
		Begin();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUI.BeginDisabledGroup(Application.isPlaying);
		if (LargeAddElementButton(instrumentsProperty, "Add New Instrument".ToGUIContent())){
			sampler.instruments[sampler.instruments.Length - 1] = new Instrument();
			sampler.instruments[sampler.instruments.Length - 1].name = Sampler.GetUniqueName(sampler.instruments, "default");
		}
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndHorizontal();
		ShowInstruments();
		
		End();
	}
	
	void ShowInstruments(){
		if (sampler.instruments != null){
			for (int i = 0; i < sampler.instruments.Length; i++){
				currentInstrument = sampler.instruments[i];
				currentInstrumentProperty = instrumentsProperty.GetArrayElementAtIndex(i);
				int noteCounter = 0;
				int octaveCounter = 0;
				
				if (DeleteElementFoldOutWithArrows(instrumentsProperty, i, currentInstrument.name.ToGUIContent())){
					break;
				}
				
				if (currentInstrumentProperty.isExpanded){
					EditorGUI.indentLevel += 1;
					currentInstrument.minNote = Mathf.Round(currentInstrument.minNote);
					currentInstrument.maxNote = Mathf.Round(currentInstrument.maxNote);
					
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					currentInstrument.name = EditorGUILayout.TextField(currentInstrument.name);
					currentInstrument.generateMode = (Instrument.GenerateModes) EditorGUILayout.EnumPopup("Generate Mode", currentInstrument.generateMode);
					
					if (currentInstrument.generateMode == Instrument.GenerateModes.GenerateAtRuntime){
						EditorGUI.indentLevel += 1;
						EditorGUILayout.PropertyField(currentInstrumentProperty.FindPropertyRelative("destroyIdle"));
						if (currentInstrument.destroyIdle) EditorGUILayout.PropertyField(currentInstrumentProperty.FindPropertyRelative("idleThreshold"));
						EditorGUI.indentLevel -= 1;
					}
					
					EditorGUILayout.PropertyField(currentInstrumentProperty.FindPropertyRelative("is3D"), new GUIContent("3D Clips"));
					EditorGUI.EndDisabledGroup();
					
					EditorGUILayout.PropertyField(currentInstrumentProperty.FindPropertyRelative("maxVoices"));
					if (PDPlayerOld.Instance != null) EditorGUILayout.PropertyField(currentInstrumentProperty.FindPropertyRelative("sendToPD"));
					else currentInstrumentProperty.FindPropertyRelative("sendToPD").boolValue = false;
					
					currentInstrument.velocitySettingsShowing = EditorGUILayout.Foldout(currentInstrument.velocitySettingsShowing, "Velocity");
					if (currentInstrument.velocitySettingsShowing){
						EditorGUI.indentLevel += 1;
						EditorGUILayout.PropertyField(currentInstrumentProperty.FindPropertyRelative("velocityAffectsVolume"), new GUIContent("Affects Volume"));
						currentInstrument.velocityCurve = EditorGUILayout.CurveField("Curve", currentInstrument.velocityCurve, Color.cyan, new Rect(0, 0, 1, 1), GUILayout.Height(16));
						EditorGUI.BeginDisabledGroup(Application.isPlaying);
						EditorGUILayout.PropertyField(currentInstrumentProperty.FindPropertyRelative("velocityLayers"), new GUIContent("Layers"));
						EditorGUI.EndDisabledGroup();
						EditorGUI.indentLevel -= 1;
					}
					
					EditorGUILayout.BeginHorizontal();
					currentInstrument.notesShowing = EditorGUILayout.Foldout(currentInstrument.notesShowing, string.Format("Audio Clips ({0})", currentInstrument.maxNote - currentInstrument.minNote));
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					if (GUILayout.Button(". Reset  .", EditorStyles.miniButton, GUILayout.Width(50))) currentInstrument.Reset();
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
					
					if (currentInstrument.notesShowing){
						EditorGUI.indentLevel -= 1;
						currentInstrument.InitializeClips();
						
						EditorGUI.BeginDisabledGroup(Application.isPlaying);
						EditorGUILayout.BeginHorizontal();
						GUILayout.Space(16);
						EditorGUILayout.PropertyField(currentInstrumentProperty.FindPropertyRelative("minNote"), GUIContent.none, GUILayout.Width(50));
						EditorGUILayout.MinMaxSlider(ref currentInstrument.minNote, ref currentInstrument.maxNote, 0, 127);
						EditorGUILayout.PropertyField(currentInstrumentProperty.FindPropertyRelative("maxNote"), GUIContent.none, GUILayout.Width(50));
						EditorGUILayout.EndHorizontal();
						EditorGUI.EndDisabledGroup();
						
						for (int j = 0; j < 128; j++){
							if (j >= currentInstrument.minNote && j <= currentInstrument.maxNote){
								EditorGUILayout.BeginHorizontal();
								GUILayout.Space(16);
								if (GUILayout.Button(currentInstrument.noteNames[noteCounter] + octaveCounter + " (" + j.ToString() + ")", GUILayout.MinWidth(80), GUILayout.Height(14))){
									if (currentInstrument.GetClipCount() != 0){
										if (Application.isPlaying) Sampler.Play(currentInstrument.name, j, 127, sampler.gameObject);
									}
								}
								for (int k = 0; k < currentInstrument.velocityLayers; k++){
									EditorGUI.BeginDisabledGroup(Application.isPlaying);
									if (!Application.isPlaying){
										currentInstrument.audioSources[j + (k * 128)] = (AudioSource) EditorGUILayout.ObjectField(currentInstrument.audioSources[j + (k * 128)], typeof(AudioSource), true, GUILayout.Height(14));
										currentInstrument.audioClips[j + (k * 128)] = currentInstrument.audioSources[j + (k * 128)] != null ? currentInstrument.audioSources[j + (k * 128)].clip : null;
									}
									else {
										EditorGUILayout.ObjectField(currentInstrument.audioClips[j + (k * 128)], typeof(AudioClip), true, GUILayout.Height(14));
									}
									EditorGUI.EndDisabledGroup();
								}
								EditorGUILayout.EndHorizontal();
							}
							noteCounter += 1;
							if (noteCounter == 12){octaveCounter += 1; noteCounter = 0;}
						}
						EditorGUI.indentLevel += 1;
					}
					Separator();
					EditorGUI.indentLevel -= 1;
				}
			}
		}
	}
}
#endif
