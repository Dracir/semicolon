using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[SelectionBase]
public class TextCollider2D : MonoBehaviour {

	[HideInInspector] public TextMesh[] childrenTextMesh;
	[PopupSelector("childrenTextMesh")] public TextMesh textMesh;
	
	[TextArea] public string text;
	[Min] public int fontSize = 166;
	public FontStyle fontStyle;
	public Font font;
	public Color color = Color.white;
	public Vector2 colliderSize = Vector2.one;
	public bool colliderIsTrigger;

	List<BoxCollider2D> boxColliders;
	MeshRenderer meshRenderer;

	string pText;
	Font pFont;
	int pFontSize;
	FontStyle pFontStyle;
	Color pColor;
	Vector2 pColliderSize;
	bool pTrigger;

	void OnEnable() {
		if (textMesh == null) {
			if (gameObject.GetComponentsInChildren<TextMesh>().Length == 0) {
				GameObject textMeshObject = gameObject.AddChild("TextMesh");
				textMeshObject.transform.Reset();
				textMeshObject.AddComponent<TextMesh>();
			}
		}
		else {
			meshRenderer = textMesh.GetOrAddComponent<MeshRenderer>();
			boxColliders = new List<BoxCollider2D>(textMesh.GetComponents<BoxCollider2D>());
			if (font == null)
				font = References.Fonts.LucidiaConsole;
			pText = text;
			pFont = font;
			pFontSize = fontSize;
			pFontStyle = fontStyle;
			UpdateTextMesh();
		}
		Preview();
	}

	void Update() {
		if (Application.isPlaying) {
			if (pText != text || pFont != font || pFontSize != fontSize || pFontStyle != fontStyle || pColor != color || pColliderSize != colliderSize || pTrigger != colliderIsTrigger) {
				pText = text;
				pFont = font;
				pFontSize = fontSize;
				pFontStyle = fontStyle;
				pColor = color;
				pColliderSize = colliderSize;
				pTrigger = colliderIsTrigger;
				UpdateTextMesh();
			}
		}
		else
			Preview();
	}

	void UpdateTextMesh() {
		if (string.IsNullOrEmpty(text) || fontSize == 0) {
			foreach (BoxCollider2D boxCollider in boxColliders) {
				boxCollider.Remove();
			}
			textMesh.text = text;
			textMesh.fontSize = fontSize;
			textMesh.fontStyle = fontStyle;
			textMesh.font = font;
			if (font != null)
				meshRenderer.material = font.material;
			textMesh.color = color;
		}
		else if (font != null) {
			textMesh.text = text;
			textMesh.fontSize = fontSize;
			textMesh.fontStyle = fontStyle;
			textMesh.font = font;
			meshRenderer.material = font.material;
			textMesh.color = color;

			string[] lines = text.Split('\n');

			for (int i = lines.Length; i < boxColliders.Count; i++) {
				boxColliders[i].Remove();
				boxColliders.RemoveAt(i);
			}

			float heightSum = 0;
			for (int i = 0; i < lines.Length; i++) {
				if (i >= boxColliders.Count)
					boxColliders.Add(textMesh.AddComponent<BoxCollider2D>());
				BoxCollider2D boxCollider = boxColliders[i];
				boxCollider.isTrigger = colliderIsTrigger;

				CharacterInfo charInfo;
				font.GetCharacterInfo(' ', out charInfo, fontSize, fontStyle);
				float xOffset = (lines[i].Length - lines[i].TrimStart().Length) * charInfo.width / 100;

				Rect textRect;
				if (string.IsNullOrEmpty(lines[i].TrimStart()))
					textRect = "\n".GetRect(font, fontSize, fontStyle);
				else
					textRect = lines[i].TrimStart().TrimEnd().GetRect(font, fontSize, fontStyle);

				float width = (textRect.width * colliderSize.x) / 100;
				float height = (textRect.height * colliderSize.y) / 100;
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
			if (font == null)
				font = References.Fonts.LucidiaConsole;

			pText = textMesh.text;
			pFontSize = textMesh.fontSize;
			pFontStyle = textMesh.fontStyle;
			pFont = textMesh.font;
			pColor = textMesh.color;
			if (pText != text || pFont != font || pFontSize != fontSize || pFontStyle != fontStyle || pColor != color || pColliderSize != colliderSize || pTrigger != colliderIsTrigger) {
				pColliderSize = colliderSize;
				pTrigger = colliderIsTrigger;
				UpdateTextMesh();
			}
		}
	}
}
