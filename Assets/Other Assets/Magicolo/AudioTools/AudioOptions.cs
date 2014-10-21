using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

namespace Magicolo.AudioTools {
	public class AudioOptions : MonoBehaviour, INamable {

		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}

		AudioSource source;
		public AudioSource Source {
			get {
				source = source ?? this.GetOrAddComponent<AudioSource>();
				return source;
			}
		}

		public AudioInfo audioInfo;
		public AudioHierarchyManager hierarchyManager;
		public AudioPlayer audioPlayer;
		
		string pName;
		
		public void Update() {
			if (!Application.isPlaying) {
				#if UNITY_EDITOR
				if (pName != Name) {
					string clipPath = UnityEditor.AssetDatabase.GetAssetPath(Source.clip);
					this.SetUniqueName(Name, "", hierarchyManager.audioOptions);
					UnityEditor.AssetDatabase.RenameAsset(clipPath, Name);
					pName = Name;
				}
				else {
					Name = Source.clip.name;
				}
				#endif
			}
		}
	}
}
