﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameCodeTheme  {

	public Font instructionFont;
	public Material instructionMaterial;

	public Color instructionColor;
	public Color commentColor;
	public Color compileSpotColor;
	public Color booleanValueColor;
	public Color integerValueColor;
	public Gradient booleanValueHighlightedGradient;
	public Gradient integerValueHighlightedGradient;
	public float parameterHighlightTime;
	
	public Gradient instructionFlash;
	public float timeOnInstructionSwap;
	
	public Gradient timeGoingUnder10Gradient;
	public float timeGoingUnder10Time;
	
	public Color background;
	
	public Effect createInstructionFlashEffect(TextCollider2D collider, bool removeAfterEffect){
		return new GradientEffet(collider , this.instructionFlash, this.timeOnInstructionSwap, removeAfterEffect);
	}
	
	public Effect createParameterHighlightEffect(Parameter parameter){
		TextCollider2D tc2d = parameter.GetComponent<TextCollider2D>();
		Gradient gradient = parameter.getHighLightedGradient();
		return new GradientEffet(tc2d, gradient, parameterHighlightTime, false);
	}
	
	public Effect createTimeGoingUnder10Gradient(TextCollider2D collider){
		return new GradientEffet(collider , this.timeGoingUnder10Gradient, this.timeGoingUnder10Time, false);
	}
}
