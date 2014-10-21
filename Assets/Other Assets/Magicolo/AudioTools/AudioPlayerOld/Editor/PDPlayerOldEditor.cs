//#if UNITY_EDITOR
//using UnityEngine;
//using UnityEditor;
//using System.Reflection;
//using System.Collections.Generic;
//
//[CustomEditor(typeof(PDPlayerOld))]
//public class PDPlayerOldEditor : CustomEditorBase {
//
//	PDPlayerOld pureDataPlayer;
//	PDPlayerOld.Module currentModule;
//	
//	public override void OnInspectorGUI(){
//		pureDataPlayer = (PDPlayerOld) target;
//		
//		Begin();
//		
//		ShowModules();
//		
//		End();
//	}
//	
//	void ShowModules(){
//		if (LargeAddElementButton(serializedObject.FindProperty("modules"), "Add New Module".ToGUIContent())){
//			pureDataPlayer.modules[pureDataPlayer.modules.Length - 1] = new PDPlayerOld.Module();
//			pureDataPlayer.modules[pureDataPlayer.modules.Length - 1].name = PDPlayerOld.GetUniqueName(pureDataPlayer.modules, "default");
//		}
//		
//		for (int i = 0; i < pureDataPlayer.modules.Length; i++) {
//			currentModule = pureDataPlayer.modules[i];
//			SerializedProperty moduleProperty = serializedObject.FindProperty("modules").GetArrayElementAtIndex(i);
//			
//			currentModule.showing = DeleteElementFoldOutWithArrows(serializedObject.FindProperty("modules"), i, currentModule.showing, currentModule.name);
//			if (deleteBreak) break;
//			
//			if (currentModule.showing){
//				EditorGUI.indentLevel += 1;
//				
//				EditorGUI.BeginDisabledGroup(Application.isPlaying);
//				currentModule.name = EditorGUILayout.TextField(new GUIContent("Prefix", "All Pure Data receivers need to be named starting with this prefix to communicate appropriately with the PDPlayer."), currentModule.name);
//				EditorGUI.EndDisabledGroup();
//				EditorGUILayout.PropertyField(moduleProperty.FindPropertyRelative("suffixes"), true);
//				EditorGUILayout.PropertyField(moduleProperty.FindPropertyRelative("playOnAwake"));
//				EditorGUI.BeginChangeCheck();
//				EditorGUILayout.PropertyField(moduleProperty.FindPropertyRelative("volume"));
//				if (EditorGUI.EndChangeCheck())	currentModule.SetVolume(moduleProperty.FindPropertyRelative("volume").floatValue);
//				EditorGUILayout.PropertyField(moduleProperty.FindPropertyRelative("source"));
//				if (currentModule.source != null){
//					EditorGUI.indentLevel += 1;
//					EditorGUILayout.PropertyField(moduleProperty.FindPropertyRelative("volumeRolloff"));
//					EditorGUI.BeginChangeCheck();
//					EditorGUILayout.PropertyField(moduleProperty.FindPropertyRelative("minDistance"));
//					if (EditorGUI.EndChangeCheck())	moduleProperty.FindPropertyRelative("maxDistance").floatValue = Mathf.Max(moduleProperty.FindPropertyRelative("maxDistance").floatValue, moduleProperty.FindPropertyRelative("minDistance").floatValue);
//					EditorGUI.BeginChangeCheck();
//					EditorGUILayout.PropertyField(moduleProperty.FindPropertyRelative("maxDistance"));
//					if (EditorGUI.EndChangeCheck())	moduleProperty.FindPropertyRelative("minDistance").floatValue = Mathf.Clamp(moduleProperty.FindPropertyRelative("minDistance").floatValue, 0, moduleProperty.FindPropertyRelative("maxDistance").floatValue);
//					EditorGUILayout.PropertyField(moduleProperty.FindPropertyRelative("panLevel"));
//					EditorGUI.indentLevel -= 1;
//				}
//				ShowSends(currentModule, moduleProperty);
//				Separator();
//				
//				EditorGUI.indentLevel -= 1;
//			}
//		}
//	}
//	
//	void ShowSends(PDPlayerOld.Module module, SerializedProperty moduleProperty){
//		string tooltip = "Used to send data to Pure Data.";
//		module.showSends = AddElementFoldOut(moduleProperty.FindPropertyRelative("sends"), module.showSends, new GUIContent("Sends", tooltip), OnSendAdded);
//		
//		if (module.showSends){
//			EditorGUI.indentLevel += 1;
//			
//			for (int i = 0; i < module.sends.Length; i++) {
//				PDPlayerOld.Send send = module.sends[i];
//				SerializedProperty sendProperty = moduleProperty.FindPropertyRelative("sends").GetArrayElementAtIndex(i);
//				
//				if (send == null) continue;
//				
//				send.showing = DeleteElementFoldOutWithArrows(moduleProperty.FindPropertyRelative("sends"), i, send.showing, send.suffix);
//				if (deleteBreak) break;
//				
//				if (send.showing){
//					EditorGUI.indentLevel += 1;
//					
//					EditorGUI.BeginChangeCheck();
//					EditorGUILayout.PropertyField(sendProperty.FindPropertyRelative("suffix"));
//					
//					EditorGUI.BeginDisabledGroup(Application.isPlaying);
//					EditorGUILayout.PropertyField(sendProperty.FindPropertyRelative("type"));
//					if (send.type == PDPlayerOld.Send.SendTypes.Audio){
//						EditorGUILayout.PropertyField(sendProperty.FindPropertyRelative("audioSource"), GUIContent.none);
//					}
//					else {
//						EditorGUILayout.PropertyField(sendProperty.FindPropertyRelative("mode"));
//						EditorGUILayout.PropertyField(sendProperty.FindPropertyRelative("currentObject"), GUIContent.none);
//						ShowComponents(send);
//						ShowFieldsAndProperties(send);
//					}
//					
//					if (EditorGUI.EndChangeCheck())	send.changed = true;
//					EditorGUI.EndDisabledGroup();
//					
//					EditorGUI.indentLevel -= 1;
//				}
//			}
//			EditorGUI.indentLevel -= 1;
//		}
//	}
//	
//	void OnSendAdded(SerializedProperty addedElement){
//		currentModule.sends[currentModule.sends.Length - 1] = new PDPlayerOld.Send();
//	}
//	
//	void ShowComponents(PDPlayerOld.Send send){
//		string tooltip = "The component from which to choose a value to send to Pure Data.";
//		
//		if (send.currentObject != null){
//			List<Object> components = new List<Object>(){send.currentObject};
//			components.AddRange(send.currentObject.GetComponents<Component>());
//			List<string> displayedOptions = new List<string>();
//			
//			foreach (Object component in components) {
//				displayedOptions.Add(component.GetType().Name);
//			}
//			
//			EditorGUI.BeginChangeCheck();
//			send.componentIndex = Mathf.Min(EditorGUILayout.Popup(new GUIContent("Component", tooltip), send.componentIndex, displayedOptions.ToArray().ToGUIContents()), displayedOptions.Count - 1);
//			send.currentComponent = components[send.componentIndex];
//			if (EditorGUI.EndChangeCheck())	send.valueIndex = 0;
//		}
//		else {
//			EditorGUILayout.Popup(new GUIContent("Component", tooltip), 0, new []{"null"}.ToGUIContents());
//			send.componentIndex = 0;
//			send.currentComponent = null;
//		}
//	}
//	
//	void ShowFieldsAndProperties(PDPlayerOld.Send send){
//		string tooltip = "The value that will be sent to Pure Data.\nHover the value to see it's original type and it's Pure Data converted type.";
//		List<string> displayedOptions = new List<string>();
//		
//		if (send.currentComponent != null){
//			send.SetObjectToSend();
//			
//			foreach (MemberInfo member in send.members) {
//				System.Type memberType = null;
//				if (member.MemberType == MemberTypes.Field)
//					memberType = ((FieldInfo)member).FieldType;
//				else if (member.MemberType == MemberTypes.Property)
//					memberType = ((PropertyInfo)member).PropertyType;
//				
//				if (memberType != null) {
//					displayedOptions.Add(string.Format("{0}:{1}", member.Name.Capitalize(), PDPlayerOld.ToPDType(memberType)));
//				}
//			}
//		}
//		
//		if (displayedOptions.Count > 0){
//			EditorGUI.BeginChangeCheck();
//			send.valueIndex = Mathf.Min(EditorGUILayout.Popup(new GUIContent("Value", tooltip), send.valueIndex, displayedOptions.ToArray().ToGUIContents(':')), displayedOptions.Count - 1);
//			if (EditorGUI.EndChangeCheck()) send.SetObjectToSend();
//		}
//		else{
//			EditorGUILayout.Popup(new GUIContent("Value", tooltip), 0, new []{"null"}.ToGUIContents());
//			send.valueIndex = 0;
//			send.objectToSend = null;
//		}
//	}
//}
//#endif
