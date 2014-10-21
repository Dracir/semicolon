using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioInfo {

		public string Name {
			get {
				return audioOptions.name;
			}
			set {
				audioOptions.name = value;
			}
		}
		
		[SerializeField]
		AudioSource source;
		public AudioSource Source {
			get {
				source = source ?? audioOptions.GetOrAddComponent<AudioSource>();
				return source;
			}
			set {
				source = value;
			}
		}

		public AudioClip Clip {
			get {
				return Source.clip;
			}
			set {
				Source.clip = value;
			}
		}

		public float fadeIn;
		public AnimationCurve fadeInCurve = new AnimationCurve(new []{ new Keyframe(0, 0), new Keyframe(1, 1) });
		public float fadeOut = 0.1F;
		public AnimationCurve fadeOutCurve = new AnimationCurve(new []{ new Keyframe(0, 1), new Keyframe(1, 0) });
		[Range(0, 1)] public float randomVolume;
		[Range(0, 6)] public float randomPitch;
		public bool doNotKill;
		
		public AudioOptions audioOptions;
		public AudioPlayer audioPlayer;
		
		public AudioInfo(float fadeIn, AnimationCurve fadeInCurve, float fadeOut, AnimationCurve fadeOutCurve, float randomVolume, float randomPitch, bool doNotKill, AudioSource source, AudioOptions audioOptions, AudioPlayer audioPlayer) {
			this.fadeIn = fadeIn;
			this.fadeInCurve = fadeInCurve;
			this.fadeOut = fadeOut;
			this.fadeOutCurve = fadeOutCurve;
			this.randomVolume = randomVolume;
			this.randomPitch = randomPitch;
			this.doNotKill = doNotKill;
			this.source = source;
			this.audioOptions = audioOptions;
			this.audioPlayer = audioPlayer;
		}
		
		public AudioInfo(AudioSource source, AudioOptions audioOptions, AudioPlayer audioPlayer) {
			this.source = source;
			this.audioOptions = audioOptions;
			this.audioPlayer = audioPlayer;
		}
		
		public AudioInfo(AudioInfo audioInfo) {
			this.Copy(audioInfo);
		}

		public void ApplyAudioOptions(AudioSource audioSource, params AudioOption[] options) {
			foreach (AudioOption option in options) {
				ApplyAudioOption(option, audioSource);
			}
		}
		
		public void ApplyAudioOption(AudioOption option, AudioSource audioSource) {
			switch (option.type) {
				case AudioOption.OptionTypes.FadeIn:
					fadeIn = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.FadeInCurve:
					fadeInCurve = option.GetValue<AnimationCurve>();
					break;
				case AudioOption.OptionTypes.FadeOut:
					fadeOut = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.FadeOutCurve:
					fadeOutCurve = option.GetValue<AnimationCurve>();
					break;
				case AudioOption.OptionTypes.RandomVolume:
					randomVolume = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.RandomPitch:
					randomPitch = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.DoNotKill:
					doNotKill = option.GetValue<bool>();
					break;
				case AudioOption.OptionTypes.Clip:
					audioSource.clip = option.GetValue<AudioClip>();
					break;
				case AudioOption.OptionTypes.Mute:
					audioSource.mute = option.GetValue<bool>();
					break;
				case AudioOption.OptionTypes.BypassEffects:
					audioSource.bypassEffects = option.GetValue<bool>();
					break;
				case AudioOption.OptionTypes.BypassListenerEffects:
					audioSource.bypassListenerEffects = option.GetValue<bool>();
					break;
				case AudioOption.OptionTypes.BypassReverbZones:
					audioSource.bypassReverbZones = option.GetValue<bool>();
					break;
				case AudioOption.OptionTypes.Loop:
					audioSource.loop = option.GetValue<bool>();
					break;
				case AudioOption.OptionTypes.Priority:
					audioSource.priority = option.GetValue<int>();
					break;
				case AudioOption.OptionTypes.Volume:
					audioSource.volume = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.Pitch:
					audioSource.pitch = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.DopplerLevel:
					audioSource.dopplerLevel = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.VolumeRolloff:
					audioSource.rolloffMode = option.GetValue<AudioRolloffMode>();
					break;
				case AudioOption.OptionTypes.MinDistance:
					audioSource.minDistance = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.PanLevel:
					audioSource.panLevel = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.Spread:
					audioSource.spread = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.MaxDistance:
					audioSource.maxDistance = option.GetValue<float>();
					break;
				case AudioOption.OptionTypes.Pan2D:
					audioSource.pan = option.GetValue<float>();
					break;
			}
		}
	}
}
