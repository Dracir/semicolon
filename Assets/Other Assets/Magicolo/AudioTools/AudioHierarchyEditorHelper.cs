using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioHierarchyEditorHelper : Magicolo.EditorTools.EditorHelper {
		
		IEnumerator routine;
		AudioSource previewAudioSource;
		
		static AudioHierarchyEditorHelper instance;
		
		public static AudioHierarchyEditorHelper GetInstance() {
			if (instance == null) {
				instance = new AudioHierarchyEditorHelper();
			}
			instance.Update();
			return instance;
		}
		
		public override void OnHierarchyWindowItemGUI(int instanceid, Rect selectionrect) {
			#if UNITY_EDITOR
			Texture icon = UnityEditor.EditorGUIUtility.ObjectContent(null, typeof(AudioSource)).image;
			GameObject gameObject = UnityEditor.EditorUtility.InstanceIDToObject(instanceid) as GameObject;
			
			if (gameObject == null || icon == null)
				return;
			
			float width = selectionrect.width;
			selectionrect.width = 30;
			selectionrect.height = 16;
			
			AudioOptions audioOptions = gameObject.GetComponent<AudioOptions>();
			if (audioOptions == null || audioOptions.audioInfo == null){
				return;
			}
			
			AudioInfo audioInfo = audioOptions.audioInfo;
			selectionrect.x = width - 3 + gameObject.GetHierarchyDepth() * 14;
			selectionrect.height += 1;
			GUIStyle style = new GUIStyle("MiniToolbarButtonLeft");
			style.fixedHeight += 1;
			style.contentOffset = new Vector2(-4, 0);
			style.clipping = TextClipping.Overflow;
			
			if (GUI.Button(selectionrect, icon, style)){
				UnityEditor.Selection.activeObject = gameObject;
				
				if (previewAudioSource != null){
					previewAudioSource.gameObject.Remove();
				}
				
				previewAudioSource = audioInfo.Source.PlayOnListener();
				if (previewAudioSource != null){
					audioInfo.audioPlayer.itemManager.SetAudioSource(previewAudioSource, audioInfo);
					routine = DestroyAfterPlaying(previewAudioSource);
				}
			}
			else if (Event.current.isMouse && Event.current.type == EventType.mouseDown){
				if (previewAudioSource != null){
					previewAudioSource.gameObject.Remove();
				}
				routine = null;
			}
			#endif
		}
		
		public override void OnPlaymodeStateChanged() {
			base.OnPlaymodeStateChanged();
			
			if (previewAudioSource != null) {
				previewAudioSource.gameObject.Remove();
			}
			routine = null;
		}
		
		public override void OnUpdate() {
			base.OnUpdate();
			
			if (routine != null && !routine.MoveNext()) {
				routine = null;
			}
		}
		
		IEnumerator DestroyAfterPlaying(AudioSource audioSource) {
			while (audioSource.isPlaying) {
				yield return null;
			}
			audioSource.gameObject.Remove();
		}
	}
}