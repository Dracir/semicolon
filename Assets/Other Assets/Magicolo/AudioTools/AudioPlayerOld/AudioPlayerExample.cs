using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioPlayerExample : MonoBehaviour {

	[Range(0, 100)]
	public float DangerRTPC = 100;
	
	AudioSource sound;
	List<AudioSource> sounds;
	
	void OnGUI(){
		
//		GUILayout.BeginHorizontal();
//		GUILayout.Space(5);
//		AudioPlayer.Buses["Piano"] = GUILayout.VerticalSlider(AudioPlayer.Buses["Piano"], 100, 0);
//		GUILayout.Space(5);
//		GUILayout.BeginVertical();
		
		// Plays a sound at position (0, 0, 0); returns the AudioSource
		if (GUILayout.Button(" Play Simple ")){
			sound = AudioPlayerOld.Play("Piano_C1");
		}

		// Plays a sound that follows the specified gameObject
		if (GUILayout.Button(" Play Spatialized ")){
			sound = AudioPlayerOld.Play("Piano_C1", gameObject);
		}
		
		// Plays multiple sounds at once; returns the audioSources as List<AudioSource>
		if (GUILayout.Button(" Play Multiple ")){
			sounds = AudioPlayerOld.Play(new string[3]{"Piano_C3", "Piano_C4", "Piano_C5"});
		}
		
		// Plays a container defined in the AudioPlayer inspector
		// Note that the returned List might be modified at any time by the AudioPlayer; make a copy
		// of it if you need to iterate through.
		if (GUILayout.Button(" Play Container ")){
			sounds = AudioPlayerOld.Containers["Mysterious"].Play();
		}

		// Plays any of the above repeatedly
		if (GUILayout.Button(" Play Repeatedly ")){
			sounds = AudioPlayerOld.PlayRepeating(0.125F, AudioPlayerOld.Containers["Mysterious"], gameObject, 0, AudioPlayerOld.SyncMode.Measure);
		}
		
		// Pauses a sound
		if (GUILayout.Button(" Pause Last Sound ")){
			AudioPlayerOld.Pause(sound);
			AudioPlayerOld.Pause(sounds);
		}
		
		// Pauses all sounds
		if (GUILayout.Button(" Pause All ")){
			AudioPlayerOld.PauseAll();
		}
		
		// Resumes a sound
		if (GUILayout.Button(" Resume Last Sound ")){
			AudioPlayerOld.Resume(sound);
			AudioPlayerOld.Resume(sounds);
		}
		
		// Resumes all sounds
		if (GUILayout.Button(" Resume All ")){
			AudioPlayerOld.ResumeAll();
		}
		
		// Sets the volume of a sound
		if (GUILayout.Button(" Set Last Sounds Volume 25% ")){
			AudioPlayerOld.SetVolume(sound, 25);
			AudioPlayerOld.SetVolume(sounds, 25);
		}
		
		// Sets the volume of a sound with fade
		if (GUILayout.Button(" Set Last Sounds Volume 100% Over 2 Seconds ")){
			AudioPlayerOld.SetVolume(sound, 100, 2);
			AudioPlayerOld.SetVolume(sounds, 100, 2);
		}
		
		// Sets the master volume
		if (GUILayout.Button(" Set Master Volume 100% ")){
			AudioPlayerOld.SetMasterVolume(100);
		}
		
		// Sets the master volume with fade
		if (GUILayout.Button(" Set Master Volume 25% Over 2 Seconds ")){
			AudioPlayerOld.SetMasterVolume(25, 2);
		}
		
		// Stops a sound with fade out
		if (GUILayout.Button(" Stop Last Sound With Fade Out ")){
			AudioPlayerOld.Stop(sound);
			AudioPlayerOld.Stop(sounds);
		}
		
		// Stops all sounds without fade out
		if (GUILayout.Button(" Stop All Without Fade Out ")){
			AudioPlayerOld.StopAllImmediate();
		}
		
//		GUILayout.EndVertical();
//		GUILayout.EndHorizontal();
	}
}
