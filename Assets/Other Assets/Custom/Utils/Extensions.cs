using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

static public class Extensions {

	#region Int
	static public int Round(this int i, double step = 1) {
		return step == 0 ? i : (int)(Mathf.Round((float)(i * (1D / step))) / (1D / step));
	}
	
	static public float[] ToFloatArray(this int[] intArray) {
		float[] floatArray = new float[intArray.Length];
		for (int i = 0; i < intArray.Length; i++) {
			floatArray[i] = (float)intArray[i];
		}
		return floatArray;
	}
	#endregion
	
	#region Float
	static public float Round(this float f, double step = 1) {
		return step == 0 ? f : (float)(Mathf.Round((float)(f * (1D / step))) / (1D / step));
	}
	#endregion
			
	#region Double
	static public double Round(this double d, double step = 1) {
		return step == 0 ? d : (double)(Mathf.Round((float)(d * (1D / step))) / (1D / step));
	}
	
	static public float[] ToFloatArray(this double[] doubleArray) {
		float[] floatArray = new float[doubleArray.Length];
		for (int i = 0; i < doubleArray.Length; i++) {
			floatArray[i] = (float)doubleArray[i];
		}
		return floatArray;
	}
	#endregion
	
	#region String
	static public char Pop(this string s, int index, out string remaining) {
		char c = s[0];
		remaining = s.Remove(index, 1);
		return c;
	}
	
	static public char Pop(this string s, out string remaining) {
		return s.Pop(0, out remaining);
	}
	
	static public string PopRange(this string s, int startIndex, char stopCharacter, out string remaining) {
		string popped = "";
		int maximumIterations = s.Length;
		
		for (int i = 0; i < maximumIterations - startIndex; i++) {
			char c = s.Pop(startIndex, out s);
	       	
			if (c == stopCharacter) {
				break;
			}
			popped += c;
		}
		
		remaining = s;
		return popped;
	}
	
	static public string PopRange(this string s, char stopCharacter, out string remaining) {
		return s.PopRange(0, stopCharacter, out remaining);
	}
	
	static public string PopRange(this string s, int startIndex, int count, out string remaining) {
		string popped = "";
		
		for (int i = 0; i < count; i++) {
			popped += s.Pop(startIndex, out s);
		}
		
		remaining = s;
		return popped;
	}
	
	static public string PopRange(this string s, int count, out string remaining) {
		return s.PopRange(0, count, out remaining);
	}
	
	static public string GetRange(this string s, int startIndex, char stopCharacter){
		string substring = "";
		
		for (int i = 0; i < s.Length - startIndex; i++) {
			char c = s[i + startIndex];
			if (c == stopCharacter) {
				break;
			}
			substring += c;
		}
		return substring;
	}
	
	static public string GetRange(this string s, char stopCharacter){
		return s.GetRange(0, stopCharacter);
	}
	
	static public string Reverse(this string s) {
		string reversed = "";
		for (int i = s.Length - 1; i >= 0; i--) {
			reversed += s[i];
		}
		return reversed;
	}
	
	static public string Capitalize(this string s) {
		string capitalized = "";
		if (s.Length == 0)
			capitalized = char.ToUpper(s[0]).ToString();
		else if (s.Length > 1)
			capitalized = char.ToUpper(s[0]) + s.Substring(1);
		return capitalized;
	}
	
	static public float GetWidth(this string s, Font font) {
		float widthSum = 0;
		
		foreach (var letter in s) {
			CharacterInfo charInfo;
			font.GetCharacterInfo(letter, out charInfo);
			widthSum += charInfo.width;
		}

		return widthSum;
	}
	
	static public Rect GetRect(this string s, Font font, int size = 10, FontStyle fontStyle = FontStyle.Normal) {
		float width = 0;
		float height = 0;
		float lineWidth = 0;
		float lineHeight = 0;
		
		foreach (char letter in s) {
			CharacterInfo charInfo;
			font.GetCharacterInfo(letter, out charInfo, size, fontStyle);
        	
			if (letter == '\n') {
				if (lineHeight == 0)
					lineHeight = size;
				width = Mathf.Max(width, lineWidth);
				height += lineHeight;
				lineWidth = 0;
				lineHeight = 0;
			}
			else {
				lineWidth += charInfo.width;
				lineHeight = Mathf.Max(lineHeight, charInfo.size);
			}
		}
		width = Mathf.Max(width, lineWidth);
		height += lineHeight;
		
		return new Rect(0, 0, width, height);
	}
	
	static public GUIContent[] ToGUIContents(this string[] labels, char labelTooltipSeparator = '\0') {
		GUIContent[] guiContents = new GUIContent[labels.Length];
		
		for (int i = 0; i < labels.Length; i++) {
			if (labelTooltipSeparator != '\0') {
				string[] split = labels[i].Split(labelTooltipSeparator);
				if (split.Length == 1)
					guiContents[i] = new GUIContent(split[0]);
				else if (split.Length == 2)
					guiContents[i] = new GUIContent(split[0], split[1]);
				else
					guiContents[i] = new GUIContent(labels[i]);
			}
			else
				guiContents[i] = new GUIContent(labels[i]);
		}
		return guiContents;
	}
	
	static public GUIContent[] ToGUIContents(this string[] labels, string[] tooltips) {
		GUIContent[] guiContents = new GUIContent[labels.Length];
		
		for (int i = 0; i < labels.Length; i++) {
			guiContents[i] = new GUIContent(labels[i], tooltips[i]);
		}
		return guiContents;
	}
	
