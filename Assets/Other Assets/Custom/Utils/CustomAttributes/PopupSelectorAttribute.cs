using UnityEngine;

public sealed class PopupSelectorAttribute : CustomAttributeBase {
	
	public string arrayName;
	public string onChangeCallback = "";
	
	public PopupSelectorAttribute(string arrayName) {
		this.arrayName = arrayName;
	}
	
	public PopupSelectorAttribute(string arrayName, string onChangeCallback) {
		this.arrayName = arrayName;
		this.onChangeCallback = onChangeCallback;
	}
}
