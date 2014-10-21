using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Magicolo.EditorTools {
	public class CustomWindowBase : EditorWindow {

		protected bool SmallButton(GUIContent label) {
			bool pressed = false;
			
			GUIStyle style = new GUIStyle("toolbarbutton");
			style.clipping = TextClipping.Overflow;
			style.fontSize = 10;
			if (GUILayout.Button(label, style, GUILayout.Width(16))) {
				pressed = true;
			}
			return pressed;
		}
		
		protected bool TinyButton(GUIContent label) {
			bool pressed = false;
			
			GUIStyle style = new GUIStyle("MiniToolbarButtonLeft");
			style.clipping = TextClipping.Overflow;
			style.fontSize = 10;
			if (GUILayout.Button(label, style, GUILayout.Width(16))) {
				pressed = true;
			}
			return pressed;
		}
		
		protected bool LargeButton(GUIContent label) {
			bool pressed = false;
			
			GUIStyle style = new GUIStyle("toolbarButton");
			if (GUILayout.Button(label, style)) {
				pressed = true;
			}
			return pressed;
		}
		
		protected void Separator(bool reserveVerticalSpace = true) {
			if (reserveVerticalSpace) {
				GUILayout.Space(4);
				EditorGUILayout.LabelField(GUIContent.none, new GUIStyle("RL DragHandle"), GUILayout.MinWidth(10), GUILayout.Height(4));
				GUILayout.Space(4);
			}
			else {
				Rect rect = EditorGUILayout.BeginVertical();
				rect.y += 7;
				EditorGUI.LabelField(rect, GUIContent.none, new GUIStyle("RL DragHandle"));
				EditorGUILayout.EndVertical();
			}
		}
	
		protected void Box(Rect rect) {
			rect.width -= EditorGUI.indentLevel * 16 - 1;
			rect.height += 1;
			rect.x += EditorGUI.indentLevel * 16;
			GUI.Box(rect, "", new GUIStyle("box"));
		}
	}
}