	#endregion
	
	#region Array
	static public int[] ToIntArray<T>(this T[] array) {
		int[] intArray = new int[array.Length];
		for (int i = 0; i < array.Length; i++) {
			intArray[i] = array[i].GetHashCode();
		}
		return intArray;
	}
	
	static public float[] ToFloatArray<T>(this T[] array) {
		float[] floatArray = new float[array.Length];
		for (int i = 0; i < array.Length; i++) {
			floatArray[i] = (float)(array[i].GetHashCode());
		}
		return floatArray;
	}
	
	static public double[] ToDoubleArray<T>(this T[] array) {
		double[] doubleArray = new double[array.Length];
		for (int i = 0; i < array.Length; i++) {
			doubleArray[i] = (double)(array[i].GetHashCode());
		}
		return doubleArray;
	}
	
	static public string[] ToStringArray<T>(this T[] array) {
		string[] stringArray = new string[array.Length];
		for (int i = 0; i < array.Length; i++) {
			stringArray[i] = array[i].ToString();
		}
		return stringArray;
	}
	
	static public bool Contains<T>(this T[] array, T targetObject) {
		return array.Any(t => t.Equals(targetObject));
	}
	
	static public bool Contains<T>(this T[] array, Type type) {
		return typeof(T) == typeof(Type) ? array.Any(t => t.Equals(type)) : array.Any(t => t.GetType() == type);
	}
	
	static public T GetRandom<T>(this T[] array) {
		if (array == null || array.Length == 0)
			return default(T);
		
		return array[UnityEngine.Random.Range(0, array.Length - 1)];
	}
	
	static public T[] Slice<T>(this T[] array, int startIndex) {
		return array.Slice(startIndex, array.Length - 1);
	}
	
	static public T[] Slice<T>(this T[] array, int startIndex, int endIndex) {
		T[] slicedArray = new T[endIndex - startIndex];
		for (int i = 0; i < endIndex - startIndex; i++) {
			slicedArray[i] = array[i + startIndex];
		}
		return slicedArray;
	}
	#endregion

	#region List
	static public T Pop<T>(this IList<T> list, int index = 0) {
		if (list == null || list.Count == 0)
			return default(T);
			
		T item = list[index];
		list.RemoveAt(index);
		return item;
	}
	
	static public T PopRandom<T>(this IList<T> list) {
		if (list == null || list.Count == 0)
			return default(T);
		
		int index = UnityEngine.Random.Range(0, list.Count - 1);
		T item = list[index];
		list.RemoveAt(index);
		return item;
	}

	static public T GetRandom<T>(this IList<T> list) {
		if (list == null || list.Count == 0)
			return default(T);
		
		return list[UnityEngine.Random.Range(0, list.Count - 1)];
	}
	#endregion
	
	#region Dictionary
	static public void SwitchKeys<T, U>(this IDictionary<T, U> dictionary, T key1, T key2) {
		U value1 = dictionary.ContainsKey(key1) ? dictionary[key1] : default(U);
		U value2 = dictionary.ContainsKey(key2) ? dictionary[key2] : default(U);
		dictionary[key1] = value2;
		dictionary[key2] = value1;
	}
	#endregion
	
	#region Vector
	static public bool Intersects(this Vector2 vector, Rect rect) {
		return vector.x >= rect.xMin && vector.x <= rect.xMax && vector.y >= rect.yMin && vector.y <= rect.yMax;
	}
	
	static public Vector2 Round(this Vector2 vector, double step = 1) {
		if (step == 0)
			return vector;
		vector.x = (float)(Mathf.Round((float)(vector.x * (1D / step))) / (1D / step));
		vector.y = (float)(Mathf.Round((float)(vector.y * (1D / step))) / (1D / step));
		return vector;
	}
		
	static public Vector2 Rotate(this Vector2 vector, float angle) {
		Vector3 vec = new Vector3(vector.x, vector.y, 0).Rotate(angle);
		return new Vector2(vec.x, vec.y);
	}
	
	static public Vector2 SquareClamp(this Vector2 vector, float size = 1) {
		Vector3 v = new Vector3(vector.x, vector.y, 0).SquareClamp(size);
		return new Vector2(v.x, v.y);
	}
	
	static public Vector2 RectClamp(this Vector2 vector, float width = 1, float height = 1) {
		Vector3 v = new Vector3(vector.x, vector.y, 0).RectClamp(width, height);
		return new Vector2(v.x, v.y);
	}
	
	static public Vector3 Round(this Vector3 vector, double step = 1) {
		if (step == 0)
			return vector;
		vector.x = (float)(Mathf.Round((float)(vector.x * (1D / step))) / (1D / step));
		vector.y = (float)(Mathf.Round((float)(vector.y * (1D / step))) / (1D / step));
		vector.z = (float)(Mathf.Round((float)(vector.z * (1D / step))) / (1D / step));
		return vector;
	}
	
	static public Vector3 Rotate(this Vector3 vector, float angle) {
		return vector.Rotate(angle, Vector3.forward);
	}
	
	static public Vector3 Rotate(this Vector3 vector, float angle, Vector3 axis) {
		angle %= 360;
		return Quaternion.AngleAxis(-angle, axis) * vector;
	}
		
	static public Vector3 SquareClamp(this Vector3 vector, float size = 1) {
		return vector.RectClamp(size, size);
	}
	
