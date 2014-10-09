#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Hydrogen.Core;

[CustomEditor(typeof(hObjectPool))]
public class hObjectPoolEditor : CustomEditorBase {
	
	hObjectPool objectPool;
	SerializedProperty objectPoolsProperty;
	ObjectPoolCollection currentPool;
	SerializedProperty currentPoolProperty;
	
	void OnEnable(){
		((hObjectPool) target).SetExecutionOrder(-12);
	}
	
    public override void OnInspectorGUI(){
		objectPool = (hObjectPool) target;
		objectPoolsProperty = serializedObject.FindProperty("ObjectPools");
		
		Begin();
		
		EditorGUILayout.PropertyField(serializedObject.FindProperty("Persistent"));
		ShowDefaultSettings();
		Separator();
		ShowObjectPools();
		
		End();
		
    }
	
	void ShowDefaultSettings(){
		BeginBox();
		EditorGUILayout.LabelField("Default Pool Settings");

		EditorGUI.indentLevel += 1;
			
		EditorGUILayout.PropertyField(serializedObject.FindProperty("CullExtras"), new GUIContent("Cull Extras", "The default value used when adding objects to the Object Pool."));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("CullInterval"), new GUIContent("Cull Interval", "How often should we look at culling extra objects."));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("HandleParticles"), new GUIContent("Handle Particles", "Should particle systems be appropriately handled when despawning?"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("PreloadAmount"), new GUIContent("Preload Amount", "The number of objects to preload in an Object Pool."));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("SlowMessage"), new GUIContent("Send Message", "Should Unity's SendMessage be used (OnSpawned, WaitToDespawn, OnDespawned)."));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("SpawnMore"), new GUIContent("Spawn More", "Should additional objects be spawned as needed?"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("TrackObjects"), new GUIContent("Track Objects", "Should objects be tracked when they are spawned?"));
		
		EditorGUI.indentLevel -= 1;
		EndBox();
	}
	
	void ShowObjectPools(){
		if (LargeAddElementButton(objectPoolsProperty, new GUIContent("Add New Object Pool"))){
			objectPool.ObjectPools[objectPool.ObjectPools.Length - 1] = new ObjectPoolCollection(objectPool.PreloadAmount, objectPool.SpawnMore, objectPool.SlowMessage, objectPool.HandleParticles, objectPool.TrackObjects, objectPool.CullExtras, objectPool.CullInterval);
		}

		if (objectPool == null || objectPool.ObjectPools == null){
			return;
		}
		
		for (int i = 0; i < objectPool.ObjectPools.Length; i++) {
			currentPool = objectPool.ObjectPools[i];
			currentPoolProperty = serializedObject.FindProperty("ObjectPools").GetArrayElementAtIndex(i);
			
			BeginBox();
			string currentPoolName = currentPool.Prefab != null ? currentPool.Prefab.name : "default";
			if (DeleteElementFoldOutWithArrows(objectPoolsProperty, i, currentPoolName.ToGUIContent())){
				break;
			}
			
			if (currentPoolProperty.isExpanded){
				EditorGUI.indentLevel += 1;
			
				EditorGUILayout.PropertyField(currentPoolProperty.FindPropertyRelative("Prefab"), new GUIContent("Prefab", "Reference to the Prefab or GameObject used by this Object Pool."));
				EditorGUILayout.PropertyField(currentPoolProperty.FindPropertyRelative("CullExtras"), new GUIContent("Cull Extras", "The default value used when adding objects to the Object Pool."));
				EditorGUILayout.PropertyField(currentPoolProperty.FindPropertyRelative("CullInterval"), new GUIContent("Cull Interval", "How often should we look at culling extra objects."));
				EditorGUILayout.PropertyField(currentPoolProperty.FindPropertyRelative("ManageParticles"), new GUIContent("Handle Particles", "Should particle systems be appropriately handled when despawning?"));
				EditorGUILayout.PropertyField(currentPoolProperty.FindPropertyRelative("PreloadAmount"), new GUIContent("Preload Amount", "The number of objects to preload in an Object Pool."));
				EditorGUILayout.PropertyField(currentPoolProperty.FindPropertyRelative("SendMessage"), new GUIContent("Send Message", "Should Unity's SendMessage be used (OnSpawned, WaitToDespawn, OnDespawned)."));
				EditorGUILayout.PropertyField(currentPoolProperty.FindPropertyRelative("SpawnMore"), new GUIContent("Spawn More", "Should additional objects be spawned as needed?"));
				EditorGUILayout.PropertyField(currentPoolProperty.FindPropertyRelative("TrackObjects"), new GUIContent("Track Objects", "Should objects be tracked when they are spawned?"));
				EditorGUILayout.PropertyField(currentPoolProperty.FindPropertyRelative("DespawnPoolLocation"), new GUIContent("Despawn Pool Location", "Should despawned object be returned to it's pool's origin position?"));
				Separator();
				
				EditorGUI.indentLevel -= 1;
			}
			EndBox();
		}
	}
}
#endif