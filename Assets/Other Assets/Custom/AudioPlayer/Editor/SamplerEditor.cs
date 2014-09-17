#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Sampler))]
public class SamplerEditor : CustomEditorBase {
	
	Sampler sampler;
	
	public override void OnInspectorGUI(){
		sampler = (Sampler) target;
		
		Begin();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUI.BeginDisabledGroup(Application.isPlaying);
		if (LargeAddElementButton(serializedObject.FindProperty("instruments"), "Add New Instrument")){
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
				Instrument instrument = sampler.instruments[i];
				SerializedProperty instrumentProperty = serializedObject.FindProperty("instruments").GetArrayElementAtIndex(i);
				int noteCounter = 0;
				int octaveCounter = 0;
				
				instrument.showing = DeleteElementFoldOutWithArrows(serializedObject.FindProperty("instruments"), i, instrument.showing, instrument.name);
				if (deleteBreak) break;
				
				if (instrument.showing){
					EditorGUI.indentLevel += 1;
					instrument.minNote = Mathf.Round(instrument.minNote);
					instrument.maxNote = Mathf.Round(instrument.maxNote);
					
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					instrument.name = EditorGUILayout.TextField(instrument.name);
					instrument.generateMode = (Instrument.GenerateModes) EditorGUILayout.EnumPopup("Generate Mode", instrument.generateMode);
					
					if (instrument.generateMode == Instrument.GenerateModes.GenerateAtRuntime){
						EditorGUI.indentLevel += 1;
						EditorGUILayout.PropertyField(instrumentProperty.FindPropertyRelative("destroyIdle"));
						if (instrument.destroyIdle) EditorGUILayout.PropertyField(instrumentProperty.FindPropertyRelative("idleThreshold"));
						EditorGUI.indentLevel -= 1;
					}
					
					EditorGUILayout.PropertyField(instrumentProperty.FindPropertyRelative("is3D"), new GUIContent("3D Clips"));
					EditorGUI.EndDisabledGroup();
					
					EditorGUILayout.PropertyField(instrumentProperty.FindPropertyRelative("maxVoices"));
					if (PDPlayer.Instance != null) EditorGUILayout.PropertyField(instrumentProperty.FindPropertyRelative("sendToPD"));
					else instrumentProperty.FindPropertyRelative("sendToPD").boolValue = false;
					
					instrument.velocitySettingsShowing = EditorGUILayout.Foldout(instrument.velocitySettingsShowing, "Velocity");
					if (instrument.velocitySettingsShowing){
						EditorGUI.indentLevel += 1;
						EditorGUILayout.PropertyField(instrumentProperty.FindPropertyRelative("velocityAffectsVolume"), new GUIContent("Affects Volume"));
						instrument.velocityCurve = EditorGUILayout.CurveField("Curve", instrument.velocityCurve, Color.cyan, new Rect(0, 0, 1, 1), GUILayout.Height(16));
						EditorGUI.BeginDisabledGroup(Application.isPlaying);
						EditorGUILayout.PropertyField(instrumentProperty.FindPropertyRelative("velocityLayers"), new GUIContent("Layers"));
						EditorGUI.EndDisabledGroup();
						EditorGUI.indentLevel -= 1;
					}
					
					EditorGUILayout.BeginHorizontal();
					instrument.notesShowing = EditorGUILayout.Foldout(instrument.notesShowing, string.Format("Audio Clips ({0})", instrument.maxNote - instrument.minNote));
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					if (GUILayout.Button(". Reset  .", EditorStyles.miniButton, GUILayout.Width(50))) instrument.Reset();
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
					
					if (instrument.notesShowing){
						EditorGUI.indentLevel -= 1;
						instrument.InitializeClips();
						
						EditorGUI.BeginDisabledGroup(Application.isPlaying);
						EditorGUILayout.BeginHorizontal();
						GUILayout.Space(16);
						EditorGUILayout.PropertyField(instrumentProperty.FindPropertyRelative("minNote"), GUIContent.none, GUILayout.Width(50));
						EditorGUILayout.MinMaxSlider(ref instrument.minNote, ref instrument.maxNote, 0, 127);
						EditorGUILayout.PropertyField(instrumentProperty.FindPropertyRelative("maxNote"), GUIContent.none, GUILayout.Width(50));
						EditorGUILayout.EndHorizontal();
						EditorGUI.EndDisabledGroup();
						
						for (int j = 0; j < 128; j++){
							if (j >= instrument.minNote && j <= instrument.maxNote){
								EditorGUILayout.BeginHorizontal();
								GUILayout.Space(16);
								if (GUILayout.Button(instrument.noteNames[noteCounter].ToString() + octaveCounter.ToString() + " (" + j.ToString() + ")", GUILayout.MinWidth(80), GUILayout.Height(14))){
									if (instrument.GetClipCount() != 0){
										if (Application.isPlaying) Sampler.Play(instrument.name, j, 127, sampler.gameObject);
									}
								}
								for (int k = 0; k < instrument.velocityLayers; k++){
									EditorGUI.BeginDisabledGroup(Application.isPlaying);
									if (!Application.isPlaying){
										instrument.audioSources[j + (k * 128)] = (AudioSource) EditorGUILayout.ObjectField(instrument.audioSources[j + (k * 128)], typeof(AudioSource), true, GUILayout.Height(14));
										instrument.audioClips[j + (k * 128)] = instrument.audioSources[j + (k * 128)] != null ? instrument.audioSources[j + (k * 128)].clip : null;
									}
									else {
										EditorGUILayout.ObjectField(instrument.audioClips[j + (k * 128)], typeof(AudioClip), true, GUILayout.Height(14));
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
