using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

static public class Extensions {

	#region Int
	static public int Round(this int i, double step = 1){
		if (step == 0) return i;
		return (int) (Mathf.Round((float) (i * (1D / step))) / (1D / step));
	}
	#endregion
	
	#region Float
	static public float Round(this float f, double step = 1){
		if (step == 0) return f;
		return (float) (Mathf.Round((float) (f * (1D / step))) / (1D / step));
	}
	#endregion
			
	#region Double	
	static public double Round(this double d, double step = 1){
		if (step == 0) return d;
		return (double) (Mathf.Round((float) (d * (1D / step))) / (1D / step));
	}
	#endregion
	
	#region String
	static public string Reverse(this string s){
		string reversed = "";
		for (int i = s.Length - 1; i >= 0; i--){
			reversed += s[i];
		}
		return reversed;
	}
	
	static public float GetWidth(this string s, Font font){
		float widthSum = 0;
		
        foreach (var letter in s){
        	CharacterInfo charInfo;
        	font.GetCharacterInfo(letter, out charInfo);
        	widthSum += charInfo.width;
        }

        return widthSum;
	}
	
	static public Rect GetRect(this string s, Font font, int size = 10, FontStyle fontStyle = FontStyle.Normal){
		float width = 0;
		float height = 0;
		float lineWidth = 0;
		float lineHeight = 0;
		
        foreach (char letter in s){
        	CharacterInfo charInfo;
        	font.GetCharacterInfo(letter, out charInfo, size, fontStyle);
        	
        	if (letter == '\n'){
        		if (lineHeight == 0) lineHeight = size;
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
	#endregion
	
	#region Array
	static public bool Contains<T>(this T[] array, T targetObject){
		return array.Any(t => t.Equals(targetObject));
	}
	
	static public bool Contains<T>(this T[] array, Type type){
		return array.Any(t => t.GetType() == type);
	}
	#endregion

	#region Vector
	static public Vector2 Round(this Vector2 vector, double step = 1){
		if (step == 0) return vector;
		vector.x = (float) (Mathf.Round((float) (vector.x * (1D / step))) / (1D / step));
		vector.y = (float) (Mathf.Round((float) (vector.y * (1D / step))) / (1D / step));
		return vector;
	}
	
	static public Vector3 Round(this Vector3 vector, double step = 1){
		if (step == 0) return vector;
		vector.x = (float) (Mathf.Round((float) (vector.x * (1D / step))) / (1D / step));
		vector.y = (float) (Mathf.Round((float) (vector.y * (1D / step))) / (1D / step));
		vector.z = (float) (Mathf.Round((float) (vector.z * (1D / step))) / (1D / step));
		return vector;
	}
	
	static public Vector4 Round(this Vector4 vector, double step = 1){
		if (step == 0) return vector;
		vector.x = (float) (Mathf.Round((float) (vector.x * (1D / step))) / (1D / step));
		vector.y = (float) (Mathf.Round((float) (vector.y * (1D / step))) / (1D / step));
		vector.z = (float) (Mathf.Round((float) (vector.z * (1D / step))) / (1D / step));
		vector.w = (float) (Mathf.Round((float) (vector.w * (1D / step))) / (1D / step));
		return vector;
	}
		
	static public Vector2 Rotate(this Vector2 vector, float angle){
		Vector3 vec = new Vector3(vector.x, vector.y, 0).Rotate(angle);
		return new Vector2(vec.x, vec.y);
	}
	
	static public Vector3 Rotate(this Vector3 vector, float angle){
		return vector.Rotate(angle, Vector3.forward);
	}
	
	static public Vector3 Rotate(this Vector3 vector, float angle, Vector3 axis){
		angle %= 360;
		return Quaternion.AngleAxis(-angle, axis) * vector;
	}
	
	static public Vector2 SquareClamp(this Vector2 vector, float size = 1){
		Vector3 v = new Vector3(vector.x, vector.y, 0).SquareClamp(size);
		return new Vector2(v.x, v.y);
	}
		
	static public Vector3 SquareClamp(this Vector3 vector, float size = 1){
		float clamped;
		if (vector.x < -size || vector.x > size){
			clamped = Mathf.Clamp(vector.x, -size, size);
			vector.y *= clamped / vector.x;
			vector.x = clamped;
		}
		if (vector.y < -size || vector.y > size){
			clamped = Mathf.Clamp(vector.y, -size, size);
			vector.x *= clamped / vector.y;
			vector.y = clamped;
		}
		return vector;
	}
	#endregion
	
	#region Quaternion
	static public Quaternion Round(this Quaternion quaternion, double step = 1){
		if (step == 0) return quaternion;
		quaternion.x = (float) (Mathf.Round((float) (quaternion.x * (1D / step))) / (1D / step));
		quaternion.y = (float) (Mathf.Round((float) (quaternion.y * (1D / step))) / (1D / step));
		quaternion.z = (float) (Mathf.Round((float) (quaternion.z * (1D / step))) / (1D / step));
		quaternion.w = (float) (Mathf.Round((float) (quaternion.w * (1D / step))) / (1D / step));
		return quaternion;
	}
	
	static public Quaternion Pow(this Quaternion input, float power){
		float inputMagnitude = input.Magnitude();
		Vector3 nHat = new Vector3(input.x, input.y, input.z).normalized;
		Quaternion vectorBit = new Quaternion(nHat.x, nHat.y, nHat.z, 0)
			.ScalarMultiply(power * Mathf.Acos(input.w / inputMagnitude))
				.Exp();
		return vectorBit.ScalarMultiply(Mathf.Pow(inputMagnitude, power));
	}
 
	static public Quaternion Exp(this Quaternion input){
		float inputA = input.w;
		Vector3 inputV = new Vector3(input.x, input.y, input.z);
		float outputA = Mathf.Exp(inputA) * Mathf.Cos(inputV.magnitude);
		Vector3 outputV = Mathf.Exp(inputA) * (inputV.normalized * Mathf.Sin(inputV.magnitude));
		return new Quaternion(outputV.x, outputV.y, outputV.z, outputA);
	}
 
	static public float Magnitude(this Quaternion input){
		return Mathf.Sqrt(input.x * input.x + input.y * input.y + input.z * input.z + input.w * input.w);
	}
 
	static public Quaternion ScalarMultiply(this Quaternion input, float scalar){
		return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
	}
	#endregion
	
	#region Color
	static public Color Round(this Color c , double step = 1){
		if (step == 0) return c;
		c.r = (float) (Mathf.Round((float) (c.r * (1D / step))) / (1D / step));
		c.g = (float) (Mathf.Round((float) (c.g * (1D / step))) / (1D / step));
		c.b = (float) (Mathf.Round((float) (c.b * (1D / step))) / (1D / step));
		c.a = (float) (Mathf.Round((float) (c.a * (1D / step))) / (1D / step));
		return c;
	}
	
	static public Color ToHSV (this Color RGBColor){
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
	
		if (minRGB == maxRGB) return new Color(0, 0, minRGB, RGBColor.a);

		if (R == minRGB) d = G - B;
		else if (B == minRGB) d = R - G;
		else d = B - R;
			
		if (R == minRGB) h = 3;
		else if (B == minRGB) h = 1;
		else h = 5;
			
		H = (60 * (h - d / (maxRGB - minRGB))) / 360;
		S = (maxRGB - minRGB) / maxRGB;
		V = maxRGB;
		
		return new Color(H, S, V, RGBColor.a);
	}
	
	static public Color ToRGB (this Color HSVColor){
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
		
		if (0 <= h && h < 60){
			R = maxHSV;
			G = z + minHSV;
			B = minHSV;
		}
		else if (60 <= h && h < 120){
			R = z + minHSV;
			G = maxHSV;
			B = minHSV;
		}
		else if (120 <= h && h < 180){
			R = minHSV;
			G = maxHSV;
			B = z + minHSV;	
		}
		else if (180 <= h && h < 240){
			R = minHSV;
			G = z + minHSV;;
			B = maxHSV;
		}
		else if (240 <= h && h < 300){
			R = z + minHSV;
			G = minHSV;
			B = maxHSV;
		}
		else if (300 <= h && h < 360){
			R = maxHSV;
			G = minHSV;
			B = z + minHSV;
		}
		return new Color(R / 255, G / 255, B / 255, HSVColor.a);
	}
	#endregion
	
	#region Rect
	static public Rect Round(this Rect rect, double step = 1){
		if (step == 0) return rect;
		rect.x = (float) (Mathf.Round((float) (rect.x * (1D / step))) / (1D / step));
		rect.y = (float) (Mathf.Round((float) (rect.y * (1D / step))) / (1D / step));
		rect.width = (float) (Mathf.Round((float) (rect.width * (1D / step))) / (1D / step));
		rect.height = (float) (Mathf.Round((float) (rect.height * (1D / step))) / (1D / step));
		return rect;
	}
	
	static public void Copy(this Rect rect, Rect otherRect){
		rect.x = otherRect.x;
		rect.y = otherRect.y;
		rect.width = otherRect.width;
		rect.height = otherRect.height;
	}
	
	static public bool Intersects(this Rect src, Rect rect){
        return !((src.x > rect.xMax) || (src.xMax< rect.x) || (src.y > rect.yMax) || (src.yMax< rect.y));
    }
	
	static public Rect Intersect(this Rect a, Rect b){
        float x = Mathf.Max((sbyte)a.x, (sbyte)b.x);
        float num2 = Mathf.Min(a.x + a.width, b.x + b.width);
        float y = Mathf.Max((sbyte)a.y, (sbyte)b.y);
        float num4 = Mathf.Min(a.y + a.height, b.y + b.height);
        if ((num2 >= x) && (num4 >= y))
        {
            return new Rect(x, y, num2 - x, num4 - y);
        }

        return new Rect();
    }
	#endregion
	
	#region LayerMask
	static public LayerMask Inverse(this LayerMask original){
		return ~original;
	}
 
	static public LayerMask AddToMask(this LayerMask original, params int[] layerNumbers){
		foreach(int layer in layerNumbers){
			original |= (1 << layer);
		}
		return original;
	}
	
	static public LayerMask AddToMask(this LayerMask original, params string[] layerNames){
		foreach(string layer in layerNames){
			original |= (1 << LayerMask.NameToLayer(layer));
		}
		return original;
	}
 
	static public LayerMask RemoveFromMask(this LayerMask original, params string[] layerNames){
		original = original.Inverse();
		original = original.AddToMask(layerNames);
		return original.Inverse();
	}
	
	static public LayerMask RemoveFromMask(this LayerMask original, params int[] layerNumbers){
		original = original.Inverse();
		original = original.AddToMask(layerNumbers);
		return original.Inverse();
	}
	
	static public string[] LayerNames(this LayerMask original){
		var names = new List<string>();
 
		for (int i = 0; i < 32; ++i)
		{
			int shifted = 1 << i;
			if ((original & shifted) == shifted)
			{
				string layerName = LayerMask.LayerToName(i);
				if (!string.IsNullOrEmpty(layerName))
				{
					names.Add(layerName);
				}
			}
		}
		return names.ToArray();
	}
	#endregion
 
	#region AnimationCurve
	static public void Clamp(this AnimationCurve curve, float minTime, float maxTime, float minValue, float maxValue){
		for (int i = 0; i < curve.keys.Length; i++){
			Keyframe key = curve.keys[i];
			if (key.time < minTime || key.time > maxTime || key.value < minValue || key.value > maxValue){
				Keyframe newKey = new Keyframe(Mathf.Clamp(key.time, minTime, maxTime), Mathf.Clamp(key.value, minValue, maxValue));
				newKey.inTangent = key.inTangent;
				newKey.outTangent = key.outTangent;
				curve.MoveKey(i, newKey);
			}
		}
	}
	#endregion

	#region Renderer
	static public bool IsVisibleFrom(this Renderer renderer, Camera camera){
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
		return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
	}
	#endregion
	
	#region Camera
	static public bool WorldPointInView(this Camera camera, Vector3 worldPoint){
		Vector3 viewPoint = camera.WorldToViewportPoint(worldPoint);
		return viewPoint.x >= 0 && viewPoint.x <= 1 && viewPoint.y >= 0 && viewPoint.y <= 1;
	}
	
	static public bool ScreenPointInView(this Camera camera, Vector2 screenPoint){
		Vector3 viewPoint = camera.ScreenToViewportPoint(screenPoint);
		return viewPoint.x >= 0 && viewPoint.x <= 1 && viewPoint.y >= 0 && viewPoint.y <= 1;
	}
	#endregion
	
	#region Transform
	static public void SetPosition(this Transform transform, Vector3 position, string axis = "XYZ"){
		Vector3 newPosition = transform.position;
		if (axis.Contains("X")) newPosition.x = position.x;
		if (axis.Contains("Y")) newPosition.y = position.y;
		if (axis.Contains("Z")) newPosition.z = position.z;
		transform.position = newPosition;
	}
	
	static public void SetPosition(this Transform transform, float position, string axis = "XYZ"){
		transform.SetPosition(new Vector3(position, position, position), axis);
	}
	
	static public void SetLocalPosition(this Transform transform, Vector3 position, string axis = "XYZ"){
		Vector3 newPosition = transform.localPosition;
		if (axis.Contains("X")) newPosition.x = position.x;
		if (axis.Contains("Y")) newPosition.y = position.y;
		if (axis.Contains("Z")) newPosition.z = position.z;
		transform.localPosition = newPosition;
	}
	
	static public void SetLocalPosition(this Transform transform, float position, string axis = "XYZ"){
		transform.SetLocalPosition(new Vector3(position, position, position), axis);
	}
	
	static public void SetEulerAngles(this Transform transform, Vector3 angles, string axis = "XYZ"){
		Vector3 newAngles = transform.eulerAngles;
		if (axis.Contains("X")) newAngles.x = angles.x;
		if (axis.Contains("Y")) newAngles.y = angles.y;
		if (axis.Contains("Z")) newAngles.z = angles.z;
		transform.eulerAngles = newAngles;
	}
	
	static public void SetEulerAngles(this Transform transform, float angle, string axis = "XYZ"){
		transform.SetEulerAngles(new Vector3(angle, angle, angle), axis);
	}
	
	static public void SetLocalEulerAngles(this Transform transform, Vector3 angles, string axis = "XYZ"){
		Vector3 newAngles = transform.localEulerAngles;
		if (axis.Contains("X")) newAngles.x = angles.x;
		if (axis.Contains("Y")) newAngles.y = angles.y;
		if (axis.Contains("Z")) newAngles.z = angles.z;
		transform.localEulerAngles = newAngles;
	}
	
	static public void SetLocalEulerAngles(this Transform transform, float angle, string axis = "XYZ"){
		transform.SetLocalEulerAngles(new Vector3(angle, angle, angle), axis);
	}
	
	static public void SetLocalScale(this Transform transform, Vector3 scale, string axis = "XYZ"){
		Vector3 newScale = transform.localScale;
		if (axis.Contains("X")) newScale.x = scale.x;
		if (axis.Contains("Y")) newScale.y = scale.y;
		if (axis.Contains("Z")) newScale.z = scale.z;
		transform.localScale = newScale;
	}
	
	static public void SetLocalScale(this Transform transform, float scale, string axis = "XYZ"){
		transform.SetLocalScale(new Vector3(scale, scale, scale), axis);
	}
	
	static public void LookAt2D(this Transform transform, Transform target){
		transform.LookAt2D(target.position, 0, 100);
	}
	
	static public void LookAt2D(this Transform transform, Vector3 target){
		transform.LookAt2D(target, 0, 100);
	}
	
	static public void LookAt2D(this Transform transform, Transform target, float angleOffset = 0, float damping = 100){
		transform.LookAt2D(target.position, angleOffset, damping);
	}
	
	static public void LookAt2D(this Transform transform, Vector3 target, float angleOffset = 0, float damping = 100){
		transform.rotation = transform.LookingAt2D(target, angleOffset, damping);
	}
	
	static public Quaternion LookingAt2D(this Transform transform, Transform target){
		return transform.LookingAt2D(target.position, 0, 100);
	}
	
	static public Quaternion LookingAt2D(this Transform transform, Vector3 target){
		return transform.LookingAt2D(target, 0, 100);
	}
	
	static public Quaternion LookingAt2D(this Transform transform, Transform target, float angleOffset = 0, float damping = 100){
		return transform.LookingAt2D(target.position, angleOffset, damping);
	}
	
	static public Quaternion LookingAt2D(this Transform transform, Vector3 target, float angleOffset = 0, float damping = 100){
		Vector3 targetDirection = (target - transform.position).normalized;
		float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg + angleOffset;
		return Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), damping * Time.deltaTime);
	}
	
	static public Transform[] GetChildren(this Transform parent){
		List<Transform> children = new List<Transform>();
		if (parent != null){
			if (parent.childCount > 0){
				for (int i = 0; i < parent.childCount; i++){
					Transform child = parent.GetChild(i);
					children.Add(child);
				}
			}
		}
		return children.ToArray();
	}
	
	static public Transform[] GetChildrenRecursive(this Transform parent){
		List<Transform> children = new List<Transform>();
		if (parent != null){
			foreach (Transform child in parent.GetChildren()){
				children.Add(child);
				if (child.childCount > 0){children.AddRange(child.GetChildrenRecursive());}
			}
		}
		return children.ToArray();
	}
	
	static public Transform FindChildRecursive(this Transform parent, string childName){
		foreach (var child in parent.GetChildrenRecursive()){
			if (child.name == childName) return child;
		}
		return null;
	}
	
	static public Transform AddChild(this Transform parent){
		return parent.AddChild("");
	}
	
	static public Transform AddChild(this Transform parent, string childName){
		GameObject child = new GameObject();
		if (!string.IsNullOrEmpty(childName)) child.name = childName;
		child.transform.parent = parent;
		return child.transform;
	}
	
	static public Transform FindOrAddChild(this Transform parent, string childName){
		Transform child = parent.FindChild(childName);
		if (child == null) child = parent.AddChild(childName);
		return child;
	}
	
	static public void SortChildren(this Transform parent){
		Transform[] children = parent.GetChildren();
		List<string> childrendNames = new List<string>();
		
		foreach (var child in children){
			childrendNames.Add(child.name);
			child.parent = null;
		}
		
		Array.Sort(childrendNames.ToArray(), children);
		
		foreach (var child in children){
			child.parent = parent;
		}
	}
	
	static public void SortChildrenRecursive(this Transform parent){
		parent.SortChildren();
		foreach (Transform child in parent.GetChildren()){
			if (child.childCount > 0) child.SortChildrenRecursive();
		}
	}
	
	static public void DestroyChildren(this Transform parent){
		foreach (Transform child in parent.GetChildren()){
			GameObject.Destroy(child.gameObject);
		}
	}
	
	static public void DestroyChildrenImmediate(this Transform parent){
		foreach (Transform child in parent.GetChildren()){
			GameObject.DestroyImmediate(child.gameObject);
		}
	}
	
	static public void Reset(this Transform transform){
		transform.localPosition = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}
	#endregion
	
	#region GameObject
	static public GameObject[] GetChildren(this GameObject parent){
		List<GameObject> children = new List<GameObject>();
		foreach (var child in parent.transform.GetChildren()){
			children.Add(child.gameObject);
		}
		return children.ToArray();
	}
	
	static public GameObject[] GetChildrenRecursive(this GameObject parent){
		List<GameObject> children = new List<GameObject>();
		foreach (var child in parent.transform.GetChildrenRecursive()){
			children.Add(child.gameObject);
		}
		return children.ToArray();
	}
	
	static public int GetChildCount(this GameObject parent){
		return parent.transform.childCount;
	}
	
	static public GameObject GetChild(this GameObject parent, int index){
		return parent.transform.GetChild(index).gameObject;
	}
	
	static public GameObject FindChild(this GameObject parent, string childName){
		foreach (var child in parent.transform.GetChildren()){
			if (child.name == childName) return child.gameObject;
		}
		return null;
	}

	static public GameObject FindChildRecursive(this GameObject parent, string childName){
		foreach (var child in parent.transform.GetChildrenRecursive()){
			if (child.name == childName) return child.gameObject;
		}
		return null;
	}
	
	static public GameObject AddChild(this GameObject parent){
		return parent.transform.AddChild().gameObject;
	}
	
	static public GameObject AddChild(this GameObject parent, string childName){
		return parent.transform.AddChild(childName).gameObject;
	}
	
	static public GameObject FindOrAddChild(this GameObject parent, string childName){
		return parent.transform.FindOrAddChild(childName).gameObject;
	}
	
	static public void SortChildren(this GameObject parent){
		parent.transform.SortChildren();
	}
	
	static public void SortChildrenRecursive(this GameObject parent){
		parent.transform.SortChildrenRecursive();
	}
	
	static public void DisconnectPrefab(this GameObject gameObject){
		#if UNITY_EDITOR
		if (gameObject.transform.parent == null){
			UnityEditor.PrefabUtility.DisconnectPrefabInstance(gameObject);
		}
		#endif
	}
	
	static public void RemoveComponent(this GameObject gameObject, Component component){
		if (Application.isPlaying) GameObject.Destroy(component);
		else GameObject.DestroyImmediate(component);
	}
	
	static public void RemoveComponent<T>(this GameObject gameObject) where T : Component {
		if (Application.isPlaying) GameObject.Destroy(gameObject.GetComponent<T>());
		else GameObject.DestroyImmediate(gameObject.GetComponent<T>());
	}
	
	static public T GetOrAddComponent<T>(this GameObject gameObject) where T : Component {
		T component = gameObject.GetComponent<T>();
		if (component == null) component = gameObject.AddComponent<T>();
		return component;
	}
	
	static public Component AddCopiedComponent(this GameObject copyTo, Component copyFrom){
		Component component = copyTo.AddComponent(copyFrom.GetType());
		component.Copy(copyFrom);
		return component;
	}
	
	static public Component[] AddCopiedComponents(this GameObject copyTo, params Component[] copyFrom){
		List<Component> components = new List<Component>();
		foreach (Component component in copyFrom){
			components.Add(copyTo.AddCopiedComponent(component));
		}
		return components.ToArray();
	}
	
	static public Component[] AddCopiedComponents(this GameObject copyTo, GameObject copyFrom, params Type[] typesToIgnore){
		List<Component> clonedComponents = new List<Component>();
		Component[] dstComponents = copyFrom.GetComponents(typeof(Component));
		
		foreach (Component dstComponent in dstComponents){
			if (!typesToIgnore.Contains(dstComponent.GetType())){
				if (dstComponent is Transform || (dstComponent is ParticleSystemRenderer && dstComponents.Contains(typeof(ParticleSystem)))) copyTo.CopyComponent(dstComponent);
				else{
					Component clonedComponent = copyTo.AddCopiedComponent(dstComponent);
					if (clonedComponent != null) clonedComponents.Add(clonedComponent);
				}
			}
		}
		return clonedComponents.ToArray();
	}
	
	static public Component CopyComponent(this GameObject copyTo, Component copyFrom){
		Component clonedComponent = copyTo.GetComponent(copyFrom.GetType());
		if (clonedComponent != null) clonedComponent.Copy(copyFrom);
		else Debug.LogError("Component of type " + copyFrom.GetType().ToString() + " was not found on the GameObject.");
		return clonedComponent;
	}
	
	static public Component[] CopyComponents(this GameObject copyTo, params Component[] copyFrom){
		List<Component> clonedComponents = new List<Component>();
		
		foreach (Component dstComponent in copyFrom){
			Component clonedComponent = copyTo.CopyComponent(dstComponent);
			if (clonedComponent != null) clonedComponents.Add(clonedComponent);
		}
		return clonedComponents.ToArray();
	}
	
	static public Component[] CopyComponents(this GameObject copyTo, GameObject copyFrom, params Type[] typesToIgnore){
		List<Component> clonedComponents = new List<Component>();
		Component[] dstComponents = copyFrom.GetComponents(typeof(Component));
		
		foreach (Component dstComponent in dstComponents){
			if (!typesToIgnore.Contains(dstComponent.GetType())){
				Component clonedComponent = copyTo.CopyComponent(dstComponent);
				if (clonedComponent != null) clonedComponents.Add(clonedComponent);
			}
		}
		return clonedComponents.ToArray();
	}
	#endregion
	
	#region MonoBehaviour
	static public GameObject[] GetChildren(this MonoBehaviour parent){
		List<GameObject> children = new List<GameObject>();
		foreach (var child in parent.transform.GetChildren()){
			children.Add(child.gameObject);
		}
		return children.ToArray();
	}
	
	static public GameObject[] GetChildrenRecursive(this MonoBehaviour parent){
		List<GameObject> children = new List<GameObject>();
		foreach (var child in parent.transform.GetChildrenRecursive()){
			children.Add(child.gameObject);
		}
		return children.ToArray();
	}
	
	static public int GetChildCount(this MonoBehaviour parent){
		return parent.transform.childCount;
	}
	
	static public GameObject GetChild(this MonoBehaviour parent, int index){
		return parent.transform.GetChild(index).gameObject;
	}
	
	static public GameObject FindChild(this MonoBehaviour parent, string childName){
		foreach (var child in parent.transform.GetChildren()){
			if (child.name == childName) return child.gameObject;
		}
		return null;
	}

	static public GameObject FindChildRecursive(this MonoBehaviour parent, string childName){
		foreach (var child in parent.transform.GetChildrenRecursive()){
			if (child.name == childName) return child.gameObject;
		}
		return null;
	}

	static public GameObject AddChild(this MonoBehaviour parent){
		return parent.transform.AddChild().gameObject;
	}
	
	static public GameObject AddChild(this MonoBehaviour parent, string childName){
		return parent.transform.AddChild(childName).gameObject;
	}
	
	static public GameObject FindOrAddChild(this MonoBehaviour parent, string childName){
		return parent.transform.FindOrAddChild(childName).gameObject;
	}
	
	static public void SortChildren(this MonoBehaviour parent){
		parent.transform.SortChildren();
	}
	
	static public void SortChildrenRecursive(this MonoBehaviour parent){
		parent.transform.SortChildrenRecursive();
	}
	
	static public void SetExecutionOrder(this MonoBehaviour script, int order){
		#if UNITY_EDITOR
		foreach (UnityEditor.MonoScript s in UnityEditor.MonoImporter.GetAllRuntimeMonoScripts()) {
			if (s.name == script.name){
				if (UnityEditor.MonoImporter.GetExecutionOrder(s) != -1){
					UnityEditor.MonoImporter.SetExecutionOrder(s, -1);
				}
			}
		}
		#endif
	}
	#endregion
	
	#region Component
	static public void Remove(this Component component){
		if (Application.isPlaying) GameObject.Destroy(component);
		else GameObject.DestroyImmediate(component);
	}
	#endregion
	
	#region Object
	static public void Copy<T>(this T copyTo, T copyFrom, params string[] parametersToIgnore) where T : class {
		if (typeof(T) == typeof(Component) || typeof(T).IsSubclassOf(typeof(Component))){
			List<string> parametersToIgnoreList = new List<string>(parametersToIgnore);
			parametersToIgnoreList.Add("name");
			parametersToIgnoreList.Add("tag");
			if (!(copyFrom is MonoBehaviour)){
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
		
		foreach (FieldInfo fieldInfo in copyFrom.GetType().GetFields()){
			if ((fieldInfo.IsPublic || fieldInfo.GetCustomAttributes(typeof(SerializeField), true).Length != 0) && !fieldInfo.IsLiteral && !parametersToIgnore.Contains(fieldInfo.Name)){
				try{
					fieldInfo.SetValue(copyTo, fieldInfo.GetValue(copyFrom));
				}
				catch{}
			}
		}
		foreach (PropertyInfo propertyInfo in copyFrom.GetType().GetProperties()){
			if (propertyInfo.CanWrite && !parametersToIgnore.Contains(propertyInfo.Name)){
				try{
					propertyInfo.SetValue(copyTo, propertyInfo.GetValue(copyFrom, null), null);
				}
				catch{}
			}
		}
	}
	#endregion
}
