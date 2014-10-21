using UnityEngine;
using System.Collections;

[System.Serializable]
public class AudioOption {

	public enum OptionTypes {
		FadeIn,
		FadeInCurve,
		FadeOut,
		FadeOutCurve,
		RandomVolume,
		RandomPitch,
		DoNotKill,
		Clip,
		Mute,
		BypassEffects,
		BypassListenerEffects,
		BypassReverbZones,
		Loop,
		Priority,
		Volume,
		Pitch,
		DopplerLevel,
		VolumeRolloff,
		MinDistance,
		PanLevel,
		Spread,
		MaxDistance,
		Pan2D
	}
	
	public OptionTypes type;
	public object value;
	
	AudioOption(OptionTypes type, object value) {
		this.type = type;
		this.value = value;
	}
	
	public static AudioOption FadeIn(float fadeIn) {
		return new AudioOption(OptionTypes.FadeIn, fadeIn);
	}
	
	public static AudioOption FadeInCurve(AnimationCurve fadeInCurve) {
		return new AudioOption(OptionTypes.FadeInCurve, fadeInCurve);
	}
	
	public static AudioOption FadeOut(float fadeOut) {
		return new AudioOption(OptionTypes.FadeOut, fadeOut);
	}
	
	public static AudioOption FadeOutCurve(AnimationCurve fadeOutCurve) {
		return new AudioOption(OptionTypes.FadeOutCurve, fadeOutCurve);
	}
	
	public static AudioOption RandomVolume(float randomVolume) {
		return new AudioOption(OptionTypes.RandomVolume, randomVolume);
	}
	
	public static AudioOption RandomPitch(float randomPitch) {
		return new AudioOption(OptionTypes.RandomPitch, randomPitch);
	}
	
	public static AudioOption DoNotKill(bool doNotKill) {
		return new AudioOption(OptionTypes.DoNotKill, doNotKill);
	}
	
	public static AudioOption Clip(AudioClip clip) {
		return new AudioOption(OptionTypes.Clip, clip);
	}
	
	public static AudioOption Mute(bool mute) {
		return new AudioOption(OptionTypes.Mute, mute);
	}
	
	public static AudioOption BypassEffects(bool bypassEffects) {
		return new AudioOption(OptionTypes.BypassEffects, bypassEffects);
	}
	
	public static AudioOption BypassListenerEffects(bool bypassListenerEffects) {
		return new AudioOption(OptionTypes.BypassListenerEffects, bypassListenerEffects);
	}
	
	public static AudioOption BypassReverbZones(bool bypassReverbZones) {
		return new AudioOption(OptionTypes.BypassReverbZones, bypassReverbZones);
	}
	
	public static AudioOption Loop(bool loop) {
		return new AudioOption(OptionTypes.Loop, loop);
	}
	
	public static AudioOption Priority(int priority) {
		return new AudioOption(OptionTypes.Priority, priority);
	}
	
	public static AudioOption Volume(float volume) {
		return new AudioOption(OptionTypes.Volume, volume);
	}
	
	public static AudioOption Pitch(float pitch) {
		return new AudioOption(OptionTypes.Pitch, pitch);
	}
	
	public static AudioOption DopplerLevel(float dopplerLevel) {
		return new AudioOption(OptionTypes.DopplerLevel, dopplerLevel);
	}
	
	public static AudioOption VolumeRolloff(AudioRolloffMode volumeRolloff) {
		return new AudioOption(OptionTypes.VolumeRolloff, volumeRolloff);
	}
	
	public static AudioOption MinDistance(float minDistance) {
		return new AudioOption(OptionTypes.MinDistance, minDistance);
	}
	
	public static AudioOption PanLevel(float panLevel) {
		return new AudioOption(OptionTypes.PanLevel, panLevel);
	}
	
	public static AudioOption Spread(float spread) {
		return new AudioOption(OptionTypes.Spread, spread);
	}
	
	public static AudioOption MaxDistance(float maxDistance) {
		return new AudioOption(OptionTypes.MaxDistance, maxDistance);
	}
	
	public static AudioOption Pan2D(float pan2D) {
		return new AudioOption(OptionTypes.Pan2D, pan2D);
	}
	
	public T GetValue<T>() {
		return (T)value;
	}
}
