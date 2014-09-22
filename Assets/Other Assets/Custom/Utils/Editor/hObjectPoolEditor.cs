#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Hydrogen.Core;

[CustomEditor(typeof(hObjectPool))]
public class hObjectPoolEditor : CustomEditorBase {
	
	hObjectPool objectPool;
	bool objectPoolsShowing = true;
	
	void OnEnable(){
		((hObjectPool) target).SetExecutionOrder(-12);
	}
	
    public override void OnInspectorGUI(){
		objectPool = (hObjectPool) target;
		
		Begin();
		
		ShowDefaultSettings();
		ShowObjectPools();
		Separator();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("Persistent"));
		
		End();
		
    }
	
	void ShowDefaultSettings(){
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
	}
	
	void ShowObjectPools(){
		objectPoolsShowing = AddElementFoldOut(serializedObject.FindProperty("ObjectPools"), objectPoolsShowing, new GUIContent("Object Pools", "Our pooled object collections."), OnPoolAdded);
	
		if (objectPoolsShowing){
			EditorGUI.indentLevel += 1;
			
			for (int i = 0; i < objectPool.ObjectPools.Length; i++) {
				ObjectPoolCollection pool = objectPool.ObjectPools[i];
				SerializedProperty poolProperty = serializedObject.FindProperty("ObjectPools").GetArrayElementAtIndex(i);
				
				string poolName = pool.Prefab != null ? pool.Prefab.name : "empty";
				if (!showingDict.ContainsKey("Pool" + i)) showingDict["Pool" + i] = false;
				showingDict["Pool" + i] = DeleteElementFoldOutWithArrows(serializedObject.FindProperty("ObjectPools"), i, showingDict["Pool" + i], poolName);
				if (deleteBreak) break;
				
				if (showingDict["Pool" + i]){
					EditorGUI.indentLevel += 1;
				
					EditorGUILayout.PropertyField(poolProperty.FindPropertyRelative("Prefab"), new GUIContent("Prefab", "Reference to the Prefab or GameObject used by this Object Pool."));
					EditorGUILayout.PropertyField(poolProperty.FindPropertyRelative("CullExtras"), new GUIContent("Cull Extras", "The default value used when adding objects to the Object Pool."));
					EditorGUILayout.PropertyField(poolProperty.FindPropertyRelative("CullInterval"), new GUIContent("Cull Interval", "How often should we look at culling extra objects."));
					EditorGUILayout.PropertyField(poolProperty.FindPropertyRelative("ManageParticles"), new GUIContent("Handle Particles", "Should particle systems be appropriately handled when despawning?"));
					EditorGUILayout.PropertyField(poolProperty.FindPropertyRelative("PreloadAmount"), new GUIContent("Preload Amount", "The number of objects to preload in an Object Pool."));
					EditorGUILayout.PropertyField(poolProperty.FindPropertyRelative("SendMessage"), new GUIContent("Send Message", "Should Unity's SendMessage be used (OnSpawned, WaitToDespawn, OnDespawned)."));
					EditorGUILayout.PropertyField(poolProperty.FindPropertyRelative("SpawnMore"), new GUIContent("Spawn More", "Should additional objects be spawned as needed?"));
					EditorGUILayout.PropertyField(poolProperty.FindPropertyRelative("TrackObjects"), new GUIContent("Track Objects", "Should objects be tracked when they are spawned?"));
					EditorGUILayout.PropertyField(poolProperty.FindPropertyRelative("DespawnPoolLocation"), new GUIContent("Despawn Pool Location", "Should despawned object be returned to it's pool's origin position?"));
					Separator();
					
					EditorGUI.indentLevel -= 1;
				}
			}
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void OnPoolAdded(SerializedProperty newPool){
		objectPool.ObjectPools[objectPool.ObjectPools.Length - 1] = new ObjectPoolCollection(objectPool.PreloadAmount, objectPool.SpawnMore, objectPool.SlowMessage, objectPool.HandleParticles, objectPool.TrackObjects, objectPool.CullExtras, objectPool.CullInterval);
	}
}
#endif