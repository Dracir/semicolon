﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameCodeTheme  {

	public Font instructionFont;
	public Material instructionMaterial;

	public Color instructionColor;
	public Color commentColor;
	public Color booleanValueColor;
	public Color integerValueColor;
	
	public Gradient instructionFlash;
	public float effetTimeOnInstructionSwap;
	
	public Color background;
}