	static public Vector3 RectClamp(this Vector3 vector, float width = 1, float height = 1) {
		float clamped;
		if (vector.x < -width || vector.x > width) {
			clamped = Mathf.Clamp(vector.x, -width, width);
			vector.y *= clamped / vector.x;
			vector.x = clamped;
		}
		if (vector.y < -height || vector.y > height) {
			clamped = Mathf.Clamp(vector.y, -height, height);
			vector.x *= clamped / vector.y;
			vector.y = clamped;
		}
		return vector;
	}
	
	static public Vector4 Round(this Vector4 vector, double step = 1) {
		if (step == 0)
			return vector;
		vector.x = (float)(Mathf.Round((float)(vector.x * (1D / step))) / (1D / step));
		vector.y = (float)(Mathf.Round((float)(vector.y * (1D / step))) / (1D / step));
		vector.z = (float)(Mathf.Round((float)(vector.z * (1D / step))) / (1D / step));
		vector.w = (float)(Mathf.Round((float)(vector.w * (1D / step))) / (1D / step));
		return vector;
	}
	#endregion
	
	#region Quaternion
	static public Quaternion Round(this Quaternion quaternion, double step = 1) {
		if (step == 0)
			return quaternion;
		quaternion.x = (float)(Mathf.Round((float)(quaternion.x * (1D / step))) / (1D / step));
		quaternion.y = (float)(Mathf.Round((float)(quaternion.y * (1D / step))) / (1D / step));
		quaternion.z = (float)(Mathf.Round((float)(quaternion.z * (1D / step))) / (1D / step));
		quaternion.w = (float)(Mathf.Round((float)(quaternion.w * (1D / step))) / (1D / step));
		return quaternion;
	}
	
	static public Quaternion Pow(this Quaternion quaternion, float power) {
		float inputMagnitude = quaternion.Magnitude();
		Vector3 nHat = new Vector3(quaternion.x, quaternion.y, quaternion.z).normalized;
		Quaternion vectorBit = new Quaternion(nHat.x, nHat.y, nHat.z, 0)
			.ScalarMultiply(power * Mathf.Acos(quaternion.w / inputMagnitude))
				.Exp();
		return vectorBit.ScalarMultiply(Mathf.Pow(inputMagnitude, power));
	}
 
	static public Quaternion Exp(this Quaternion quaternion) {
		float inputA = quaternion.w;
		var inputV = new Vector3(quaternion.x, quaternion.y, quaternion.z);
		float outputA = Mathf.Exp(inputA) * Mathf.Cos(inputV.magnitude);
		Vector3 outputV = Mathf.Exp(inputA) * (inputV.normalized * Mathf.Sin(inputV.magnitude));
		return new Quaternion(outputV.x, outputV.y, outputV.z, outputA);
	}
 
	static public float Magnitude(this Quaternion quaternion) {
		return Mathf.Sqrt(quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w);
	}
 
	static public Quaternion ScalarMultiply(this Quaternion quaternion, float scalar) {
		return new Quaternion(quaternion.x * scalar, quaternion.y * scalar, quaternion.z * scalar, quaternion.w * scalar);
	}
	#endregion
	
	#region Color
	static public Color Round(this Color c, double step = 1) {
		if (step == 0)
			return c;
		c.r = (float)(Mathf.Round((float)(c.r * (1D / step))) / (1D / step));
		c.g = (float)(Mathf.Round((float)(c.g * (1D / step))) / (1D / step));
		c.b = (float)(Mathf.Round((float)(c.b * (1D / step))) / (1D / step));
		c.a = (float)(Mathf.Round((float)(c.a * (1D / step))) / (1D / step));
		return c;
	}
	
	static public Color ToHSV(this Color RGBColor) {
		float R = RGBColor.r;
		float G = RGBColor.g;
		float B = RGBColor.b;
		float H = 0;
		float S = 0;
		float V = 0;
		float d = 0;
		float h = 0;
		
		float minRGB = Mathf.Min(R, Mathf.Min(G, B));
		float maxRGB = Mathf.Max(R, Mathf.Max(G, B));
	
		if (minRGB == maxRGB)
			return new Color(0, 0, minRGB, RGBColor.a);

		if (R == minRGB)
			d = G - B;
		else if (B == minRGB)
			d = R - G;
		else
			d = B - R;
			
		if (R == minRGB)
			h = 3;
		else if (B == minRGB)
			h = 1;
		else
			h = 5;
			
		H = (60 * (h - d / (maxRGB - minRGB))) / 360;
		S = (maxRGB - minRGB) / maxRGB;
		V = maxRGB;
		
		return new Color(H, S, V, RGBColor.a);
	}
	
	static public Color ToRGB(this Color HSVColor) {
		float H = HSVColor.r;
		float S = HSVColor.g;
		float V = HSVColor.b;
		float R = 0;
		float G = 0;
		float B = 0;
		float maxHSV = 255 * V;
		float minHSV = maxHSV * (1 - S);
		float h = H * 360;
		float z = (maxHSV - minHSV) * (1 - Mathf.Abs((h / 60) % 2 - 1));
		
		if (0 <= h && h < 60) {
			R = maxHSV;
			G = z + minHSV;
			B = minHSV;
		}
		else if (60 <= h && h < 120) {
			R = z + minHSV;
			G = maxHSV;
			B = minHSV;
		}
		else if (120 <= h && h < 180) {
			R = minHSV;
			G = maxHSV;
			B = z + minHSV;	
		}
		else if (180 <= h && h < 240) {
			R = minHSV;
			G = z + minHSV;
			;
			B = maxHSV;
		}
		else if (240 <= h && h < 300) {
			R = z + minHSV;
			G = minHSV;
			B = maxHSV;
		}
		else if (300 <= h && h < 360) {
			R = maxHSV;
			G = minHSV;
			B = z + minHSV;
		}
		return new Color(R / 255, G / 255, B / 255, HSVColor.a);
	}
	#endregion
	
