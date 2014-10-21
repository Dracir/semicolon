using UnityEngine;
using System.Collections;

public static class FloatExtensions {

	public static float Pow(this float f, double power = 2) {
		return Mathf.Pow(f, (float)power);
	}
	
	public static float Round(this float f) {
		return f.Round(1);
	}
	
	public static float Round(this float f, double step) {
		return step <= 0 ? f : (float)(Mathf.Round((float)(f * (1D / step))) / (1D / step));
	}
}
