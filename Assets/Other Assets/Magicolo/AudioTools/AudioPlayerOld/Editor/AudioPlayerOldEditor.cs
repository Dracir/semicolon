#if UNITY_EDITOR
using Magicolo.EditorTools;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(AudioPlayerOld))]
public class AudioPlayerOldEditor : CustomEditorBase {

	AudioPlayerOld audioPlayer;
	AudioPlayerOld.Container currentContainer;
	AudioPlayerOld.SubContainer currentSource;
	SerializedProperty currentSourceProperty;

	public override void OnInspectorGUI(){
		audioPlayer = (AudioPlayerOld) target;
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
		
		Sampler sampler = FindObjectOfType<Sampler>();
		PDPlayer pdPlayer = FindObjectOfType<PDPlayer>();
		if (sampler == null || pdPlayer == null){
			Separator();
			if (sampler == null){
				if (LargeAddElementButton("Add Sampler".ToGUIContent()))
					audioPlayer.gameObject.AddComponent<Sampler>();
			}
			if (pdPlayer == null){
				if (LargeAddElementButton("Add PDPlayer".ToGUIContent()))
					audioPlayer.gameObject.AddComponent<PDPlayer>();
			}
		}
		
		End();
	}
	
	void ShowRTPCs(){
		if (audioPlayer.rTPCs == null) return;
		
		SerializedProperty rTPCsProperty = serializedObject.FindProperty("rTPCs");
		if (AddElementFoldOut(rTPCsProperty, "RTPCs".ToGUIContent())){
			audioPlayer.rTPCs[audioPlayer.rTPCs.Length - 1] = new AudioPlayerOld.RTPC();
			audioPlayer.rTPCs[audioPlayer.rTPCs.Length - 1].name = AudioPlayerOld.GetUniqueName(audioPlayer.rTPCs, "default");	
		}
		
		if (rTPCsProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			
			for (int i = 0; i < audioPlayer.rTPCs.Length; i++){
				AudioPlayerOld.RTPC rtpc = audioPlayer.rTPCs[i];
				SerializedProperty rtpcProperty = rTPCsProperty.GetArrayElementAtIndex(i);
				
				EditorGUILayout.BeginHorizontal();
				rtpc.showing = EditorGUILayout.Foldout(rtpc.showing, rtpc.name);
				GUILayout.Space(30);
				if (!rtpc.showing) rtpc.defaultValue = EditorGUILayout.Slider(rtpc.defaultValue, rtpc.minValue, rtpc.maxValue);
				GUILayout.Space(10);
				DeleteElementButtonWithArrows(rTPCsProperty, i);
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
	
	void ShowBuses(){
		if (audioPlayer.buses == null) return;
		
		SerializedProperty busesProperty = serializedObject.FindProperty("buses");
		if (AddElementFoldOut(busesProperty, "Buses".ToGUIContent())){
			audioPlayer.buses[audioPlayer.buses.Length - 1] = new AudioPlayerOld.AudioBus();
			audioPlayer.buses[audioPlayer.buses.Length - 1].name = AudioPlayerOld.GetUniqueName(audioPlayer.buses, "default");
		}
		
		if (busesProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			
			for (int i = 0; i < audioPlayer.buses.Length; i++){
				AudioPlayerOld.AudioBus bus = audioPlayer.buses[i];
				SerializedProperty busProperty = busesProperty.GetArrayElementAtIndex(i);
				
				EditorGUILayout.BeginHorizontal();
				bus.showing = EditorGUILayout.Foldout(bus.showing, bus.name);
				GUILayout.Space(30);
				if (!bus.showing) bus.volume = EditorGUILayout.Slider(bus.volume, 0, 100);
				GUILayout.Space(10);
				DeleteElementButtonWithArrows(busesProperty, i);
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
	
	void ShowContainers(){
		if (audioPlayer.containers == null)	return;
		
		SerializedProperty containersProperty = serializedObject.FindProperty("containers");
		if (AddElementFoldOut(containersProperty, "Containers".ToGUIContent())){
			audioPlayer.containers[audioPlayer.containers.Length - 1] = new AudioPlayerOld.Container();
			audioPlayer.containers[audioPlayer.containers.Length - 1].name = AudioPlayerOld.GetUniqueName(audioPlayer.containers, "default");
			audioPlayer.containers[audioPlayer.containers.Length - 1].subContainers = new List<AudioPlayerOld.SubContainer>();
		}
		
		if (containersProperty.isExpanded) {
			EditorGUI.indentLevel += 1;
			
			for (int i = 0; i < audioPlayer.containers.Length; i++) {
				AudioPlayerOld.Container container = audioPlayer.containers[i];
				currentContainer = container;
				SerializedProperty containerProperty = containersProperty.GetArrayElementAtIndex(i);
				
				if (DeleteElementFoldOutWithArrows(containersProperty, i, container.name.ToGUIContent())){
					break;
				}
				
				if (container.showing) {
					EditorGUI.indentLevel += 1;
					
					EditorGUI.BeginDisabledGroup(Application.isPlaying);
					container.name = EditorGUILayout.TextField(container.name);
					EditorGUI.EndDisabledGroup();
					container.containerType = (AudioPlayerOld.Container.ContainerTypes)EditorGUILayout.EnumPopup(container.containerType);
					ShowSources(container, containerProperty);
					
					EditorGUI.indentLevel -= 1;
				}
			}
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowSources(AudioPlayerOld.Container container, SerializedProperty containerProperty){
		SerializedProperty sourcesProperty = containerProperty.FindPropertyRelative("sources");
		if (AddElementFoldOut(sourcesProperty, "Sources".ToGUIContent())){
			currentContainer.sources[currentContainer.sources.Length - 1] = new AudioPlayerOld.SubContainer();
			if (currentContainer.sources.Length > 1) currentContainer.sources[currentContainer.sources.Length - 1].Initialize(currentContainer, 0, currentContainer.sources[currentContainer.sources.Length - 2]);
			else currentContainer.sources[currentContainer.sources.Length - 1].Initialize(currentContainer, 0);
		}
		
		if (sourcesProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			
			if (container.sources != null){
				for (int i = 0; i < container.sources.Length; i++){
					currentSource = container.sources[i];
					currentSourceProperty = sourcesProperty.GetArrayElementAtIndex(i);
						
					if (DeleteElementFoldOut(sourcesProperty, i, currentSource.name.ToGUIContent())){
						container.RemoveEmptyReferences();
						break;
					}
					
					switch (currentSource.sourceType) {
						case AudioPlayerOld.SubContainer.ContainerTypes.AudioSource:
							ShowAudioSource(container, containerProperty);
							break;
						case AudioPlayerOld.SubContainer.ContainerTypes.Sampler:
							ShowSampler(container, containerProperty);
							break;
						case AudioPlayerOld.SubContainer.ContainerTypes.MixContainer:
							ShowMixContainer(container, containerProperty);
							break;
						case AudioPlayerOld.SubContainer.ContainerTypes.RandomContainer:
							ShowRandomContainer(container, containerProperty);
							break;
						case AudioPlayerOld.SubContainer.ContainerTypes.SequenceContainer:
							ShowSequenceContainer(container, containerProperty);
							break;
					}
				}
			}
			EditorGUI.indentLevel -= 1;
		}
		Separator();
	}
	
	void ShowChildrenSources(AudioPlayerOld.SubContainer source, AudioPlayerOld.Container container, SerializedProperty containerProperty){
		SerializedProperty subContainersProperty = containerProperty.FindPropertyRelative("subContainers");
		if (AddElementFoldOut(subContainersProperty, "Sources".ToGUIContent(), source.childrenLink.Count)){
			currentContainer.subContainers[currentContainer.subContainers.Count - 1] = new AudioPlayerOld.SubContainer();
			if (currentContainer.subContainers.Count > 1) {
				currentContainer.subContainers.Last().Initialize(currentContainer, currentSource.id, currentContainer.subContainers[currentContainer.subContainers.Count - 2]);
			}
			else {
				currentContainer.subContainers.Last().Initialize(currentContainer, currentSource.id);
			}
		}
		
		if (subContainersProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			
			if (source.childrenLink.Count != 0){
				for (int i = 0; i < source.childrenLink.Count; i++){
					currentSource = container.GetSourceWithID(source.childrenLink[i]);
					currentSourceProperty = subContainersProperty.GetArrayElementAtIndex(i);
					int index = container.subContainers.IndexOf(currentSource);
					
					if (DeleteElementFoldOut(subContainersProperty, index, currentSource.name.ToGUIContent())){
						container.GetSourceWithID(source.id).childrenLink.Remove(currentSource.id);
						container.RemoveEmptyReferences();
						break;
					}
					
					switch (currentSource.sourceType) {
						case AudioPlayerOld.SubContainer.ContainerTypes.AudioSource:
							ShowAudioSource(container, containerProperty);
							break;
						case AudioPlayerOld.SubContainer.ContainerTypes.Sampler:
							ShowSampler(container, containerProperty);
							break;
						case AudioPlayerOld.SubContainer.ContainerTypes.MixContainer:
							ShowMixContainer(container, containerProperty);
							break;
						case AudioPlayerOld.SubContainer.ContainerTypes.RandomContainer:
							ShowRandomContainer(container, containerProperty);
							break;
						case AudioPlayerOld.SubContainer.ContainerTypes.SequenceContainer:
							ShowSequenceContainer(container, containerProperty);
							break;
					}
				}
			}
			EditorGUI.indentLevel -= 1;
		}
		Separator();
	}
	
	void ShowAudioSource(AudioPlayerOld.Container container, SerializedProperty containerProperty){
		if (currentSource.audioSource == null) AdjustName("Audio Source: null", currentSource, container);
		else AdjustName("Audio Source: " + currentSource.audioSource.clip.name, currentSource, container);
		
		if (currentSourceProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(currentSource, container);
			currentSource.audioSource = (AudioSource) EditorGUILayout.ObjectField("Audio Source", currentSource.audioSource, typeof(AudioSource), true);
			ShowGeneralSourceSettings(currentSource, container);
			
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowSampler(AudioPlayerOld.Container container, SerializedProperty containerProperty){
		
		if (string.IsNullOrEmpty(currentSource.instrumentName)) AdjustName("Sampler: null", currentSource, container);
		else AdjustName("Sampler: " + currentSource.instrumentName + " " + currentSource.midiNote + "/" + currentSource.velocity, currentSource, container);
			
		if (currentSourceProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(currentSource, container);
			
			string[] displayedOptions = new string[0];
			if (Sampler.Instance != null) if (Sampler.Instance.instruments != null){
				displayedOptions = new string[Sampler.Instance.instruments.Length];
				for (int i = 0; i < Sampler.Instance.instruments.Length; i++){
					displayedOptions[i] = Sampler.Instance.instruments[i].name;
				}
			}
			
			if (displayedOptions.Length > 0){
				currentSource.instrumentIndex = Mathf.Min(EditorGUILayout.Popup("Instrument", currentSource.instrumentIndex, displayedOptions), Sampler.Instance.instruments.Length - 1);
				currentSource.instrumentName = Sampler.Instance.instruments[currentSource.instrumentIndex].name;
				currentSource.midiNote = EditorGUILayout.IntSlider("Midi Note", currentSource.midiNote, (int) Sampler.Instance.instruments[currentSource.instrumentIndex].minNote, (int) Sampler.Instance.instruments[currentSource.instrumentIndex].maxNote);
				currentSource.velocity = EditorGUILayout.Slider("Velocity", currentSource.velocity, 0, 127);
				ShowGeneralSourceSettings(currentSource, container);
			}
			else {
				EditorGUILayout.HelpBox("Add Instruments in the Sampler.", MessageType.Info);
				currentSource.instrumentName = "";
			}
			
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowMixContainer(AudioPlayerOld.Container container, SerializedProperty containerProperty){
		AdjustName("Mix Container", currentSource, container);
		
		if (currentSourceProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(currentSource, container);
			ShowGeneralSourceSettings(currentSource, container);
			ShowChildrenSources(currentSource, container, containerProperty);
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowRandomContainer(AudioPlayerOld.Container container, SerializedProperty containerProperty){
		AdjustName("Random Container", currentSource, container);
		
		if (currentSourceProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(currentSource, container);
			ShowGeneralSourceSettings(currentSource, container);
			ShowChildrenSources(currentSource, container, containerProperty);
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowSequenceContainer(AudioPlayerOld.Container container, SerializedProperty containerProperty){
		AdjustName("Sequence Container", currentSource, container);
		
		if (currentSourceProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(currentSource, container);
			ShowGeneralSourceSettings(currentSource, container);
			ShowChildrenSources(currentSource, container, containerProperty);
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowGeneralSourceSettings(AudioPlayerOld.SubContainer source, AudioPlayerOld.Container container){
		source.delay = Mathf.Max(EditorGUILayout.FloatField("Delay", source.delay), 0);
		source.syncMode = (AudioPlayerOld.SyncMode) EditorGUILayout.EnumPopup("Sync Mode", source.syncMode);
	}
	
	void AdjustName(string prefix, AudioPlayerOld.SubContainer source, AudioPlayerOld.Container container){
		source.name = prefix;
		
		if (source.sourceType == AudioPlayerOld.SubContainer.ContainerTypes.MixContainer || source.sourceType == AudioPlayerOld.SubContainer.ContainerTypes.RandomContainer || source.sourceType == AudioPlayerOld.SubContainer.ContainerTypes.SequenceContainer){
			source.name += " | Sources: " + source.childrenLink.Count;
		}
		
		if (GetParentContainerType(source, container) == AudioPlayerOld.SubContainer.ContainerTypes.RandomContainer){
			source.name += " | Weight: " + source.weight;
		}
		else if (GetParentContainerType(source, container) == AudioPlayerOld.SubContainer.ContainerTypes.SequenceContainer){
			source.name += " | Repeat: " + source.repeat;
		}
	}
	
	void ShowSourceParentSettings(AudioPlayerOld.SubContainer source, AudioPlayerOld.Container container){
		if (GetParentContainerType(source, container) == AudioPlayerOld.SubContainer.ContainerTypes.RandomContainer){
			source.weight = Mathf.Max(EditorGUILayout.FloatField("Weight", source.weight), 0);
			EditorGUILayout.Space();
		}
		else if (GetParentContainerType(source, container) == AudioPlayerOld.SubContainer.ContainerTypes.SequenceContainer){
			source.repeat = Mathf.Max(EditorGUILayout.IntField("Repeat", source.repeat), 1);
			EditorGUILayout.Space();
		}
		source.sourceType = (AudioPlayerOld.SubContainer.ContainerTypes) EditorGUILayout.EnumPopup(source.sourceType);
	}
	
	AudioPlayerOld.SubContainer.ContainerTypes GetParentContainerType(AudioPlayerOld.SubContainer source, AudioPlayerOld.Container container){
		AudioPlayerOld.SubContainer.ContainerTypes containerType = AudioPlayerOld.SubContainer.ContainerTypes.AudioSource;
		
		if (source.parentLink != 0){
			containerType = container.GetSourceWithID(source.parentLink).sourceType;
		}
		else if (container.containerType == AudioPlayerOld.Container.ContainerTypes.MixContainer){
			containerType = AudioPlayerOld.SubContainer.ContainerTypes.MixContainer;
		}
		else if (container.containerType == AudioPlayerOld.Container.ContainerTypes.RandomContainer){
			containerType = AudioPlayerOld.SubContainer.ContainerTypes.RandomContainer;
		}
		else if (container.containerType == AudioPlayerOld.Container.ContainerTypes.SequenceContainer){
			containerType = AudioPlayerOld.SubContainer.ContainerTypes.SequenceContainer;
		}
		
		return containerType;
	}
}
#endif
