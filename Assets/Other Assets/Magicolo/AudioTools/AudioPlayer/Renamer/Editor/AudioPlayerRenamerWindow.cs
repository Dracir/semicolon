using System.Collections.Generic;
using System.Threading;
using Magicolo.EditorTools;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;

namespace Magicolo.AudioTools {
	public class AudioPlayerRenamerWindow : CustomWindowBase {

		string[] tagSeparators = { " ", "_", "-", "+", "*", "~", "|", "!", "?", "#", "±", "@", "£", "µ", "$", "¢", "%", "¤", "¬", "¦", "§", "¶", ";", ":", "'", "`", "¯", "(", ")", "[", "]", "{", "}", "<", ">", "^" };
		string tagSeparator = "_";
		int tagSeparatorIndex = 1;
		string renameName;
		
		string currentCategoryName;
		AudioOptions[] selectedAudioOptions;
		readonly List<AudioClip> selectedAudioClips = new List<AudioClip>();
		List<AudioPlayerRenamerCategory> audioClipCategories = new List<AudioPlayerRenamerCategory>();
		
		#region Presets
		string currentPresetName;
		string currentPreset = "default";
		
		const char presetSeparator = '¢';
		const char presetCategorySeparator = '±';
		const char presetTagSeparator = '£';
		
		Dictionary<string, List<AudioPlayerRenamerCategory>> presets;
		Dictionary<string, List<AudioPlayerRenamerCategory>> Presets {
			get {
				if (presets == null) {
					presets = new Dictionary<string, List<AudioPlayerRenamerCategory>>();
					LoadPresets();
					audioClipCategories = Presets[currentPreset];
				}
				return presets;
			}
		}
		#endregion

		static AudioPlayerRenamerWindow Instance;
		AudioPlayer audioPlayer;
		AudioPlayer AudioPlayer {
			get {
				if (audioPlayer == null) {
					audioPlayer = FindObjectOfType<AudioPlayer>();
				}
				return audioPlayer;
			}
		}
		
		[MenuItem("Magicolo's Tools/Audio Player Renamer")]
		static void CreateSymbolsWindow() {
			if (Instance == null) {
				Instance = (AudioPlayerRenamerWindow)EditorWindow.GetWindow<AudioPlayerRenamerWindow>("Renamer", true);
				Vector2 screenSize = new Vector2(430, 400);
				Instance.position = new Rect(Screen.currentResolution.width / 2 - screenSize.x / 2, Screen.currentResolution.height / 2 - screenSize.y / 2, screenSize.x, screenSize.y);
				Instance.minSize = new Vector2(430, 200);
				Instance.OnSelectionChange();
			}
		}
		
		void OnGUI() {
			Separator();
			ShowGeneralSettings();
			ShowCategories();
			Separator();
			ShowRenameButton();
			Separator();
			ShowWarnings();
		}

		void ShowGeneralSettings() {
			// Preset Label
			EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(position.width - 8));
			EditorGUILayout.LabelField("Preset: ".ToGUIContent(), GUILayout.Width(90));
			string[] presetOptions = Presets.Keys.ToArray();
			System.Array.Sort(presetOptions);
			int currentIndex = System.Array.IndexOf(presetOptions, currentPreset);
			
			// Preset Popup
			EditorGUI.BeginChangeCheck();
			currentIndex = EditorGUILayout.Popup(currentIndex, presetOptions, GUILayout.Width(80));
			currentPreset = presetOptions[Mathf.Clamp(currentIndex, 0, presetOptions.Length - 1)];
			if (EditorGUI.EndChangeCheck()) {
				audioClipCategories = Presets[currentPreset];
			}
			
			// Remove Preset Button
			if (SmallButton("−".ToGUIContent()) && currentPreset != "default") {
				Presets.Remove(currentPreset);
				currentPreset = presetOptions[(currentIndex + 1) % presetOptions.Length];
				audioClipCategories = Presets[currentPreset];
				SavePresets();
			}
			
			// Separator
			GUILayout.Space(4);
			EditorGUILayout.BeginVertical();
			GUILayout.Space(8);
			EditorGUILayout.LabelField(GUIContent.none, new GUIStyle("RL DragHandle"), GUILayout.MinWidth(10), GUILayout.Height(4));
			EditorGUILayout.EndVertical();
			GUILayout.Space(4);
			
			// Add Preset Label
			EditorGUILayout.LabelField("New Preset: ".ToGUIContent(), GUILayout.Width(90));
			currentPresetName = EditorGUILayout.TextField(currentPresetName);
			
			// Add Preset Button
			if (SmallButton("+".ToGUIContent()) && !string.IsNullOrEmpty(currentPresetName)) {
				Presets[currentPresetName] = new List<AudioPlayerRenamerCategory>();
				currentPreset = currentPresetName;
				audioClipCategories = Presets[currentPreset];
				SavePresets();
			}
			EditorGUILayout.EndHorizontal();
			