	#region Rect
	static public Rect Round(this Rect rect, double step = 1) {
		if (step == 0)
			return rect;
		rect.x = (float)(Mathf.Round((float)(rect.x * (1D / step))) / (1D / step));
		rect.y = (float)(Mathf.Round((float)(rect.y * (1D / step))) / (1D / step));
		rect.width = (float)(Mathf.Round((float)(rect.width * (1D / step))) / (1D / step));
		rect.height = (float)(Mathf.Round((float)(rect.height * (1D / step))) / (1D / step));
		return rect;
	}
	
	static public void Copy(this Rect rect, Rect otherRect) {
		rect.x = otherRect.x;
		rect.y = otherRect.y;
		rect.width = otherRect.width;
		rect.height = otherRect.height;
	}
	
	static public bool Intersects(this Rect rect, Rect otherRect) {
		return !((rect.x > otherRect.xMax) || (rect.xMax < otherRect.x) || (rect.y > otherRect.yMax) || (rect.yMax < otherRect.y));
	}
	
	static public Rect Intersect(this Rect rect, Rect otherRect) {
		float x = Mathf.Max((sbyte)rect.x, (sbyte)otherRect.x);
		float num2 = Mathf.Min(rect.x + rect.width, otherRect.x + otherRect.width);
		float y = Mathf.Max((sbyte)rect.y, (sbyte)otherRect.y);
		float num4 = Mathf.Min(rect.y + rect.height, otherRect.y + otherRect.height);
		if ((num2 >= x) && (num4 >= y)) {
			return new Rect(x, y, num2 - x, num4 - y);
		}

		return new Rect();
	}
	#endregion
	
	#region LayerMask
	static public LayerMask Inverse(this LayerMask layerMask) {
		return ~layerMask;
	}
 
	static public LayerMask AddToMask(this LayerMask layerMask, params int[] layerNumbers) {
		foreach (int layer in layerNumbers) {
			layerMask |= (1 << layer);
		}
		return layerMask;
	}
	
	static public LayerMask AddToMask(this LayerMask layerMask, params string[] layerNames) {
		foreach (string layer in layerNames) {
			layerMask |= (1 << LayerMask.NameToLayer(layer));
		}
		return layerMask;
	}
 
	static public LayerMask RemoveFromMask(this LayerMask layerMask, params string[] layerNames) {
		layerMask = layerMask.Inverse();
		layerMask = layerMask.AddToMask(layerNames);
		return layerMask.Inverse();
	}
	
	static public LayerMask RemoveFromMask(this LayerMask layerMask, params int[] layerNumbers) {
		layerMask = layerMask.Inverse();
		layerMask = layerMask.AddToMask(layerNumbers);
		return layerMask.Inverse();
	}
	
	static public string[] LayerNames(this LayerMask layerMask) {
		var names = new List<string>();
 
		for (int i = 0; i < 32; ++i) {
			int shifted = 1 << i;
			if ((layerMask & shifted) == shifted) {
				string layerName = LayerMask.LayerToName(i);
				if (!string.IsNullOrEmpty(layerName)) {
					names.Add(layerName);
				}
			}
		}
		return names.ToArray();
	}
	#endregion
 
	#region AnimationCurve
	static public AnimationCurve Clamp(this AnimationCurve curve, float minTime, float maxTime, float minValue, float maxValue) {
		for (int i = 0; i < curve.keys.Length; i++) {
			Keyframe key = curve.keys[i];
			if (key.time < minTime || key.time > maxTime || key.value < minValue || key.value > maxValue) {
				var newKey = new Keyframe(Mathf.Clamp(key.time, minTime, maxTime), Mathf.Clamp(key.value, minValue, maxValue));
				newKey.inTangent = key.inTangent;
				newKey.outTangent = key.outTangent;
				curve.MoveKey(i, newKey);
			}
		}
		return curve;
	}
	#endregion

	#region Renderer
	static public bool IsVisibleFrom(this Renderer renderer, Camera camera) {
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
		return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
	}
	#endregion
	
	#region Camera
	static public Vector3 GetWorldMousePosition(this Camera camera, float depth = 0) {
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = depth - camera.transform.position.z;
		return camera.ScreenToWorldPoint(mousePosition);
	}
	
	static public bool WorldPointInView(this Camera camera, Vector3 worldPoint) {
		Vector3 viewPoint = camera.WorldToViewportPoint(worldPoint);
		return viewPoint.x >= 0 && viewPoint.x <= 1 && viewPoint.y >= 0 && viewPoint.y <= 1;
	}
	
	static public bool ScreenPointInView(this Camera camera, Vector2 screenPoint) {
		Vector3 viewPoint = camera.ScreenToViewportPoint(screenPoint);
		return viewPoint.x >= 0 && viewPoint.x <= 1 && viewPoint.y >= 0 && viewPoint.y <= 1;
	}
	#endregion
	
