using UnityEngine;
using System.Collections;

public class GameConstantes : MonoBehaviour {

	public Font statementFont;
	public Material statementMaterial;

	public Color statementColor;
	public Color booleanValueColor;
	public Color integerValueColor;

	public static GameConstantes instance;

	public GameConstantes(){
		GameConstantes.instance = this;	
	}

	void Start () {

	}
}