			// Tag separator
			EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(position.width - 8));
			EditorGUILayout.LabelField("Tag Separator: ".ToGUIContent(), GUILayout.Width(90));
			tagSeparatorIndex = EditorGUILayout.Popup(tagSeparatorIndex, tagSeparators, GUILayout.Width(32));
			tagSeparator = tagSeparators[tagSeparatorIndex];
			EditorGUILayout.EndHorizontal();
			
			// Separator
			Separator();
			
			// Add category button
			EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(position.width - 8));
			EditorGUILayout.LabelField("New Category: ".ToGUIContent(), GUILayout.Width(90));
			currentCategoryName = EditorGUILayout.TextField(currentCategoryName);
			if (SmallButton("+".ToGUIContent()) && !string.IsNullOrEmpty(currentCategoryName)) {
				audioClipCategories.Add(new AudioPlayerRenamerCategory(currentCategoryName));
				SavePresets();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
		
		void ShowCategories() {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			
			foreach (var category in audioClipCategories.ToArray()) {
				Box(EditorGUILayout.BeginVertical());
				
				// Name
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(category.Name, EditorStyles.boldLabel, GUILayout.MinWidth(30));
				if (SmallButton("−".ToGUIContent())) {
					audioClipCategories.Remove(category);
					SavePresets();
					break;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
				
				// Add new tags
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("New Tag: ".ToGUIContent(), GUILayout.Width("New Tag: ".GetWidth(EditorStyles.standardFont)));
				category.tagNameToAdd = EditorGUILayout.TextField(category.tagNameToAdd);
				if (SmallButton("+".ToGUIContent()) && !string.IsNullOrEmpty(category.tagNameToAdd)) {
					category.AddTag(category.tagNameToAdd);
					SavePresets();
				}
				EditorGUILayout.EndHorizontal();
				
				Separator();
				
				// Show tags
				for (int i = 0; i < category.tags.Count; i++) {
					string tag = category.tags[i];
					
					Rect rect = EditorGUILayout.BeginHorizontal();
					GUILayout.Box(GUIContent.none, new GUIStyle(), GUILayout.Height(16));
					
					bool pressed = EditorGUI.Toggle(new Rect(rect.x, rect.y - 3, rect.width - 15, rect.height), category.selectedTagIndex == i, new GUIStyle("minibutton"));
					Rect labelRect = new Rect(rect.x + (rect.width - 15) / 2 - tag.GetWidth(EditorStyles.standardFont) / 2 - 2, rect.y - 3, rect.width - 15, rect.height - 1);
					if (pressed) {
						GUIStyle labelStyle = new GUIStyle("whiteLabel");
						labelStyle.clipping = TextClipping.Overflow;
						labelStyle.fontStyle = FontStyle.BoldAndItalic;
						EditorGUI.LabelField(labelRect, tag.ToGUIContent(), labelStyle);
						category.selectedTagIndex = i;
						category.selectedTag = tag;
					}
					else {
						GUIStyle labelStyle = new GUIStyle("label");
						labelStyle.fontStyle = FontStyle.Italic;
						labelStyle.clipping = TextClipping.Overflow;
						EditorGUI.LabelField(labelRect, tag.ToGUIContent(), labelStyle);
					}
					
					GUIStyle buttonStyle = new GUIStyle("MiniToolbarButtonLeft");
					buttonStyle.clipping = TextClipping.Overflow;
					buttonStyle.fontSize = 10;
					if (GUI.Button(new Rect(rect.x + rect.width - 15, rect.y - 2, 16, 16), "−".ToGUIContent(), buttonStyle)) {
						category.tags.RemoveAt(i);
						category.selectedTagIndex = Mathf.Clamp(category.selectedTagIndex, 0, category.tags.Count - 1);
						category.selectedTag = category.tags.Count > 0 ? category.tags[category.selectedTagIndex] : "";
						SavePresets();
						break;
					}
					EditorGUILayout.EndHorizontal();
				}
				
				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndHorizontal();
		}

		void ShowRenameButton() {
			bool renameButtonActive = true;
			
			foreach (AudioPlayerRenamerCategory category in audioClipCategories) {
				if (string.IsNullOrEmpty(category.selectedTag) || category.tags.Count == 0) {
					renameButtonActive = false;
					break;
				}
			}
			
			EditorGUI.BeginDisabledGroup(!renameButtonActive || AudioPlayer == null || audioClipCategories.Count == 0 || selectedAudioOptions == null || selectedAudioOptions.Length == 0 || selectedAudioOptions.All(t => t == null));
			BuildRenameName();
			if (LargeButton(new GUIContent("Rename To : " + renameName))) {
				Rename();
			}
			EditorGUI.EndDisabledGroup();
		}

		void ShowWarnings() {
			if (AudioPlayer == null) {
				EditorGUILayout.HelpBox("Create an AudioPlayer..", MessageType.Warning);
			}
			if (audioClipCategories.Count == 0) {
				EditorGUILayout.HelpBox("Create at least one category with at least one tag.", MessageType.Warning);
			}
			if (selectedAudioOptions == null || selectedAudioOptions.Length == 0 || selectedAudioOptions.All(t => t == null)) {
				EditorGUILayout.HelpBox("Select a sound object from the AudioPlayer's hierarchy.", MessageType.Warning);
			}
		}
		
		void BuildRenameName() {
			renameName = "";
			
			foreach (AudioPlayerRenamerCategory category in audioClipCategories) {
				renameName += category.selectedTag;
				if (category != audioClipCategories.Last()) {
					renameName += tagSeparator;
				}
			}
		}
		
		void Rename() {
			AudioClip[] audioClips = Resources.LoadAll<AudioClip>("");
			
			foreach (AudioOptions audioOptions in selectedAudioOptions) {
				if (audioOptions == null || audioOptions.Source == null || audioOptions.Source.clip == null) {
					continue;
				}
				AudioClip clip = audioOptions.Source.clip;
				string clipPath = AssetDatabase.GetAssetPath(clip);
				string uniqueName = GetUniqueName(clip, renameName, audioClips);
				if (AssetDatabase.RenameAsset(clipPath, uniqueName) != string.Empty) {
					Debug.LogError(string.Format("Failed to rename {0} to {1}.", clipPath, uniqueName));
					continue;
				}
				audioOptions.Name = uniqueName;
			}
		}

		string GetUniqueName(AudioClip clip, string newName, IList<AudioClip> audioClips) {
			int suffix = 0;
			bool uniqueName = false;
			string currentName = "";
		
			while (!uniqueName) {
				uniqueName = true;
				currentName = newName + "_" + suffix;
			
				foreach (AudioClip audioClip in audioClips) {
					if (audioClip != clip && audioClip.name == currentName) {
						uniqueName = false;
						break;
					}
				}
				suffix += 1;
			}
			return currentName;
		}

		void SavePresets() {
			string presetsData = "";
			string[] keys = Presets.Keys.ToArray();
			
			for (int i = 0; i < keys.Length; i++) {
				string presetName = keys[i];
				presetsData += presetName;
				presetsData += presetCategorySeparator;
				
				for (int j = 0; j < presets[presetName].Count; j++) {
					AudioPlayerRenamerCategory category = presets[presetName][j];
					presetsData += category.Name;
					presetsData += presetTagSeparator;
					
					for (int k = 0; k < category.tags.Count; k++) {
						string tag = category.tags[k];
						presetsData += tag;
						
						if (k != category.tags.Count - 1) {
							presetsData += presetTagSeparator;
						}
					}
					
					if (j != presets[presetName].Count - 1) {
						presetsData += presetCategorySeparator;
					}
				}
				
				if (i != presets.Keys.Count - 1) {
					presetsData += presetSeparator;
				}
			}
			
			EditorPrefs.SetString("AudioPlayerRenamerPresets", presetsData);
		}
		
		void LoadPresets() {
			if (!EditorPrefs.HasKey("AudioPlayerRenamerPresets")) {
				SavePresets();
			}
			
			presets = new Dictionary<string, List<AudioPlayerRenamerCategory>>();
			
			string data = EditorPrefs.GetString("AudioPlayerRenamerPresets", "");
			if (string.IsNullOrEmpty(data)) {
				presets["default"] = new List<AudioPlayerRenamerCategory>() {
					new AudioPlayerRenamerCategory("Material", "Wood", "Metal", "Rock", "Plastic"),
					new AudioPlayerRenamerCategory("Behaviour", "Hit", "Destroy", "Scratch"),
					new AudioPlayerRenamerCategory("Length", "Short", "Medium", "Long"),
				};
				SavePresets();
				return;
			}
			
			string[] presetsData = data.Split(presetSeparator);
			
			foreach (string presetData in presetsData) {
				string[] categoriesData = presetData.Split(presetCategorySeparator);
				string presetName = categoriesData.Pop(out categoriesData);
				presets[presetName] = new List<AudioPlayerRenamerCategory>();
			
				foreach (string categoryData in categoriesData) {
					string[] tagData = categoryData.Split(presetTagSeparator);
					string categoryName = tagData.Pop(out tagData);
					presets[presetName].Add(new AudioPlayerRenamerCategory(categoryName, tagData));
				}
			}
		}
		
		void OnSelectionChange() {
			selectedAudioOptions = Selection.gameObjects != null ? Selection.gameObjects.GetComponents<AudioOptions>() : null;
			
			selectedAudioClips.Clear();
			if (selectedAudioOptions != null && selectedAudioOptions.Length > 0) {
				foreach (AudioOptions audioOptions in selectedAudioOptions) {
					if (audioOptions != null && audioOptions.Source != null && audioOptions.Source.clip != null && !selectedAudioClips.Contains(audioOptions.Source.clip)) {
						selectedAudioClips.Add(audioOptions.Source.clip);
					}
				}
			}
			
			Repaint();
		}
	}
}