	#region Transform
	static public void SetPosition(this Transform transform, Vector3 position, string axis = "XYZ") {
		Vector3 newPosition = transform.position;
		if (axis.Contains("X"))
			newPosition.x = position.x;
		if (axis.Contains("Y"))
			newPosition.y = position.y;
		if (axis.Contains("Z"))
			newPosition.z = position.z;
		transform.position = newPosition;
	}
	
	static public void SetPosition(this Transform transform, float position, string axis = "XYZ") {
		transform.SetPosition(new Vector3(position, position, position), axis);
	}
	
	static public void SetLocalPosition(this Transform transform, Vector3 position, string axis = "XYZ") {
		Vector3 newPosition = transform.localPosition;
		if (axis.Contains("X"))
			newPosition.x = position.x;
		if (axis.Contains("Y"))
			newPosition.y = position.y;
		if (axis.Contains("Z"))
			newPosition.z = position.z;
		transform.localPosition = newPosition;
	}
	
	static public void SetLocalPosition(this Transform transform, float position, string axis = "XYZ") {
		transform.SetLocalPosition(new Vector3(position, position, position), axis);
	}
	
	static public void Translate(this Transform transform, Vector3 translation, string axis) {
		transform.SetPosition(transform.position + translation, axis);
	}
	
	static public void Translate(this Transform transform, float translation, string axis = "XYZ") {
		transform.SetPosition(transform.position + new Vector3(translation, translation, translation), axis);
	}
	
	static public void SetEulerAngles(this Transform transform, Vector3 angles, string axis = "XYZ") {
		Vector3 newAngles = transform.eulerAngles;
		if (axis.Contains("X"))
			newAngles.x = angles.x;
		if (axis.Contains("Y"))
			newAngles.y = angles.y;
		if (axis.Contains("Z"))
			newAngles.z = angles.z;
		transform.eulerAngles = newAngles;
	}
	
	static public void SetEulerAngles(this Transform transform, float angle, string axis = "XYZ") {
		transform.SetEulerAngles(new Vector3(angle, angle, angle), axis);
	}
	
	static public void SetLocalEulerAngles(this Transform transform, Vector3 angles, string axis = "XYZ") {
		Vector3 newAngles = transform.localEulerAngles;
		if (axis.Contains("X"))
			newAngles.x = angles.x;
		if (axis.Contains("Y"))
			newAngles.y = angles.y;
		if (axis.Contains("Z"))
			newAngles.z = angles.z;
		transform.localEulerAngles = newAngles;
	}
	
	static public void SetLocalEulerAngles(this Transform transform, float angle, string axis = "XYZ") {
		transform.SetLocalEulerAngles(new Vector3(angle, angle, angle), axis);
	}
	
	static public void Rotate(this Transform transform, Vector3 rotation, string axis) {
		transform.SetEulerAngles(transform.eulerAngles + rotation, axis);
	}
	
	static public void Rotate(this Transform transform, float rotation, string axis = "XYZ") {
		transform.SetEulerAngles(transform.eulerAngles + new Vector3(rotation, rotation, rotation), axis);
	}
	
	static public void SetLocalScale(this Transform transform, Vector3 scale, string axis = "XYZ") {
		Vector3 newScale = transform.localScale;
		if (axis.Contains("X"))
			newScale.x = scale.x;
		if (axis.Contains("Y"))
			newScale.y = scale.y;
		if (axis.Contains("Z"))
			newScale.z = scale.z;
		transform.localScale = newScale;
	}
	
	static public void SetLocalScale(this Transform transform, float scale, string axis = "XYZ") {
		transform.SetLocalScale(new Vector3(scale, scale, scale), axis);
	}
	
	static public void Scale(this Transform transform, Vector3 scale, string axis = "XYZ") {
		transform.SetLocalScale(transform.localScale + scale, axis);
	}
	
	static public void Scale(this Transform transform, float scale, string axis = "XYZ") {
		transform.SetLocalScale(transform.localScale + new Vector3(scale, scale, scale), axis);
	}
	
	static public void FlipScale(this Transform transform, string axis = "Y") {
		Vector3 flippedScale = transform.localScale;
		
		if (axis.Contains("X"))
			flippedScale.x = -flippedScale.x;
		if (axis.Contains("Y"))
			flippedScale.y = -flippedScale.y;
		if (axis.Contains("Z"))
			flippedScale.z = -flippedScale.z;
		
		transform.localScale = flippedScale;
	}
	
	static public void LookAt2D(this Transform transform, Transform target) {
		transform.LookAt2D(target.position, 0, 100);
	}
	
	static public void LookAt2D(this Transform transform, Vector3 target) {
		transform.LookAt2D(target, 0, 100);
	}
	
	static public void LookAt2D(this Transform transform, Transform target, float angleOffset = 0, float damping = 100) {
		transform.LookAt2D(target.position, angleOffset, damping);
	}
	
	static public void LookAt2D(this Transform transform, Vector3 target, float angleOffset = 0, float damping = 100) {
		transform.rotation = transform.LookingAt2D(target, angleOffset, damping);
	}
	
	static public Quaternion LookingAt2D(this Transform transform, Transform target) {
		return transform.LookingAt2D(target.position, 0, 100);
	}
	
	static public Quaternion LookingAt2D(this Transform transform, Vector3 target) {
		return transform.LookingAt2D(target, 0, 100);
	}
	
