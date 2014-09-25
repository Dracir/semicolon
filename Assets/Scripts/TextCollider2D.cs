using UnityEngine;
using Candlelight;
using System.Collections.Generic;

[ExecuteInEditMode]
[SelectionBase]
public class TextCollider2D : MonoBehaviour {

	[HideInInspector] public TextMesh[] childrenTextMesh;
	[PopupSelector("childrenTextMesh")] public TextMesh textMesh;
	
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
	
	List<BoxCollider2D> boxColliders;
	MeshRenderer meshRenderer;

	void OnEnable() {
		if (textMesh == null) {
			this.GetOrAddComponent<TextMesh>();
		}
		else {
			meshRenderer = textMesh.GetOrAddComponent<MeshRenderer>();
			boxColliders = new List<BoxCollider2D>(textMesh.GetComponents<BoxCollider2D>());
			if (Font == null)
				Font = References.Fonts.LucidiaConsole;
			UpdateTextMesh();
		}
		Preview();
	}

//	public void Update() {
//		if (!Application.isPlaying)
//			Preview();
//		else {
//			if (pText != Text || pFont != Font || pFontSize != FontSize || pFontStyle != FontStyle || pColor != Color || pColliderSize != ColliderSize || pTrigger != ColliderIsTrigger) {
//				pText = Text;
//				pFont = Font;
//				pFontSize = FontSize;
//				pFontStyle = FontStyle;
//				pColor = Color;
//				pColliderSize = ColliderSize;
//				pTrigger = ColliderIsTrigger;
//				UpdateTextMesh();
//			}
//		}
//	}

	void UpdateTextMesh() {
		if (string.IsNullOrEmpty(Text) || FontSize == 0) {
			foreach (BoxCollider2D boxCollider in boxColliders) {
				boxCollider.Remove();
			}
			textMesh.text = Text;
			textMesh.fontSize = FontSize;
			textMesh.fontStyle = FontStyle;
			textMesh.font = Font;
			if (Font != null)
				meshRenderer.material = Font.material;
			textMesh.color = Color;
		}
		else if (Font != null) {
			textMesh.text = Text;
			textMesh.fontSize = FontSize;
			textMesh.fontStyle = FontStyle;
			textMesh.font = Font;
			meshRenderer.material = Font.material;
			textMesh.color = Color;

			string[] lines = Text.Split('\n');

			for (int i = lines.Length; i < boxColliders.Count; i++) {
				boxColliders[i].Remove();
				boxColliders.RemoveAt(i);
			}

			float heightSum = 0;
			for (int i = 0; i < lines.Length; i++) {
				if (i >= boxColliders.Count)
					boxColliders.Add(textMesh.AddComponent<BoxCollider2D>());
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
		
		if (textMesh == null){
			textMesh = ownTextMesh;
		}
		
		if (textMesh != null) {
			textMesh.anchor = TextAnchor.UpperLeft;
			textMesh.characterSize = 0.1F;
			meshRenderer = textMesh.GetOrAddComponent<MeshRenderer>();
			boxColliders = new List<BoxCollider2D>(textMesh.GetComponents<BoxCollider2D>());
			if (Font == null)
				Font = References.Fonts.LucidiaConsole;

			UpdateTextMesh();
		}
	}
}
