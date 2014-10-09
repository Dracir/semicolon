using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class PDAudioItemManager : Magicolo.AudioTools.AudioItemManager {
		
		float masterVolume = 100;
		public float MasterVolume {
			get {
				return masterVolume;
			}
			set {
				masterVolume = value;
				pdPlayer.communicator.SendValue("UMasterVolume", masterVolume / 100);
			}
		}
		
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

		public PDModule Play(string moduleName, string soundName, GameObject source = null) {
			PDModule module = GetModule(moduleName, source);
			module.AddAudioItem(GetAudioItem(moduleName, soundName, module.spatializer.Source));
			LimitVoices();
			module.Play();
			pdPlayer.communicator.SendValue("UMasterVolume", masterVolume / 100);
			return module;
		}
		
		public PDModule Play(string moduleName, GameObject source = null) {
			PDModule module = GetModule(moduleName, source);
			module.Play();
			pdPlayer.communicator.SendValue("UMasterVolume", masterVolume / 100);
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
		
		public PDModule GetModule(string moduleName, GameObject source = null) {
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
		
		public AudioItem GetAudioItem(string moduleName, string soundName, GameObject source = null) {
			AudioInfo audioInfo = infoManager.GetAudioInfo(soundName);
			AudioSource audioSource = GetAudioSource(audioInfo, source);
			
			GameObject gameObject = audioSource.gameObject;
			CoroutineHolder coroutineHolder = gameObject.GetOrAddComponent<CoroutineHolder>();
			
			PDGainManager gainManager = gameObject.GetOrAddComponent<PDGainManager>();
			idCounter += 1;
			PDSingleAudioItem audioItem = new PDSingleAudioItem(moduleName + "_" + soundName, idCounter, audioSource, audioInfo, gameObject, coroutineHolder, gainManager, this, pdPlayer);
			
			gainManager.Initialize(audioItem, pdPlayer);
			activeAudioItems.Add(audioItem);
			
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
