using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDAudioItemManager : Magicolo.AudioTools.AudioItemManager {
		
		public PDPlayer pdPlayer;
		
		Dictionary<string, PDModule> modules;
		
		public PDAudioItemManager(PDPlayer pdPlayer)
			: base(pdPlayer.listener, pdPlayer.infoManager, pdPlayer) {
			this.pdPlayer = pdPlayer;
			BuildModulesDict();
		}
		
		public void Initialize() {
			foreach (PDModule module in modules.Values) {
				module.Initialize();
			}
		}
		
		public override void Update() {
			base.Update();
			
			foreach (PDModule module in modules.Values) {
				module.Update();
			}
		}

		public override void UpdateVolume() {
			base.UpdateVolume();
			
			pdPlayer.communicator.SendValue("UMasterVolume", player.audioSettings.masterVolume);
		}
		
		public PDModule Play(string moduleName, string soundName, GameObject source, float delay, SyncMode syncMode, params AudioOption[] audioOptions) {
			PDModule module = GetModule(moduleName, source);
			module.AddAudioItem(GetAudioItem(moduleName, soundName, module.spatializer.Source, audioOptions));
			LimitVoices();
			module.Play(delay, syncMode);
			return module;
		}
		
		public PDModule Play(string moduleName, string soundName, GameObject source, params AudioOption[] audioOptions) {
			PDModule module = GetModule(moduleName, source);
			module.AddAudioItem(GetAudioItem(moduleName, soundName, module.spatializer.Source, audioOptions));
			LimitVoices();
			module.Play();
			return module;
		}
		
		public PDModule Play(string moduleName, GameObject source, float delay, SyncMode syncMode) {
			PDModule module = GetModule(moduleName, source);
			module.Play(delay, syncMode);
			return module;
		}
		
		public PDModule Play(string moduleName, GameObject source) {
			PDModule module = GetModule(moduleName, source);
			module.Play();
			return module;
		}
		
		public void Pause(string moduleName) {
			GetModule(moduleName).Pause();
		}
		
		public void Stop(string moduleName) {
			GetModule(moduleName).Stop();
		}
		
		public float GetVolume(string moduleName) {
			return GetModule(moduleName).GetVolume();
		}
		
		public void SetVolume(string moduleName, float targetVolume, float time) {
			GetModule(moduleName).SetVolume(targetVolume, time);
		}
		
		public void SetVolume(string moduleName, float targetVolume) {
			GetModule(moduleName).SetVolume(targetVolume);
		}
		
		public PDModule GetModule(string moduleName) {
			return modules[moduleName];
		}

		public PDModule GetModule(string moduleName, GameObject source) {
			PDModule module;
			if (modules.ContainsKey(moduleName)) {
				module = modules[moduleName];
				module.spatializer.Source = source ?? module.spatializer.Source;
			}
			else {
				idCounter += 1;
				module = new PDModule(moduleName, idCounter, pdPlayer.editorHelper.defaultModule, this, pdPlayer);
				module.spatializer.Source = source ?? module.spatializer.Source;
				modules[moduleName] = module;
				pdPlayer.editorHelper.modules.Add(new PDEditorModule(module, pdPlayer));
			}
			
			return module;
		}

		public AudioItem GetAudioItem(string moduleName, string soundName, GameObject source, params AudioOption[] audioOptions) {
			AudioInfo audioInfo = infoManager.GetAudioInfo(soundName);
			AudioSource audioSource = GetAudioSource(audioInfo, source, audioOptions);
			
			GameObject gameObject = audioSource.gameObject;
			CoroutineHolder coroutineHolder = gameObject.GetOrAddComponent<CoroutineHolder>();
			
			PDGainManager gainManager = gameObject.GetOrAddComponent<PDGainManager>();
			idCounter += 1;
			PDSingleAudioItem audioItem = new PDSingleAudioItem(moduleName + "_" + soundName, idCounter, audioSource, audioInfo, gameObject, coroutineHolder, gainManager, this, pdPlayer);
			
			gainManager.Initialize(audioItem, pdPlayer);
			audioItem.Update();
			player.metronome.Subscribe(audioItem);
			inactiveAudioItems.Add(audioItem);
			
			return audioItem;
		}
		
		public void BuildModulesDict() {
			modules = new Dictionary<string, PDModule>();
			
			foreach (PDEditorModule editorModule in pdPlayer.editorHelper.modules) {
				idCounter += 1;
				PDModule module = new PDModule(idCounter, editorModule, this, pdPlayer);
				modules[module.Name] = module;
			}
		}
	}
}
