using UnityEngine;
using System.Collections;

public class References : MonoBehaviour {

	public Player player;
	public static Player Player {
		get { return Instance.player; }
		set { Instance.player = value; }
	}
	
	public Camera mainCamera;
	public static Camera MainCamera {
		get { return Instance.mainCamera; }
		set { Instance.mainCamera = value; }
	}
	
	public SmoothFollow2D mainCameraFollow;
	public static SmoothFollow2D MainCameraFollow {
		get { return Instance.mainCameraFollow; }
		set { Instance.mainCameraFollow = value; }
	}
	
	public Fonts fonts;
	
	static References Instance;
	
	void Awake(){
		Instance = this;
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