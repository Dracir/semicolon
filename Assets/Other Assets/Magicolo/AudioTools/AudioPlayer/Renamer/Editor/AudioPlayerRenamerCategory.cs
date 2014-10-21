using System.Collections.Generic;
using Magicolo.GeneralTools;
using UnityEngine;
using System.Collections;

namespace Magicolo.AudioTools {
	[System.Serializable]
	public class AudioPlayerRenamerCategory : INamable {

		public string Name { get; set; }
		public List<string> tags = new List<string>();
		public string selectedTag = "";
		public int selectedTagIndex;
		
		public string tagNameToAdd;
	
		public AudioPlayerRenamerCategory() {
			Name = "default";
		}
	
		public AudioPlayerRenamerCategory(string name) {
			this.Name = name;
		}
		
		public AudioPlayerRenamerCategory(string name, params string[] tags) {
			this.Name = name;
			foreach (string tag in tags) {
				if (!string.IsNullOrEmpty(tag)) {
					this.tags.Add(tag);
				}
			}
			this.tags.Sort();
		}
		
		public void AddTag(string tag) {
			tags.Add(tag);
			tags.Sort();
		}
	}
}
