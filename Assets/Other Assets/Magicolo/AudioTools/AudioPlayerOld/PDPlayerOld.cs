using UnityEngine;
using LibPDBinding;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PDPlayerOld : MonoBehaviour {

	public Module[] modules = { };
	public CoroutineHolder coroutineHolder;
	public int sampleRate;
	public int bufferSize;
	public int bufferAmount;
	public AudioListener listener;
	public Transform listenerTransform;

	public bool showModules;
	
	public static Dictionary<string, Module> Modules;
	static PDPlayerOld instance;
	public static PDPlayerOld Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<PDPlayerOld>();
			}
			return instance;
		}
	}
	
	void Awake() {
		if (Application.isPlaying) {
			SetReferences();
			SetLibPD();
			
			foreach (Module module in modules) {
				module.Awake();
			}
		}
	}
	
	void Start() {
		if (Application.isPlaying) {
			foreach (Module module in modules) {
				module.Start();
			}
		}
		else {
			this.SetExecutionOrder(-8);
			if (FindObjectsOfType<PDPlayerOld>().Length > 1) {
				Debug.LogError("There can only be one PDPlayer.");
				DestroyImmediate(gameObject);
			}
		}
	}
	
	void Update() {
		if (!Application.isPlaying) {
			transform.hideFlags = HideFlags.HideInInspector;
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}
		else {
			foreach (Module module in modules) {
				module.Update();
			}
		}
	}

	void OnDestroy() {
		LibPD.Bang -= ReceiveDebugBang;
		LibPD.Float -= ReceiveDebugFloat;
		LibPD.Symbol -= ReceiveDebugSymbol;
		LibPD.List -= ReceiveDebugList;
		LibPD.Message -= ReceiveDebugMessage;
		LibPD.Unsubscribe("Debug");
	}
	
	void OnDrawGizmos() {
		#if UNITY_EDITOR
		foreach (Module module in modules) {
			if (module.source == null) return;
			
			Gizmos.DrawIcon(module.source.transform.position, "pd.png", true);
		    if (UnityEditor.Selection.gameObjects.Contains(gameObject) && module.showing) module.DrawGizmos();
		    else if (UnityEditor.Selection.gameObjects.Contains(module.source.gameObject) && module.showing) module.DrawGizmos();
		}
		#endif
	}
	
	void SetReferences() {
		coroutineHolder = gameObject.GetOrAddComponent<CoroutineHolder>();
		listener = FindObjectOfType<AudioListener>();
		listenerTransform = listener.transform;
		BuildModulesDict();
	}
	
	void SetLibPD() {
		LibPD.Subscribe("Debug");
		LibPD.Bang += ReceiveDebugBang;
		LibPD.Float += ReceiveDebugFloat;
		LibPD.Symbol += ReceiveDebugSymbol;
		LibPD.List += ReceiveDebugList;
		LibPD.Message += ReceiveDebugMessage;
		sampleRate = AudioSettings.outputSampleRate;
		AudioSettings.GetDSPBufferSize(out bufferSize, out bufferAmount);
		LibPD.SendFloat("BufferSize", bufferSize);
		LibPD.SendFloat("BufferAmount", bufferAmount);
		LibPD.SendFloat("SampleRate", sampleRate);
	}
	
	void BuildModulesDict() {
		Modules = new Dictionary<string, Module>();
		for (int i = 0; i < modules.Length; i++) {
			Modules[modules[i].name] = modules[i];
		}
	}
	
	void ReceiveDebugBang(string sendName) {
		if (sendName == "Debug") Debug.Log(string.Format("{0} received Bang", sendName));
	}
	
	void ReceiveDebugFloat(string sendName, float value) {
		if (sendName == "Debug") Debug.Log(string.Format("{0} received Float: {1}", sendName, value));
	}
	
	void ReceiveDebugSymbol(string sendName, string value) {
		if (sendName == "Debug") Debug.Log(string.Format("{0} received Symbol: {1}", sendName, value));
	}
	
	void ReceiveDebugList(string sendName, object[] value) {
		if (sendName == "Debug") Debug.Log(string.Format("{0} received List: {1}", sendName, Logger.ObjectToString(value)));
	}
	
	void ReceiveDebugMessage(string sendName, string message, object[] value) {
		if (sendName == "Debug") Debug.Log(string.Format("{0} received Message: {1} {2}", sendName, message, Logger.ObjectToString(value)));
	}
	
	public static void Play(string moduleName, float delay = 0, AudioPlayerOld.SyncMode syncMode = AudioPlayerOld.SyncMode.None) {
		if (Modules.ContainsKey(moduleName)) Modules[moduleName].Play(delay, syncMode);
	}
	
	public static void Stop(string moduleName, float delay = 0) {
		if (Modules.ContainsKey(moduleName)) Modules[moduleName].Stop(delay);
	}
	
	public static void Pause(string moduleName, float delay = 0) {
		if (Modules.ContainsKey(moduleName)) Modules[moduleName].Pause(delay);
	}
	
	public static void Resume(string moduleName, float delay = 0) {
		if (Modules.ContainsKey(moduleName)) Modules[moduleName].Resume(delay);
	}
	
	public static float GetVolume(string moduleName) {
		float volume = 0;
		if (Modules.ContainsKey(moduleName)) volume = Modules[moduleName].volume;
		return volume;
	}

	public static void SetVolume(string moduleName, float targetVolume) {
		if (Modules.ContainsKey(moduleName)) Modules[moduleName].SetVolume(targetVolume);
	}
	
	public static void SetVolume(string moduleName, float targetVolume, float time) {
		if (Modules.ContainsKey(moduleName)) Modules[moduleName].SetVolume(targetVolume, time);
	}
	
	public static bool SendValue(string sendName, object toSend) {
		int success = -1;
			
		if (toSend is int) success = LibPD.SendFloat(sendName, (float)((int)toSend));
		else if (toSend is int[]) success = LibPD.SendList(sendName, ((int[])toSend).ToFloatArray());
		else if (toSend is float) success = LibPD.SendFloat(sendName, (float)toSend);
		else if (toSend is float[]) success = LibPD.SendList(sendName, (float[])toSend);
		else if (toSend is double) success = LibPD.SendFloat(sendName, (float)((double)toSend));
		else if (toSend is double[]) success = LibPD.SendList(sendName, ((double[])toSend).ToFloatArray());
		else if (toSend is bool) success = LibPD.SendFloat(sendName, (float)((bool)toSend).GetHashCode());
		else if (toSend is bool[]) success = LibPD.SendList(sendName, ((bool[])toSend).ToFloatArray());
		else if (toSend is char) success = LibPD.SendSymbol(sendName, ((char)toSend).ToString());
		else if (toSend is char[]) success = LibPD.SendSymbol(sendName, new string((char[])toSend));
		else if (toSend is string) success = LibPD.SendSymbol(sendName, (string)toSend);
		else if (toSend is string[]) success = LibPD.SendList(sendName, (string[])toSend);
		else if (toSend is System.Enum) success = LibPD.SendFloat(sendName, (float)(toSend.GetHashCode()));
		else if (toSend is System.Enum[]) success = LibPD.SendList(sendName, ((System.Enum[])toSend).ToFloatArray());
		else if (toSend is Vector2) success = LibPD.SendList(sendName, ((Vector2)toSend).x, ((Vector2)toSend).y);
		else if (toSend is Vector3) success = LibPD.SendList(sendName, ((Vector3)toSend).x, ((Vector3)toSend).y, ((Vector3)toSend).z);
		else if (toSend is Vector4) success = LibPD.SendList(sendName, ((Vector4)toSend).x, ((Vector4)toSend).y, ((Vector4)toSend).z, ((Vector4)toSend).w);
		else if (toSend is Quaternion) success = LibPD.SendList(sendName, ((Quaternion)toSend).x, ((Quaternion)toSend).y, ((Quaternion)toSend).z, ((Quaternion)toSend).w);
		else if (toSend is Rect) success = LibPD.SendList(sendName, ((Rect)toSend).x, ((Rect)toSend).y, ((Rect)toSend).width, ((Rect)toSend).height);
		else if (toSend is Color) success = LibPD.SendList(sendName, ((Color)toSend).r, ((Color)toSend).g, ((Color)toSend).b, ((Color)toSend).a);
		else {
			Debug.LogError("Invalid type to send to Pure Data: " + toSend);
		}
			
		return success == 0;
	}
	
	public static string GetUniqueName(object[] array, string newName, string oldName = "", int suffix = 0) {
		bool uniqueName = false;
		string currentName = "";
		
		while (!uniqueName) {
			uniqueName = true;
			currentName = newName;
			if (suffix > 0) currentName += suffix.ToString();
			
			foreach (object obj in array) {
				if (obj is Module) {
					if (((Module)obj).name == currentName && ((Module)obj).name != oldName) {
						uniqueName = false;
						break;
					}
				}
			}
			suffix += 1;
		}
		return currentName;
	}
	
	public static string ToPDType(System.Type type) {
		string pdType = "";
		if (type.IsNumerical() || type == typeof(bool) || type == typeof(System.Enum) || type.IsSubclassOf(typeof(System.Enum))) pdType = type.Name + " -> Float";
		else if (type == typeof(string) || type == typeof(char)) pdType = type.Name + " -> Symbol";
		else if (type.IsVector() || type == typeof(Quaternion) || type == typeof(Rect) || type == typeof(Color)) pdType = type.Name + " -> List[Float]";
		else if (type.IsArray) {
			if (type.GetElementType().IsNumerical() || type.GetElementType() == typeof(bool)) pdType = type.Name + " -> List[Float]";
			else if (type.GetElementType() == typeof(string) || type.GetElementType() == typeof(char)) pdType = type.Name + " -> List[Symbol]";
		}
		return pdType;
	}
	
	
	[System.Serializable]
	public class Module {
		public enum RolloffMode {
			Logarithmic,
			Linear
		}

		public string Name;
		public string name {
			get { return Name; }
			set { Name = PDPlayerOld.GetUniqueName(Instance.modules, value, Name); }
		}
		[Tooltip("A receiver that wants to access these default send values must be named 'Prefix''Suffix'. The suffixes can be adapted to any naming convention, but changes might not be compatible with some Pure Data patches.")]
		public Suffixes suffixes = new Suffixes();
		[Tooltip("The module will be activated or not when the application starts depending on this value.")]
		public bool playOnAwake;
		[Tooltip("Current volume of the module.")]
		[Range(0, 100)] public float volume = 100;
		[Tooltip("If implemented, the module will be spatialized and attenuated using the source's position in relation to the listener's position.\nIf left empty, the module will not be spatialized or attenuated.")]
		public GameObject source;
		[Tooltip("Curve of the distance attenuation.")]
		public RolloffMode volumeRolloff;
		[Tooltip("Distance at which the module starts to be attenuated.")]
		[Min(0)] public float minDistance = 5;
		[Tooltip("Distance at which the module has been completely attenuated.")]
		[Min(0)] public float maxDistance = 30;
		[Tooltip("Controls how much panning is applied.")]
		[Range(0, 1)] public float panLevel = 0.75F;
		public Send[] sends = { };
		public bool isPlaying;
		
		public bool showing;
		public bool showSends;
		
		bool changed = true;
		RolloffMode pVolumeRolloff;
		float pMinDistance;
		float pMaxDistance;
		float pPanLevel;
		
		public void Awake() {
			foreach (Send send in sends) {
				send.Awake();
			}
		}
		
		public void Start() {
			SetVolume(volume);
			if (playOnAwake) Play();
		}
		
		public void Update() {
			CheckForChanges();
			
			if (changed) {
				Spatialize();
			}
			
			foreach (Send send in sends) {
				send.Update(name);
			}
		}
	
		public void DrawGizmos() {
			Gizmos.color = new Color(0.25F, 0.5F, 0.75F, 1);
			Gizmos.DrawWireSphere(source.transform.position, minDistance);
			Gizmos.color = new Color(0.25F, 0.75F, 0.5F, 0.35F);
			Gizmos.DrawWireSphere(source.transform.position, maxDistance);
		}
		
		public void Play(float delay = 0, AudioPlayerOld.SyncMode syncMode = AudioPlayerOld.SyncMode.None) {
			delay = (float)(AudioPlayerOld.GetDelayToSync(syncMode) + AudioPlayerOld.GetAdjustedDelay(delay, syncMode));
			Instance.coroutineHolder.AddCoroutine("PDPlayerPlayAfterDelay", PlayAfterDelay(delay));
		}
		
		public void Stop(float delay = 0) {
			Instance.coroutineHolder.AddCoroutine("PDPlayerStopAfterDelay", StopAfterDelay(delay));
		}
		
		public void Pause(float delay = 0) {
			Instance.coroutineHolder.AddCoroutine("PDPlayerPauseAfterDelay", PauseAfterDelay(delay));
		}
		
		public void Resume(float delay = 0) {
			Instance.coroutineHolder.AddCoroutine("PDPlayerResumeAfterDelay", ResumeAfterDelay(delay));
		}
		
		public void SetVolume(float targetVolume) {
			volume = Mathf.Clamp(targetVolume, 0, 100);
			PDPlayerOld.SendValue(name + suffixes.setVolume, volume / 100);
		}
		
		public void SetVolume(float targetVolume, float time) {
			volume = Mathf.Clamp(targetVolume, 0, 100);
			PDPlayerOld.SendValue(name + suffixes.setVolume, new [] {
				volume / 100,
				Mathf.Max(time, 0.05F) * 1000
			});
		}
	
		public bool SendValue(string sendSuffix, object toSend) {
			return PDPlayerOld.SendValue(name + sendSuffix, toSend);
		}
		
		IEnumerator PlayAfterDelay(float delay = 0) {
			float counter = 0;
			while (counter < delay) {
				yield return new WaitForSeconds(0);
				counter += Time.deltaTime;
			}
			
			isPlaying = true;
			PDPlayerOld.SendValue(name + suffixes.play, 1);
		}
		
		IEnumerator StopAfterDelay(float delay = 0) {
			float counter = 0;
			while (counter < delay) {
				yield return new WaitForSeconds(0);
				counter += Time.deltaTime;
			}
			
			isPlaying = false;
			PDPlayerOld.SendValue(name + suffixes.stop, 0);
		}
		
		IEnumerator PauseAfterDelay(float delay = 0) {
			float counter = 0;
			while (counter < delay) {
				yield return new WaitForSeconds(0);
				counter += Time.deltaTime;
			}
			
			PDPlayerOld.SendValue(name + suffixes.pause, 0);
		}
		
		IEnumerator ResumeAfterDelay(float delay = 0) {
			float counter = 0;
			while (counter < delay) {
				yield return new WaitForSeconds(0);
				counter += Time.deltaTime;
			}
			
			PDPlayerOld.SendValue(name + suffixes.resume, 1);
		}

		void CheckForChanges() {
			if (pVolumeRolloff != volumeRolloff || pMinDistance != minDistance || pMaxDistance != maxDistance || pPanLevel != panLevel) {
				changed = true;
				pVolumeRolloff = volumeRolloff;
				pMinDistance = minDistance;
				pMaxDistance = maxDistance;
				pPanLevel = panLevel;
			}
			if (source != null && (source.transform.hasChanged || Instance.listenerTransform.hasChanged)) {
				changed = true;
				Instance.SetTransformHasChanged(source.transform, false);
				Instance.SetTransformHasChanged(Instance.listenerTransform, false);
			}
		}

		void Spatialize() {
			if (source == null) {
				PDPlayerOld.SendValue(name + "_HRFLeft", 20000);
				PDPlayerOld.SendValue(name + "_HRFRight", 20000);
				PDPlayerOld.SendValue(name + "_PanLeft", 1);
				PDPlayerOld.SendValue(name + "_PanRight", 1);
				PDPlayerOld.SendValue(name + "_Attenuation", 1);
			}
			else {
				Vector3 listenerToSource = source.transform.position - Instance.listenerTransform.position;
				float angle = Vector3.Angle(Instance.listenerTransform.right, listenerToSource);
				float panLeft = (1 - panLevel) + panLevel * Mathf.Sin(Mathf.Max(180 - angle, 90) * Mathf.Deg2Rad);
				float panRight = (1 - panLevel) + panLevel * Mathf.Sin(Mathf.Max(angle, 90) * Mathf.Deg2Rad);
				
				const float fullFrequencyRange = 20000;
				const float hrfFactor = 1500;
				
				float behindFactor = 1 + 4 * (Mathf.Clamp(Vector3.Angle(listenerToSource, Instance.listenerTransform.forward), 90, 135) - 90) / (135 - 90);
				float hrfLeft = Mathf.Pow(panLeft, 2) * (fullFrequencyRange - hrfFactor) / behindFactor + hrfFactor;
				float hrfRight = Mathf.Pow(panRight, 2) * (fullFrequencyRange - hrfFactor) / behindFactor + hrfFactor;
				float distance = Vector3.Distance(source.transform.position, Instance.listenerTransform.position);
				float adjustedDistance = Mathf.Clamp01(Mathf.Max(distance - minDistance, 0) / Mathf.Max(maxDistance - minDistance, 0.001F));
				
				float attenuation;
				if (volumeRolloff == RolloffMode.Linear) {
					attenuation = 1F - adjustedDistance;
				}
				else {
					const float curveDepth = 2;
					attenuation = Mathf.Pow((1F - Mathf.Pow(adjustedDistance, 1F / curveDepth)), curveDepth);
				}
				
				PDPlayerOld.SendValue(name + "_HRFLeft", hrfLeft);
				PDPlayerOld.SendValue(name + "_HRFRight", hrfRight);
				PDPlayerOld.SendValue(name + "_PanLeft", panLeft);
				PDPlayerOld.SendValue(name + "_PanRight", panRight);
				PDPlayerOld.SendValue(name + "_Attenuation", attenuation);
				PDPlayerOld.SendValue(name + "_SourcePosition", source.transform.position);
				PDPlayerOld.SendValue(name + "_ListenerAngle", angle);
				PDPlayerOld.SendValue(name + "_ListenerDistance", distance);
			}
			changed = false;
		}
	}
	
	
	[System.Serializable]
	public class Suffixes {
		
		[Tooltip("A '1' is sent when the module's 'Play' function is called.\nImplemented withinin the [uvolpan~] Pure Data object.")]
		public string play = "_Play";
		[Tooltip("A '0' is sent when the module's 'Stop' function is called.\nImplemented withinin the [uvolpan~] Pure Data object.")]
		public string stop = "_Stop";
		[Tooltip("A '0' is sent when the module's 'Pause' function is called.\nThis function is NOT implemented by default within the [uvolpan~] Pure Data object.")]
		public string pause = "_Pause";
		[Tooltip("A '1' is sent when the module's 'Resume' function is called.\nThis function is NOT implemented by default within the [uvolpan~] Pure Data object.")]
		public string resume = "_Resume";
		[Tooltip("The current normalized volume (between '0' and '1') is sent when the module's 'SetVolume' function is called.\nImplemented withinin the [uvolpan~] Pure Data object.")]
		public string setVolume = "_Volume";
	}
	
	
	[System.Serializable]
	public class Send {
		
		public enum SendMode {
			Continuous,
			OnChange

		}
		public enum SendTypes {
			Audio,
			Data
		}
		
		[Tooltip("A receiver must be named 'Prefix''Suffix' to access the sent data.")]
		public string suffix = "_default";
		public SendTypes type = SendTypes.Data;
		[Tooltip("Continuous: The value is sent to Pure Data on each frame.\nOnChange: The value is sent to Pure Data only when it is modified.")]
		public SendMode mode;
		
		[Separator]
		public AudioSource audioSource;
		[Separator]
		public GameObject currentObject;
		public Object currentComponent;
		public object objectToSend;
		object pObjectToSend;
		public List<MemberInfo> members;
		
		public int componentIndex;
		public int valueIndex;
		public bool changed;
		public bool showing;
		
		static readonly public List<System.Type> ValidTypes = new List<System.Type>() {
			typeof(int),
			typeof(int[]),
			typeof(float),
			typeof(float[]),
			typeof(double),
			typeof(double[]),
			typeof(bool),
			typeof(bool[]),
			typeof(char),
			typeof(char[]),
			typeof(string), 
			typeof(string[]),
			typeof(System.Enum),
			typeof(System.Enum[]),
			typeof(Vector2), 
			typeof(Vector3), 
			typeof(Vector4), 
			typeof(Quaternion), 
			typeof(Rect), 
			typeof(Color)
		};
		
		public void Awake() {
			SetObjectToSend();
		}
		
		public void Update(string prefix) {
			if (objectToSend == null) return;
			
			UpdateObjectToSend();
			
			switch (mode) {
				case SendMode.Continuous:
					SendValue(prefix + suffix, objectToSend);
					break;
				case SendMode.OnChange:
					if (pObjectToSend == null) {
						if (pObjectToSend != objectToSend) {
							pObjectToSend = objectToSend;
							changed = false;
							SendValue(prefix + suffix, objectToSend);
						}
					}
					else if (!pObjectToSend.Equals(objectToSend) || changed) {
						pObjectToSend = objectToSend;
						changed = false;
						SendValue(prefix + suffix, objectToSend);
					}
					break;
			}
		}

		public void UpdateObjectToSend() {
			if (members.Count > 0) {
				if (members[valueIndex].MemberType == MemberTypes.Field) objectToSend = ((FieldInfo)members[valueIndex]).GetValue(currentComponent);
				else if (members[valueIndex].MemberType == MemberTypes.Property) objectToSend = ((PropertyInfo)members[valueIndex]).GetValue(currentComponent, null);
			}
		}
		
		public void SetObjectToSend() {
			members = new List<MemberInfo>();
			
			if (currentComponent != null) {
				foreach (MemberInfo member in currentComponent.GetType().GetMembers()) {
					System.Type memberType = null;
					if (member.MemberType == MemberTypes.Field) memberType = ((FieldInfo)member).FieldType;
					else if (member.MemberType == MemberTypes.Property) memberType = ((PropertyInfo)member).PropertyType;
					
					if (memberType != null) {
						foreach (System.Type type in ValidTypes) {
							if (memberType == type || memberType.IsSubclassOf(type)) {
								members.Add(member);
								break;
							}
						}
					}
				}
				UpdateObjectToSend();
			}
			else {
				componentIndex = 0;
				valueIndex = 0;
				currentComponent = null;
				objectToSend = null;
			}
		}
	}
}