﻿using UnityEngine;

[ExecuteInEditMode]
public class References : MonoBehaviour
{

	public Player player;
	static public Player Player
	{
		get { return Instance.player; }
		set { Instance.player = value; }
	}

	public Camera mainCamera;
	static public Camera MainCamera
	{
		get { return Instance.mainCamera; }
		set { Instance.mainCamera = value; }
	}

	public SmoothFollow2D mainCameraFollow;
	static public SmoothFollow2D MainCameraFollow
	{
		get { return Instance.mainCameraFollow; }
		set { Instance.mainCameraFollow = value; }
	}

	public SpikeManager spikeManager;
	static public SpikeManager SpikeManager
	{
		get { return Instance.spikeManager; }
		set { Instance.spikeManager = value; }
	}

	public Prefabs prefabs;
	public Fonts fonts;

	static References Instance;

	void OnEnable()
	{
		Instance = this;
		if (!Application.isPlaying)
			this.SetExecutionOrder(-50);
		else
			SetObjectPools();
	}

	void SetObjectPools()
	{
		hObjectPool.Instance.Add(Prefabs.Spike);
	}


	[System.Serializable]
	public class Prefabs
	{

		public GameObject spike;
		static public GameObject Spike
		{
			get { return Instance.prefabs.spike; }
			set { Instance.prefabs.spike = value; }
		}
	}

	[System.Serializable]
	public class Fonts
	{

		public Font courrier;
		static public Font Courrier
		{
			get { return Instance.fonts.courrier; }
			set { Instance.fonts.courrier = value; }
		}

		public Font courrierBold;
		static public Font CourrierBold
		{
			get { return Instance.fonts.courrierBold; }
			set { Instance.fonts.courrierBold = value; }
		}

		public Font dark;
		static public Font Dark
		{
			get { return Instance.fonts.dark; }
			set { Instance.fonts.dark = value; }
		}

		public Font lucidiaConsole;
		static public Font LucidiaConsole
		{
			get { return Instance.fonts.lucidiaConsole; }
			set { Instance.fonts.lucidiaConsole = value; }
		}
	}
}