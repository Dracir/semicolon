using UnityEngine;
using Candlelight;
using System.Collections.Generic;

[ExecuteInEditMode]
[SelectionBase]
public class TextCollider2D : MonoBehaviour {

	[HideInInspector] public TextMesh[] childrenTextMesh;
	[SerializeField, PropertyBackingField(typeof(TextCollider2D), "Text", typeof(PopupSelectorAttribute), "childrenTextMesh")]
	TextMesh textMesh;
	public TextMesh TextMesh {
		get {
			return textMesh;
		}
		set {
			textMesh = value;
			if (Application.isPlaying) {
				UpdateTextMesh();
			}
			else {
				Preview();
			}
		}
	}
	
	[SerializeField, PropertyBackingField(typeof(TextCollider2D), "Text", typeof(TextAreaAttribute))]
	string text = "";
	public string Text {
		get {
			return text;
		}
		set {
			text = value;
			if (Application.isPlaying) {
				UpdateTextMesh();
			}
			else {
				Preview();
			}
		}
	}
	
	[SerializeField, PropertyBackingField(typeof(TextCollider2D), "FontSize", typeof(MinAttribute))]
	int fontSize = 166;
	public int FontSize {
		get {
			return fontSize;
		}
		set {
			fontSize = value;
			if (Application.isPlaying) {
				UpdateTextMesh();
			}
			else {
				Preview();
			}
		}
	}
	
	[SerializeField, PropertyBackingField(typeof(TextCollider2D), "FontStyle")]
	FontStyle fontStyle;
	public FontStyle FontStyle {
		get {
			return fontStyle;
		}
		set {
			fontStyle = value;
			if (Application.isPlaying) {
				UpdateTextMesh();
			}
			else {
				Preview();
			}
		}
	}
	
	[SerializeField, PropertyBackingField(typeof(TextCollider2D), "Font")]
	Font font;
	public Font Font {
		get {
			return font;
		}
		set {
			font = value;
			if (Application.isPlaying) {
				UpdateTextMesh();
			}
			else {
				Preview();
			}
		}
	}
	
	[SerializeField, PropertyBackingField(typeof(TextCollider2D), "Color")]
	Color color = Color.white;
	public Color Color {
		get {
			return color;
		}
		set {
			color = value;
			if (Application.isPlaying) {
				UpdateTextMesh();
			}
			else {
				Preview();
			}
		}
	}
	
	[SerializeField, PropertyBackingField(typeof(TextCollider2D), "ColliderSize")]
	Vector2 colliderSize = Vector2.one;
	public Vector2 ColliderSize {
		get {
			return colliderSize;
		}
		set {
			colliderSize = value;
			if (Application.isPlaying) {
				UpdateTextMesh();
			}
			else {
				Preview();
			}
		}
	}
	
	[SerializeField, PropertyBackingField(typeof(TextCollider2D), "ColliderIsTrigger")]
	bool colliderIsTrigger;
	public bool ColliderIsTrigger {
		get {
			return colliderIsTrigger;
		}
		set {
			colliderIsTrigger = value;
			if (Application.isPlaying) {
				UpdateTextMesh();
			}
			else {
				Preview();
			}
		}
	}
	
	List<BoxCollider2D> boxColliders = new List<BoxCollider2D>();
	MeshRenderer meshRenderer;

	void OnEnable() {
		if (TextMesh == null) {
			this.GetOrAddComponent<TextMesh>();
		}
		else {
			meshRenderer = TextMesh.GetOrAddComponent<MeshRenderer>();
			boxColliders = new List<BoxCollider2D>(TextMesh.GetComponents<BoxCollider2D>());
			if (Font == null)
				Font = References.Fonts.LucidiaConsole;
			UpdateTextMesh();
		}
		Preview();
	}

	void UpdateTextMesh() {
		if (string.IsNullOrEmpty(Text) || FontSize == 0) {
			foreach (BoxCollider2D boxCollider in boxColliders) {
				boxCollider.Remove();
			}
			TextMesh.text = Text;
			TextMesh.fontSize = FontSize;
			TextMesh.fontStyle = FontStyle;
			TextMesh.font = Font;
			if (Font != null && meshRenderer != null)
				meshRenderer.material = Font.material;
			TextMesh.color = Color;
		}
		else if (Font != null) {
			TextMesh.text = Text;
			TextMesh.fontSize = FontSize;
			TextMesh.fontStyle = FontStyle;
			TextMesh.font = Font;
			TextMesh.color = Color;
			if (Font != null && meshRenderer != null) {
				meshRenderer.material = Font.material;
			}

			string[] lines = Text.Split('\n');

			for (int i = lines.Length; i < boxColliders.Count; i++) {
				boxColliders[i].Remove();
				boxColliders.RemoveAt(i);
			}

			float heightSum = 0;
			for (int i = 0; i < lines.Length; i++) {
				if (i >= boxColliders.Count)
					boxColliders.Add(TextMesh.AddComponent<BoxCollider2D>());
				BoxCollider2D boxCollider = boxColliders[i];
				boxCollider.isTrigger = ColliderIsTrigger;

				CharacterInfo charInfo;
				Font.GetCharacterInfo(' ', out charInfo, FontSize, FontStyle);
				float xOffset = (lines[i].Length - lines[i].TrimStart().Length) * charInfo.width / 100;

				Rect textRect;
				if (string.IsNullOrEmpty(lines[i].TrimStart()))
					textRect = "\n".GetRect(Font, FontSize, FontStyle);
				else
					textRect = lines[i].TrimStart().TrimEnd().GetRect(Font, FontSize, FontStyle);

				float width = (textRect.width * ColliderSize.x) / 100;
				float height = (textRect.height * ColliderSize.y) / 100;
				float x = (width / 2) + (textRect.width / 100 - width) / 2 + xOffset;
				float y = heightSum - (height + (textRect.height / 100 - height)) / 2;
				boxCollider.size = new Vector2(width, height);
				boxCollider.center = new Vector2(x, y);
				heightSum -= textRect.height / 100;
			}
		}
	}

	void Preview() {
		List<TextMesh> childrenTextMeshList = new List<TextMesh>();
		TextMesh ownTextMesh = GetComponent<TextMesh>();
		if (ownTextMesh != null) {
			childrenTextMeshList.Add(ownTextMesh);
		}
		foreach (TextMesh child in GetComponentsInChildren<TextMesh>()) {
			if (child != null && !childrenTextMeshList.Contains(child)) {
				childrenTextMeshList.Add(child);
			}
		}                         
		childrenTextMesh = childrenTextMeshList.ToArray();
		
		if (TextMesh == null){
			TextMesh = ownTextMesh;
		}
		
		if (TextMesh != null) {
			TextMesh.anchor = TextAnchor.UpperLeft;
			TextMesh.characterSize = 0.1F;
			meshRenderer = TextMesh.GetOrAddComponent<MeshRenderer>();
			boxColliders = new List<BoxCollider2D>(TextMesh.GetComponents<BoxCollider2D>());
			if (Font == null)
				Font = References.Fonts.LucidiaConsole;

			UpdateTextMesh();
		}
	}
}
