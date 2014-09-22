#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(AudioPlayer))]
public class AudioPlayerEditor : CustomEditorBase {

	AudioPlayer audioPlayer;
	AudioPlayer.Container currentContainer;
	AudioPlayer.SubContainer currentSource;
	
	[MenuItem("Magicolo's Tools/Create/Audio Player")]
	static void CreateAudioPlayer() {
		GameObject gameObject;
		AudioPlayer existingAudioPlayer = FindObjectOfType<AudioPlayer>();
		
		if (existingAudioPlayer == null) {
			gameObject = new GameObject();
			gameObject.name = "AudioPlayer";
			gameObject.AddComponent<AudioPlayer>();
		}
		else {
			gameObject = existingAudioPlayer.gameObject;
		}
		Selection.activeGameObject = gameObject;
	}
	
	public override void OnInspectorGUI(){
		audioPlayer = (AudioPlayer) target;
		if (!audioPlayer.initialized) return;
			
		Begin();
		
		EditorGUILayout.PropertyField(serializedObject.FindProperty("audioClipsPath"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("masterVolume"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("maxVoices"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("tempoSettings"), new GUIContent(string.Format("Tempo Settings: {0} | {1}", audioPlayer.tempoSettings.beatsPerMinute, audioPlayer.tempoSettings.beatsPerMeasure)), true);
		Separator();
		ShowContainers();
		ShowRTPCs();
		ShowBuses();
		
		if (Sampler.Instance == null || PDPlayer.Instance == null){
			Separator();
			if (Sampler.Instance == null){
				if (LargeAddElementButton("Add Sampler"))
					audioPlayer.gameObject.AddComponent<Sampler>();
			}
			if (PDPlayer.Instance == null){
				if (LargeAddElementButton("Add PDPlayer"))
					audioPlayer.gameObject.AddComponent<PDPlayer>();
			}
		}
		
		End();
	}
	
	void ShowRTPCs(){
		if (audioPlayer.rTPCs == null) return;
		
		audioPlayer.showRTPCs = AddElementFoldOut(serializedObject.FindProperty("rTPCs"), audioPlayer.showRTPCs, "RTPCs", OnRTPCAdded);
		
		if (audioPlayer.showRTPCs){
			EditorGUI.indentLevel += 1;
			
			for (int i = 0; i < audioPlayer.rTPCs.Length; i++){
				AudioPlayer.RTPC rtpc = audioPlayer.rTPCs[i];
				SerializedProperty rtpcProperty = serializedObject.FindProperty("rTPCs").GetArrayElementAtIndex(i);
				
				EditorGUILayout.BeginHorizontal();
				rtpc.showing = EditorGUILayout.Foldout(rtpc.showing, rtpc.name);
				GUILayout.Space(30);
				if (!rtpc.showing) rtpc.defaultValue = EditorGUILayout.Slider(rtpc.defaultValue, rtpc.minValue, rtpc.maxValue);
				GUILayout.Space(10);
				DeleteElementButtonWithArrows(serializedObject.FindProperty("rTPCs"), i);
				if (deleteBreak) break;
				EditorGUILayout.EndHorizontal();
				
				if (rtpc.showing){
					EditorGUI.indentLevel += 1;
					
					EditorGUI.BeginChangeCheck();
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					rtpc.name = EditorGUILayout.TextField(rtpc.name);
					EditorGUI.EndDisabledGroup();
					rtpc.defaultValue = EditorGUILayout.Slider("Value", rtpc.defaultValue, rtpc.minValue, rtpc.maxValue);
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(rtpcProperty.FindPropertyRelative("minValue"));
					if (EditorGUI.EndChangeCheck())	rtpcProperty.FindPropertyRelative("maxValue").floatValue = Mathf.Max(rtpcProperty.FindPropertyRelative("maxValue").floatValue, rtpcProperty.FindPropertyRelative("minValue").floatValue);
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(rtpcProperty.FindPropertyRelative("maxValue"));
					if (EditorGUI.EndChangeCheck())	rtpcProperty.FindPropertyRelative("minValue").floatValue = Mathf.Clamp(rtpcProperty.FindPropertyRelative("minValue").floatValue, 0, rtpcProperty.FindPropertyRelative("maxValue").floatValue);
					rtpcProperty.FindPropertyRelative("minValue").floatValue = Mathf.Max(rtpcProperty.FindPropertyRelative("minValue").floatValue, 0);
					rtpcProperty.FindPropertyRelative("maxValue").floatValue = Mathf.Max(rtpcProperty.FindPropertyRelative("maxValue").floatValue, 0);
					if (EditorGUI.EndChangeCheck())	rtpcProperty.FindPropertyRelative("changed").boolValue = true;
					Separator();
					
					EditorGUI.indentLevel -= 1;
				}
			}
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void OnRTPCAdded(SerializedProperty newRTPC){
		audioPlayer.rTPCs[audioPlayer.rTPCs.Length - 1] = new AudioPlayer.RTPC();
		audioPlayer.rTPCs[audioPlayer.rTPCs.Length - 1].name = AudioPlayer.GetUniqueName(audioPlayer.rTPCs, "default");	
	}
	
	void ShowBuses(){
		if (audioPlayer.buses == null) return;
		
		audioPlayer.showBuses = AddElementFoldOut(serializedObject.FindProperty("buses"), audioPlayer.showBuses, "Buses", OnBusAdded);
		
		if (audioPlayer.showBuses){
			EditorGUI.indentLevel += 1;
			
			for (int i = 0; i < audioPlayer.buses.Length; i++){
				AudioPlayer.AudioBus bus = audioPlayer.buses[i];
				SerializedProperty busProperty = serializedObject.FindProperty("buses").GetArrayElementAtIndex(i);
				
				EditorGUILayout.BeginHorizontal();
				bus.showing = EditorGUILayout.Foldout(bus.showing, bus.name);
				GUILayout.Space(30);
				if (!bus.showing) bus.volume = EditorGUILayout.Slider(bus.volume, 0, 100);
				GUILayout.Space(10);
				DeleteElementButtonWithArrows(serializedObject.FindProperty("buses"), i);
				if (deleteBreak) break;
				EditorGUILayout.EndHorizontal();
				
				if (bus.showing){
					EditorGUI.indentLevel += 1;
					
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					bus.name = EditorGUILayout.TextField(bus.name);
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.PropertyField(busProperty.FindPropertyRelative("volume"));
					Separator();
					
					EditorGUI.indentLevel -= 1;
				}
				
				if (bus.pVolume != bus.volume){
					bus.changed = true;
					bus.pVolume = bus.volume;
				}
			}
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void OnBusAdded(SerializedProperty newBus){
		audioPlayer.buses[audioPlayer.buses.Length - 1] = new AudioPlayer.AudioBus();
		audioPlayer.buses[audioPlayer.buses.Length - 1].name = AudioPlayer.GetUniqueName(audioPlayer.buses, "default");
	}
	
	void ShowContainers(){
		if (audioPlayer.containers == null)	return;
		
		audioPlayer.showContainers = AddElementFoldOut(serializedObject.FindProperty("containers"), audioPlayer.showContainers, "Containers", OnContainerAdded);
		
		if (audioPlayer.showContainers) {
			EditorGUI.indentLevel += 1;
			
			for (int i = 0; i < audioPlayer.containers.Length; i++) {
				AudioPlayer.Container container = audioPlayer.containers[i];
				currentContainer = container;
				SerializedProperty containerProperty = serializedObject.FindProperty("containers").GetArrayElementAtIndex(i);
				
				container.showing = DeleteElementFoldOutWithArrows(serializedObject.FindProperty("containers"), i, container.showing, container.name);
				if (deleteBreak) break;
				
				if (container.showing) {
					EditorGUI.indentLevel += 1;
					
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					container.name = EditorGUILayout.TextField(container.name);
					EditorGUI.EndDisabledGroup();
					container.containerType = (AudioPlayer.Container.ContainerTypes)EditorGUILayout.EnumPopup(container.containerType);
					ShowSources(container, containerProperty);
					
					EditorGUI.indentLevel -= 1;
				}
			}
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void OnContainerAdded(SerializedProperty newContainer){
		audioPlayer.containers[audioPlayer.containers.Length - 1] = new AudioPlayer.Container();
		audioPlayer.containers[audioPlayer.containers.Length - 1].name = AudioPlayer.GetUniqueName(audioPlayer.containers, "default");
		audioPlayer.containers[audioPlayer.containers.Length - 1].subContainers = new List<AudioPlayer.SubContainer>();
	}
	
	void ShowSources(AudioPlayer.Container container, SerializedProperty containerProperty){
		container.sourcesShowing = AddElementFoldOut(containerProperty.FindPropertyRelative("sources"), container.sourcesShowing, "Sources", OnSourceAdded);
		
		if (container.sourcesShowing){
			EditorGUI.indentLevel += 1;
			
			if (container.sources != null){
				for (int i = 0; i < container.sources.Length; i++){
					AudioPlayer.SubContainer source = container.sources[i];
					currentSource = source;
					
					source.showing = DeleteElementFoldOut(containerProperty.FindPropertyRelative("sources"), i, source.showing, source.name);
					if (deleteBreak){
						container.RemoveEmptyReferences();
						break;
					}
					
					switch (source.sourceType) {
						case AudioPlayer.SubContainer.ContainerTypes.AudioSource:
							ShowAudioSource(source, container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.Sampler:
							ShowSampler(source, container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.MixContainer:
							ShowMixContainer(source, container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.RandomContainer:
							ShowRandomContainer(source, container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.SequenceContainer:
							ShowSequenceContainer(source, container, containerProperty);
							break;
					}
				}
			}
			EditorGUI.indentLevel -= 1;
		}
		Separator();
	}
	
	void OnSourceAdded(SerializedProperty newSource){
		currentContainer.sources[currentContainer.sources.Length - 1] = new AudioPlayer.SubContainer();
		if (currentContainer.sources.Length > 1) currentContainer.sources[currentContainer.sources.Length - 1].Initialize(currentContainer, 0, currentContainer.sources[currentContainer.sources.Length - 2]);
		else currentContainer.sources[currentContainer.sources.Length - 1].Initialize(currentContainer, 0);
	}
	
	void ShowChildrenSources(AudioPlayer.SubContainer source, AudioPlayer.Container container, SerializedProperty containerProperty){
		source.sourcesShowing = AddElementFoldOut(containerProperty.FindPropertyRelative("subContainers"), source.sourcesShowing, "Sources", source.childrenLink.Count, OnChildSourceAdded);
		
		if (source.sourcesShowing){
			EditorGUI.indentLevel += 1;
			
			if (source.childrenLink.Count != 0){
				for (int i = 0; i < source.childrenLink.Count; i++){
					AudioPlayer.SubContainer childSource = container.GetSourceWithID(source.childrenLink[i]);
					currentSource = childSource;
					int index = container.subContainers.IndexOf(childSource);
					
					childSource.showing = DeleteElementFoldOut(containerProperty.FindPropertyRelative("subContainers"), index, childSource.showing, childSource.name);
					if (deleteBreak){
						container.GetSourceWithID(source.id).childrenLink.Remove(childSource.id);
						container.RemoveEmptyReferences();
						break;
					}
					
					switch (childSource.sourceType) {
						case AudioPlayer.SubContainer.ContainerTypes.AudioSource:
							ShowAudioSource(childSource, container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.Sampler:
							ShowSampler(childSource, container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.MixContainer:
							ShowMixContainer(childSource, container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.RandomContainer:
							ShowRandomContainer(childSource, container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.SequenceContainer:
							ShowSequenceContainer(childSource, container, containerProperty);
							break;
					}
				}
			}
			EditorGUI.indentLevel -= 1;
		}
		Separator();
	}
	
	void OnChildSourceAdded(SerializedProperty newChildSource){
		currentContainer.subContainers[currentContainer.subContainers.Count - 1] = new AudioPlayer.SubContainer();
		if (currentContainer.subContainers.Count > 1) {
			currentContainer.subContainers.Last().Initialize(currentContainer, currentSource.id, currentContainer.subContainers[currentContainer.subContainers.Count - 2]);
		}
		else {
			currentContainer.subContainers.Last().Initialize(currentContainer, currentSource.id);
		}
	}
	
	void ShowAudioSource(AudioPlayer.SubContainer source, AudioPlayer.Container container, SerializedProperty containerProperty){
		if (source.audioSource == null) AdjustName("Audio Source: null", source, container);
		else AdjustName("Audio Source: " + source.audioSource.clip.name, source, container);
		
		if (source.showing){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(source, container);
			source.audioSource = (AudioSource) EditorGUILayout.ObjectField("Audio Source", source.audioSource, typeof(AudioSource), true);
			ShowGeneralSourceSettings(source, container);
			
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowSampler(AudioPlayer.SubContainer source, AudioPlayer.Container container, SerializedProperty containerProperty){
		
		if (string.IsNullOrEmpty(source.instrumentName)) AdjustName("Sampler: null", source, container);
		else AdjustName("Sampler: " + source.instrumentName + " " + source.midiNote + "/" + source.velocity, source, container);
			
		if (source.showing){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(source, container);
			
			string[] displayedOptions = new string[0];
			if (Sampler.Instance != null) if (Sampler.Instance.instruments != null){
				displayedOptions = new string[Sampler.Instance.instruments.Length];
				for (int i = 0; i < Sampler.Instance.instruments.Length; i++){
					displayedOptions[i] = Sampler.Instance.instruments[i].name;
				}
			}
			
			if (displayedOptions.Length > 0){
				source.instrumentIndex = Mathf.Min(EditorGUILayout.Popup("Instrument", source.instrumentIndex, displayedOptions), Sampler.Instance.instruments.Length - 1);
				source.instrumentName = Sampler.Instance.instruments[source.instrumentIndex].name;
				source.midiNote = EditorGUILayout.IntSlider("Midi Note", source.midiNote, (int) Sampler.Instance.instruments[source.instrumentIndex].minNote, (int) Sampler.Instance.instruments[source.instrumentIndex].maxNote);
				source.velocity = EditorGUILayout.Slider("Velocity", source.velocity, 0, 127);
				ShowGeneralSourceSettings(source, container);
			}
			else {
				EditorGUILayout.HelpBox("Add Instruments in the Sampler.", MessageType.Info);
				source.instrumentName = "";
			}
			
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowMixContainer(AudioPlayer.SubContainer source, AudioPlayer.Container container, SerializedProperty containerProperty){
		AdjustName("Mix Container", source, container);
		
		if (source.showing){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(source, container);
			ShowGeneralSourceSettings(source, container);
			ShowChildrenSources(source, container, containerProperty);
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowRandomContainer(AudioPlayer.SubContainer source, AudioPlayer.Container container, SerializedProperty containerProperty){
		AdjustName("Random Container", source, container);
		
		if (source.showing){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(source, container);
			ShowGeneralSourceSettings(source, container);
			ShowChildrenSources(source, container, containerProperty);
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowSequenceContainer(AudioPlayer.SubContainer source, AudioPlayer.Container container, SerializedProperty containerProperty){
		AdjustName("Sequence Container", source, container);
		
		if (source.showing){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(source, container);
			ShowGeneralSourceSettings(source, container);
			ShowChildrenSources(source, container, containerProperty);
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowGeneralSourceSettings(AudioPlayer.SubContainer source, AudioPlayer.Container container){
		source.delay = Mathf.Max(EditorGUILayout.FloatField("Delay", source.delay), 0);
		source.syncMode = (AudioPlayer.SyncMode) EditorGUILayout.EnumPopup("Sync Mode", source.syncMode);
	}
	
	void AdjustName(string prefix, AudioPlayer.SubContainer source, AudioPlayer.Container container){
		source.name = prefix;
		
		if (source.sourceType == AudioPlayer.SubContainer.ContainerTypes.MixContainer || source.sourceType == AudioPlayer.SubContainer.ContainerTypes.RandomContainer || source.sourceType == AudioPlayer.SubContainer.ContainerTypes.SequenceContainer){
			source.name += " | Sources: " + source.childrenLink.Count;
		}
		
		if (GetParentContainerType(source, container) == AudioPlayer.SubContainer.ContainerTypes.RandomContainer){
			source.name += " | Weight: " + source.weight;
		}
		else if (GetParentContainerType(source, container) == AudioPlayer.SubContainer.ContainerTypes.SequenceContainer){
			source.name += " | Repeat: " + source.repeat;
		}
	}
	
	void ShowSourceParentSettings(AudioPlayer.SubContainer source, AudioPlayer.Container container){
		if (GetParentContainerType(source, container) == AudioPlayer.SubContainer.ContainerTypes.RandomContainer){
			source.weight = Mathf.Max(EditorGUILayout.FloatField("Weight", source.weight), 0);
			EditorGUILayout.Space();
		}
		else if (GetParentContainerType(source, container) == AudioPlayer.SubContainer.ContainerTypes.SequenceContainer){
			source.repeat = Mathf.Max(EditorGUILayout.IntField("Repeat", source.repeat), 1);
			EditorGUILayout.Space();
		}
		source.sourceType = (AudioPlayer.SubContainer.ContainerTypes) EditorGUILayout.EnumPopup(source.sourceType);
	}
	
	AudioPlayer.SubContainer.ContainerTypes GetParentContainerType(AudioPlayer.SubContainer source, AudioPlayer.Container container){
		AudioPlayer.SubContainer.ContainerTypes containerType = AudioPlayer.SubContainer.ContainerTypes.AudioSource;
		
		if (source.parentLink != 0){
			containerType = container.GetSourceWithID(source.parentLink).sourceType;
		}
		else if (container.containerType == AudioPlayer.Container.ContainerTypes.MixContainer){
			containerType = AudioPlayer.SubContainer.ContainerTypes.MixContainer;
		}
		else if (container.containerType == AudioPlayer.Container.ContainerTypes.RandomContainer){
			containerType = AudioPlayer.SubContainer.ContainerTypes.RandomContainer;
		}
		else if (container.containerType == AudioPlayer.Container.ContainerTypes.SequenceContainer){
			containerType = AudioPlayer.SubContainer.ContainerTypes.SequenceContainer;
		}
		
		return containerType;
	}
}
#endif
