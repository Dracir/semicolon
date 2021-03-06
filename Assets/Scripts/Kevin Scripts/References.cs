﻿using UnityEngine;
using System.Collections.Generic;
using Magicolo.AudioTools;
using Magicolo.GeneralTools;

[ExecuteInEditMode]
public class References : MonoBehaviour {

	public Semicolon player;
	static public Semicolon Player {
		get { return Instance.player; }
		set { Instance.player = value; }
	}

	public Camera mainCamera;
	static public Camera MainCamera {
		get { return Instance.mainCamera; }
		set { Instance.mainCamera = value; }
	}

	public SmoothFollow2D mainCameraFollow;
	static public SmoothFollow2D MainCameraFollow {
		get { return Instance.mainCameraFollow; }
		set { Instance.mainCameraFollow = value; }
	}

	public Metronome metronome;
	static public Metronome Metronome {
		get { 
			if (Instance.metronome == null) {
				AudioPlayer audioPlayer = FindObjectOfType<AudioPlayer>();
				Instance.metronome = audioPlayer.GetComponent<Metronome>();
			}
			return Instance.metronome; 
		}
		set { Instance.metronome = value; }
	}

	public Prefabs prefabs;
	public Fonts fonts;

	static References instance;
	static References Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<References>();
			}
			return instance;
		}
	}

	
	[System.Serializable]
	public class Prefabs {

		public GameObject spike;
		static public GameObject Spike {
			get { return Instance.prefabs.spike; }
			set { Instance.prefabs.spike = value; }
		}
		
		public GameObject arcadeManager;
		static public GameObject ArcadeManager {
			get { return Instance.prefabs.arcadeManager; }
			set { Instance.prefabs.arcadeManager = value; }
		}
		public GameObject arcadeCanevas;
		static public GameObject ArcadeLoseScreen {
			get { return Instance.prefabs.arcadeCanevas; }
			set { Instance.prefabs.arcadeCanevas = value; }
		}
		public GameObject audioPrefab;
		static public GameObject AudioPrefab {
			get { return Instance.prefabs.audioPrefab; }
			set { Instance.prefabs.audioPrefab = value; }
		}
		public GameObject playerPrefab;
		static public GameObject PlayerPrefab {
			get { return Instance.prefabs.playerPrefab; }
			set { Instance.prefabs.playerPrefab = value; }
		}
		public GameObject mainCameraPrefab;
		static public GameObject MainCameraPrefab {
			get { return Instance.prefabs.mainCameraPrefab; }
			set { Instance.prefabs.mainCameraPrefab = value; }
		}
	}

	[System.Serializable]
	public class Fonts {

		public Font courrier;
		static public Font Courrier {
			get { return Instance.fonts.courrier; }
			set { Instance.fonts.courrier = value; }
		}

		public Font courrierBold;
		static public Font CourrierBold {
			get { return Instance.fonts.courrierBold; }
			set { Instance.fonts.courrierBold = value; }
		}

		public Font dark;
		static public Font Dark {
			get { return Instance.fonts.dark; }
			set { Instance.fonts.dark = value; }
		}

		public Font lucidiaConsole;
		static public Font LucidiaConsole {
			get { return Instance.fonts.lucidiaConsole; }
			set { Instance.fonts.lucidiaConsole = value; }
		}
	}
	
}