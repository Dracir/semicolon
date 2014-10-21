#if UNITY_EDITOR
using Magicolo.EditorTools;
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Magicolo.AudioTools {
	[CustomEditor(typeof(Magicolo.AudioTools.AudioOptions)), CanEditMultipleObjects]
	public class AudioOptionsEditor : CustomEditorBase {
	
		AudioOptions audioOptions;
		AudioInfo audioInfo;
		SerializedProperty audioInfoProperty;
		AudioClip clip;
		
		public override void OnInspectorGUI() {
			audioOptions = (Magicolo.AudioTools.AudioOptions)target;
			audioInfo = audioOptions.audioInfo;
			audioInfoProperty = serializedObject.FindProperty("audioInfo");
			clip = audioOptions.Source.clip;
			
			Begin();
			
			ShowFadeIn();
			ShowFadeInCurve();
			ShowFadeOut();
			ShowFadeOutCurve();
			EditorGUILayout.PropertyField(audioInfoProperty.FindPropertyRelative("randomVolume"));
			EditorGUILayout.PropertyField(audioInfoProperty.FindPropertyRelative("randomPitch"));
			EditorGUILayout.PropertyField(audioInfoProperty.FindPropertyRelative("doNotKill"));
			ShowClipInfo();
			
			End();
		}
	
		void ShowFadeIn(){
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(audioInfoProperty.FindPropertyRelative("fadeIn"));
			if (clip.length <= 0) {
				return;
			}
			if (EditorGUI.EndChangeCheck()){
				serializedObject.ApplyModifiedProperties();
				audioInfo.fadeOut = Mathf.Clamp(audioInfo.fadeOut, 0, clip.length - audioInfo.fadeIn);
			}
		}
		
		void ShowFadeOut(){
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(audioInfoProperty.FindPropertyRelative("fadeOut"));
			if (clip.length <= 0) {
				return;
			}
			if (EditorGUI.EndChangeCheck()){
				serializedObject.ApplyModifiedProperties();
				audioInfo.fadeIn = Mathf.Clamp(audioInfo.fadeIn, 0, clip.length - audioInfo.fadeOut);
			}
			audioInfo.fadeIn = Mathf.Clamp(audioInfo.fadeIn, 0, clip.length);
			audioInfo.fadeOut = Mathf.Clamp(audioInfo.fadeOut, 0, clip.length);
		}
		
		void ShowFadeInCurve() {
			audioInfo.fadeInCurve = EditorGUILayout.CurveField("Fade In Curve".ToGUIContent(), audioInfo.fadeInCurve, Color.cyan, new Rect(0, 0, 1, 1));
			if (audioInfo.fadeInCurve.keys.Length < 2) {
				audioInfo.fadeInCurve.keys = new []{ new Keyframe(0, 0), new Keyframe(1, 1) };
			}
			audioInfo.fadeInCurve.MoveKey(0, new Keyframe(0, 0));
			audioInfo.fadeInCurve.MoveKey(audioInfo.fadeInCurve.keys.Length - 1, new Keyframe(1, 1));
		}
		
		void ShowFadeOutCurve() {
			audioInfo.fadeOutCurve = EditorGUILayout.CurveField("Fade Out Curve".ToGUIContent(), audioInfo.fadeOutCurve, Color.cyan, new Rect(0, 0, 1, 1));
			if (audioInfo.fadeOutCurve.keys.Length < 2) {
				audioInfo.fadeOutCurve.keys = new []{ new Keyframe(0, 1), new Keyframe(1, 0) };
			}
			audioInfo.fadeOutCurve.MoveKey(0, new Keyframe(0, 1));
			audioInfo.fadeOutCurve.MoveKey(audioInfo.fadeOutCurve.keys.Length - 1, new Keyframe(1, 0));
		}
	
		void ShowClipInfo() {
			BeginBox();
			audioInfoProperty.isExpanded = EditorGUILayout.Foldout(audioInfoProperty.isExpanded, "Clip Info");
			
			if (audioInfoProperty.isExpanded){
				EditorGUI.indentLevel += 1;
				GUIStyle style = EditorStyles.boldLabel;
				EditorGUILayout.LabelField("Name:", clip.name, style);
				EditorGUILayout.LabelField("Channels:", clip.channels.ToString(), style);
				EditorGUILayout.LabelField("Frequency:", clip.frequency.ToString(), style);
				EditorGUILayout.LabelField("Length:", clip.length.ToString(), style);
				EditorGUILayout.LabelField("Samples:", clip.samples.ToString(), style);
				EditorGUI.indentLevel -= 1;
			}
			EndBox();
		}
	}
}
#endif
