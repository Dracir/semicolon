using UnityEngine;
using System.Collections;

public class GameConstantes : MonoBehaviour {

	public Font statementFont;
	public Material statementMaterial;

	public Color statementColor;
	public Color booleanValueColor;
	public Color integerValueColor;
	public Gradient instructionFlash;
	public float effetTimeOnInstructionSwap;
	
	public Color background;
	
	public static GameConstantes instance;

	public GameConstantes(){
		GameConstantes.instance = this;	
	}

	void Start () {

	}
	
	void Update(){
		Camera.main.camera.backgroundColor = background;
	}
}
