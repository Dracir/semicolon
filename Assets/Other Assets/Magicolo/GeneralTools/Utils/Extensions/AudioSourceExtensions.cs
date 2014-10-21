using UnityEngine;
using System.Collections;

public static class AudioSourceExtensions {

	public static AudioSource PlayOnListener(this AudioSource audioSource){
		if (audioSource == null || audioSource.clip == null) {
			return null;
		}
		
		AudioListener listener = Object.FindObjectOfType<AudioListener>();
		if (listener == null){
			Debug.LogError("No listener was found in the scene.");
			return null;
		}
		
		GameObject gameObject = new GameObject(audioSource.clip.name);
		AudioSource source = gameObject.AddCopiedComponent<AudioSource>(audioSource);
		gameObject.transform.parent = listener.transform;
		gameObject.transform.Reset();
		source.Play();
		
		return source;
	}
}
