#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(AudioPlayer))]
public class AudioPlayerEditor : CustomEditorBase {

	AudioPlayer audioPlayer;
	AudioPlayer.Container currentContainer;
	AudioPlayer.SubContainer currentSource;
	SerializedProperty currentSourceProperty;
	
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
			audioPlayer.rTPCs[audioPlayer.rTPCs.Length - 1] = new AudioPlayer.RTPC();
			audioPlayer.rTPCs[audioPlayer.rTPCs.Length - 1].name = AudioPlayer.GetUniqueName(audioPlayer.rTPCs, "default");	
		}
		
		if (rTPCsProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			
			for (int i = 0; i < audioPlayer.rTPCs.Length; i++){
				AudioPlayer.RTPC rtpc = audioPlayer.rTPCs[i];
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
			audioPlayer.buses[audioPlayer.buses.Length - 1] = new AudioPlayer.AudioBus();
			audioPlayer.buses[audioPlayer.buses.Length - 1].name = AudioPlayer.GetUniqueName(audioPlayer.buses, "default");
		}
		
		if (busesProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			
			for (int i = 0; i < audioPlayer.buses.Length; i++){
				AudioPlayer.AudioBus bus = audioPlayer.buses[i];
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
			audioPlayer.containers[audioPlayer.containers.Length - 1] = new AudioPlayer.Container();
			audioPlayer.containers[audioPlayer.containers.Length - 1].name = AudioPlayer.GetUniqueName(audioPlayer.containers, "default");
			audioPlayer.containers[audioPlayer.containers.Length - 1].subContainers = new List<AudioPlayer.SubContainer>();
		}
		
		if (containersProperty.isExpanded) {
			EditorGUI.indentLevel += 1;
			
			for (int i = 0; i < audioPlayer.containers.Length; i++) {
				AudioPlayer.Container container = audioPlayer.containers[i];
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
					container.containerType = (AudioPlayer.Container.ContainerTypes)EditorGUILayout.EnumPopup(container.containerType);
					ShowSources(container, containerProperty);
					
					EditorGUI.indentLevel -= 1;
				}
			}
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowSources(AudioPlayer.Container container, SerializedProperty containerProperty){
		SerializedProperty sourcesProperty = containerProperty.FindPropertyRelative("sources");
		if (AddElementFoldOut(sourcesProperty, "Sources".ToGUIContent())){
			currentContainer.sources[currentContainer.sources.Length - 1] = new AudioPlayer.SubContainer();
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
						case AudioPlayer.SubContainer.ContainerTypes.AudioSource:
							ShowAudioSource(container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.Sampler:
							ShowSampler(container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.MixContainer:
							ShowMixContainer(container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.RandomContainer:
							ShowRandomContainer(container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.SequenceContainer:
							ShowSequenceContainer(container, containerProperty);
							break;
					}
				}
			}
			EditorGUI.indentLevel -= 1;
		}
		Separator();
	}
	
	void ShowChildrenSources(AudioPlayer.SubContainer source, AudioPlayer.Container container, SerializedProperty containerProperty){
		SerializedProperty subContainersProperty = containerProperty.FindPropertyRelative("subContainers");
		if (AddElementFoldOut(subContainersProperty, "Sources".ToGUIContent(), source.childrenLink.Count)){
			currentContainer.subContainers[currentContainer.subContainers.Count - 1] = new AudioPlayer.SubContainer();
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
						case AudioPlayer.SubContainer.ContainerTypes.AudioSource:
							ShowAudioSource(container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.Sampler:
							ShowSampler(container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.MixContainer:
							ShowMixContainer(container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.RandomContainer:
							ShowRandomContainer(container, containerProperty);
							break;
						case AudioPlayer.SubContainer.ContainerTypes.SequenceContainer:
							ShowSequenceContainer(container, containerProperty);
							break;
					}
				}
			}
			EditorGUI.indentLevel -= 1;
		}
		Separator();
	}
	
	void ShowAudioSource(AudioPlayer.Container container, SerializedProperty containerProperty){
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
	
	void ShowSampler(AudioPlayer.Container container, SerializedProperty containerProperty){
		
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
	
	void ShowMixContainer(AudioPlayer.Container container, SerializedProperty containerProperty){
		AdjustName("Mix Container", currentSource, container);
		
		if (currentSourceProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(currentSource, container);
			ShowGeneralSourceSettings(currentSource, container);
			ShowChildrenSources(currentSource, container, containerProperty);
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowRandomContainer(AudioPlayer.Container container, SerializedProperty containerProperty){
		AdjustName("Random Container", currentSource, container);
		
		if (currentSourceProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(currentSource, container);
			ShowGeneralSourceSettings(currentSource, container);
			ShowChildrenSources(currentSource, container, containerProperty);
			EditorGUI.indentLevel -= 1;
		}
	}
	
	void ShowSequenceContainer(AudioPlayer.Container container, SerializedProperty containerProperty){
		AdjustName("Sequence Container", currentSource, container);
		
		if (currentSourceProperty.isExpanded){
			EditorGUI.indentLevel += 1;
			ShowSourceParentSettings(currentSource, container);
			ShowGeneralSourceSettings(currentSource, container);
			ShowChildrenSources(currentSource, container, containerProperty);
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