	static public Quaternion LookingAt2D(this Transform transform, Transform target, float angleOffset = 0, float damping = 100) {
		return transform.LookingAt2D(target.position, angleOffset, damping);
	}
	
	static public Quaternion LookingAt2D(this Transform transform, Vector3 target, float angleOffset = 0, float damping = 100) {
		Vector3 targetDirection = (target - transform.position).normalized;
		float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg + angleOffset;
		return Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), damping * Time.deltaTime);
	}
	
	static public T GetOrAddComponent<T>(this Transform transform) where T : Component {
		return transform.gameObject.GetOrAddComponent<T>();
	}
	
	static public Transform[] GetChildren(this Transform parent) {
		var children = new List<Transform>();
		if (parent != null) {
			if (parent.childCount > 0) {
				for (int i = 0; i < parent.childCount; i++) {
					Transform child = parent.GetChild(i);
					children.Add(child);
				}
			}
		}
		return children.ToArray();
	}
	
	static public Transform[] GetChildrenRecursive(this Transform parent) {
		var children = new List<Transform>();
		if (parent != null) {
			foreach (Transform child in parent.GetChildren()) {
				children.Add(child);
				if (child.childCount > 0) {
					children.AddRange(child.GetChildrenRecursive());
				}
			}
		}
		return children.ToArray();
	}
	
	static public Transform FindChildRecursive(this Transform parent, string childName) {
		foreach (var child in parent.GetChildrenRecursive()) {
			if (child.name == childName)
				return child;
		}
		return null;
	}
	
	static public Transform AddChild(this Transform parent) {
		return parent.AddChild("");
	}
	
	static public Transform AddChild(this Transform parent, string childName) {
		var child = new GameObject();
		if (!string.IsNullOrEmpty(childName))
			child.name = childName;
		child.transform.Reset();
		child.transform.parent = parent;
		return child.transform;
	}
	
	static public Transform FindOrAddChild(this Transform parent, string childName) {
		Transform child = parent.FindChild(childName) ?? parent.AddChild(childName);
		return child;
	}
	
	static public void SortChildren(this Transform parent) {
		Transform[] children = parent.GetChildren();
		var childrendNames = new List<string>();
		
		foreach (var child in children) {
			childrendNames.Add(child.name);
			child.parent = null;
		}
		
		Array.Sort(childrendNames.ToArray(), children);
		
		foreach (var child in children) {
			child.parent = parent;
		}
	}
	
	static public void SortChildrenRecursive(this Transform parent) {
		parent.SortChildren();
		foreach (Transform child in parent.GetChildren()) {
			if (child.childCount > 0)
				child.SortChildrenRecursive();
		}
	}
	
	static public void DestroyChildren(this Transform parent) {
		foreach (Transform child in parent.GetChildren()) {
			UnityEngine.Object.Destroy(child.gameObject);
		}
	}
	
	static public void DestroyChildrenImmediate(this Transform parent) {
		foreach (Transform child in parent.GetChildren()) {
			UnityEngine.Object.DestroyImmediate(child.gameObject);
		}
	}
	
	static public int GetHierarchyDepth(this Transform transform){
		int depth = 0;
		Transform currentTransform = transform;
		
		while (currentTransform.parent != null){
			currentTransform = currentTransform.parent;
			depth += 1;
		}
		
		return depth;
	}
	
	static public void Reset(this Transform transform) {
		transform.localPosition = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}
	#endregion
	
	#region GameObject
	static public GameObject[] GetChildren(this GameObject parent) {
		var children = new List<GameObject>();
		foreach (var child in parent.transform.GetChildren()) {
			children.Add(child.gameObject);
		}
		return children.ToArray();
	}
	
	static public GameObject[] GetChildrenRecursive(this GameObject parent) {
		var children = new List<GameObject>();
		foreach (var child in parent.transform.GetChildrenRecursive()) {
			children.Add(child.gameObject);
		}
		return children.ToArray();
	}
	
	static public int GetChildCount(this GameObject parent) {
		return parent.transform.childCount;
	}
	
	static public GameObject GetChild(this GameObject parent, int index) {
		return parent.transform.GetChild(index).gameObject;
	}
	
	static public GameObject FindChild(this GameObject parent, string childName) {
		foreach (var child in parent.transform.GetChildren()) {
			if (child.name == childName)
				return child.gameObject;
		}
		return null;
	}

	static public GameObject FindChildRecursive(this GameObject parent, string childName) {
		foreach (var child in parent.transform.GetChildrenRecursive()) {
			if (child.name == childName)
				return child.gameObject;
		}
		return null;
	}
	
	static public GameObject AddChild(this GameObject parent) {
		return parent.transform.AddChild().gameObject;
	}
	
	static public GameObject AddChild(this GameObject parent, string childName) {
		return parent.transform.AddChild(childName).gameObject;
	}
	
	static public GameObject FindOrAddChild(this GameObject parent, string childName) {
		return parent.transform.FindOrAddChild(childName).gameObject;
	}
	
	static public void SortChildren(this GameObject parent) {
		parent.transform.SortChildren();
	}
	
	static public void SortChildrenRecursive(this GameObject parent) {
		parent.transform.SortChildrenRecursive();
	}
	
	static public int GetHierarchyDepth(this GameObject gameObject){
		return gameObject.transform.GetHierarchyDepth();
	}
	
	static public void DisconnectPrefab(this GameObject gameObject) {
		#if UNITY_EDITOR
		if (gameObject.transform.parent == null){
			UnityEditor.PrefabUtility.DisconnectPrefabInstance(gameObject);
		}
		#endif
	}
	
	static public void RemoveComponent(this GameObject gameObject, Component component) {
		if (Application.isPlaying)
			UnityEngine.Object.Destroy(component);
		else
			UnityEngine.Object.DestroyImmediate(component);
	}
	
	static public void RemoveComponent<T>(this GameObject gameObject) where T : Component {
		if (Application.isPlaying)
			UnityEngine.Object.Destroy(gameObject.GetComponent<T>());
		else
			UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<T>());
	}
	
	static public T GetOrAddComponent<T>(this GameObject gameObject) where T : Component {
		T component = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
		return component;
	}
	
	static public Component AddCopiedComponent(this GameObject copyTo, Component copyFrom) {
		Component component = copyTo.AddComponent(copyFrom.GetType());
		component.Copy(copyFrom);
		return component;
	}
	
	static public Component[] AddCopiedComponents(this GameObject copyTo, params Component[] copyFrom) {
		var components = new List<Component>();
		foreach (Component component in copyFrom) {
			components.Add(copyTo.AddCopiedComponent(component));
		}
		return components.ToArray();
	}
	
	static public Component[] AddCopiedComponents(this GameObject copyTo, GameObject copyFrom, params Type[] typesToIgnore) {
		var clonedComponents = new List<Component>();
		Component[] dstComponents = copyFrom.GetComponents(typeof(Component));
		
		foreach (Component dstComponent in dstComponents) {
			if (!typesToIgnore.Contains(dstComponent.GetType())) {
				if (dstComponent is Transform || (dstComponent is ParticleSystemRenderer && dstComponents.Contains(typeof(ParticleSystem))))
					copyTo.CopyComponent(dstComponent);
				else {
					Component clonedComponent = copyTo.AddCopiedComponent(dstComponent);
					if (clonedComponent != null)
						clonedComponents.Add(clonedComponent);
				}
			}
		}
		return clonedComponents.ToArray();
	}
	
	static public Component CopyComponent(this GameObject copyTo, Component copyFrom) {
		Component clonedComponent = copyTo.GetComponent(copyFrom.GetType());
		if (clonedComponent != null)
			clonedComponent.Copy(copyFrom);
		else
			Debug.LogError("Component of type " + copyFrom.GetType().ToString() + " was not found on the GameObject.");
		return clonedComponent;
	}
	
	static public Component[] CopyComponents(this GameObject copyTo, params Component[] copyFrom) {
		var clonedComponents = new List<Component>();
		
		foreach (Component dstComponent in copyFrom) {
			Component clonedComponent = copyTo.CopyComponent(dstComponent);
			if (clonedComponent != null)
				clonedComponents.Add(clonedComponent);
		}
		return clonedComponents.ToArray();
	}
	
	static public Component[] CopyComponents(this GameObject copyTo, GameObject copyFrom, params Type[] typesToIgnore) {
		var clonedComponents = new List<Component>();
		Component[] dstComponents = copyFrom.GetComponents(typeof(Component));
		
		foreach (Component dstComponent in dstComponents) {
			if (!typesToIgnore.Contains(dstComponent.GetType())) {
				Component clonedComponent = copyTo.CopyComponent(dstComponent);
				if (clonedComponent != null)
					clonedComponents.Add(clonedComponent);
			}
		}
		return clonedComponents.ToArray();
	}
	#endregion
	
	#region MonoBehaviour
	static public T GetOrAddComponent<T>(this MonoBehaviour monoBehaviour) where T : Component {
		return monoBehaviour.gameObject.GetOrAddComponent<T>();
	}
	
	static public GameObject[] GetChildren(this MonoBehaviour parent) {
		var children = new List<GameObject>();
		foreach (var child in parent.transform.GetChildren()) {
			children.Add(child.gameObject);
		}
		return children.ToArray();
	}
	
	static public GameObject[] GetChildrenRecursive(this MonoBehaviour parent) {
		var children = new List<GameObject>();
		foreach (var child in parent.transform.GetChildrenRecursive()) {
			children.Add(child.gameObject);
		}
		return children.ToArray();
	}
	
	static public int GetChildCount(this MonoBehaviour parent) {
		return parent.transform.childCount;
	}
	
	static public GameObject GetChild(this MonoBehaviour parent, int index) {
		return parent.transform.GetChild(index).gameObject;
	}
	
	static public GameObject FindChild(this MonoBehaviour parent, string childName) {
		foreach (var child in parent.transform.GetChildren()) {
			if (child.name == childName)
				return child.gameObject;
		}
		return null;
	}

	static public GameObject FindChildRecursive(this MonoBehaviour parent, string childName) {
		foreach (var child in parent.transform.GetChildrenRecursive()) {
			if (child.name == childName)
				return child.gameObject;
		}
		return null;
	}

	static public GameObject AddChild(this MonoBehaviour parent) {
		return parent.transform.AddChild().gameObject;
	}
	
	static public GameObject AddChild(this MonoBehaviour parent, string childName) {
		return parent.transform.AddChild(childName).gameObject;
	}
	
	static public GameObject FindOrAddChild(this MonoBehaviour parent, string childName) {
		return parent.transform.FindOrAddChild(childName).gameObject;
	}
	
	static public void SortChildren(this MonoBehaviour parent) {
		parent.transform.SortChildren();
	}
	
	static public void SortChildrenRecursive(this MonoBehaviour parent) {
		parent.transform.SortChildrenRecursive();
	}
	
	static public void SetExecutionOrder(this MonoBehaviour script, int order) {
		#if UNITY_EDITOR
		foreach (UnityEditor.MonoScript s in UnityEditor.MonoImporter.GetAllRuntimeMonoScripts()) {
			if (s.name == script.GetType().Name){
				if (UnityEditor.MonoImporter.GetExecutionOrder(s) != order){
					UnityEditor.MonoImporter.SetExecutionOrder(s, order);
				}
			}
		}
		#endif
	}
	
	static public void SetHasChanged(this MonoBehaviour script, Transform transform, bool hasChanged) {
		script.StartCoroutine(SetHasChanged(transform, hasChanged));
	}
	static IEnumerator SetHasChanged(Transform transform, bool hasChanged) {
		yield return new WaitForEndOfFrame();
		transform.hasChanged = hasChanged;
	}
	#endregion
	
	#region Component
	static public T AddComponent<T>(this Component component) where T : Component {
		return component.gameObject.AddComponent<T>();
	}
	
	static public T GetOrAddComponent<T>(this Component component) where T : Component {
		return component.gameObject.GetOrAddComponent<T>();
	}
	
	static public void Remove(this Component component) {
		if (Application.isPlaying)
			UnityEngine.Object.Destroy(component);
		else
			UnityEngine.Object.DestroyImmediate(component);
	}
	#endregion
	
	#region Object
	static public T Clone<T>(this T toClone) {
		if (!typeof(T).IsSerializable) {
			throw new ArgumentException("The type must be serializable.", "source");
		}

		if (object.ReferenceEquals(toClone, null)) {
			return default(T);
		}

		IFormatter formatter = new BinaryFormatter();
		Stream stream = new MemoryStream();
		using (stream) {
			formatter.Serialize(stream, toClone);
			stream.Seek(0, SeekOrigin.Begin);
			return (T)formatter.Deserialize(stream);
		}
	}
	
	static public void Copy<T>(this T copyTo, T copyFrom, params string[] parametersToIgnore) {
		if (typeof(T) == typeof(Component) || typeof(T).IsSubclassOf(typeof(Component))) {
			var parametersToIgnoreList = new List<string>(parametersToIgnore);
			parametersToIgnoreList.Add("name");
			parametersToIgnoreList.Add("tag");
			if (!(copyFrom is MonoBehaviour)) {
				parametersToIgnoreList.Add("minVolume");
				parametersToIgnoreList.Add("maxVolume");
				parametersToIgnoreList.Add("rolloffFactor");
				parametersToIgnoreList.Add("playbackTime");
				parametersToIgnoreList.Add("mesh");
				parametersToIgnoreList.Add("material");
				parametersToIgnoreList.Add("materials");
				parametersToIgnoreList.Add("destination");
				parametersToIgnoreList.Add("path");
			}
			parametersToIgnore = parametersToIgnoreList.ToArray();
		}
		
		foreach (FieldInfo fieldInfo in copyFrom.GetType().GetFields()) {
			if ((fieldInfo.IsPublic || fieldInfo.GetCustomAttributes(typeof(SerializeField), true).Length != 0) && !fieldInfo.IsLiteral && fieldInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0 && !parametersToIgnore.Contains(fieldInfo.Name)) {
				try {
					fieldInfo.SetValue(copyTo, fieldInfo.GetValue(copyFrom));
				}
				catch {
				}
			}
		}
		foreach (PropertyInfo propertyInfo in copyFrom.GetType().GetProperties()) {
			if (propertyInfo.CanWrite && propertyInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0 && !parametersToIgnore.Contains(propertyInfo.Name)) {
				try {
					propertyInfo.SetValue(copyTo, propertyInfo.GetValue(copyFrom, null), null);
				}
				catch {
				}
			}
		}
	}
	#endregion

	#region Type
	static public object CreateDefaultInstance(this Type type) {
		object instance = null;
		
		if (type == typeof(string)) {
			instance = string.Empty;
		}
		else {
			instance = Activator.CreateInstance(type, type.GetDefaultConstructorParameters());
		}
		
		return instance;
	}
	
	static public object[] GetDefaultConstructorParameters(this Type type) {
		List<object> parameters = new List<object>();
		
		if (!type.HasEmptyConstructor() && type.HasConstructor()) {
			ParameterInfo[] parameterInfos = type.GetConstructors()[0].GetParameters();
		
			foreach (ParameterInfo info in parameterInfos) {
				parameters.Add(info.ParameterType.CreateDefaultInstance());
			}
		}
		
		return parameters.ToArray();
	}
	
	static public bool HasConstructor(this Type type) {
		return type.GetConstructors().Length > 0;
	}
	
	static public bool HasEmptyConstructor(this Type type) {
		return type.GetConstructor(Type.EmptyTypes) != null;
	}
	
	static public bool HasDefaultConstructor(this Type type) {
		return type.IsValueType || type.HasEmptyConstructor();
	}
	
	static public bool IsNumerical(this Type type) {
		return type == typeof(int) || type == typeof(float) || type == typeof(double);
	}
	
	static public bool IsVector(this Type type) {
		return type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4);
	}
	#endregion
}
