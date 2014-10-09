using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	public class Player : MonoBehaviour {

		[Range(1, 64)] public int maxVoices = 32;
		public CoroutineHolder coroutineHolder;
		public AudioListener listener;

		protected virtual void Awake() {
			if (Application.isPlaying) {
				coroutineHolder = gameObject.GetOrAddComponent<CoroutineHolder>();
				listener = FindObjectOfType<AudioListener>();
				if (listener == null) {
					Debug.LogError("There are no audio listeners in the scene.");
					enabled = false;
					return;
				}
			}
		}
	}
}
