using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[SelectionBase]
public class TextCollider2D : MonoBehaviour {

	[TextArea] public string text;
	public int fontSize = 100;
	public FontStyle fontStyle;
	public Font font;
	public Color color = Color.white;
	public Vector2 colliderSize = Vector2.one;
	public bool colliderIsTrigger;

	List<BoxCollider2D> boxColliders;
	GameObject textMeshObject;
	TextMesh textMesh;
	MeshRenderer meshRenderer;

	string pText;
	Font pFont;
	int pFontSize;
	FontStyle pFontStyle;
	Color pColor;
	Vector2 pColliderSize;
	bool pTrigger;

	void Awake(){
		textMeshObject = gameObject.FindOrAddChild("TextMesh");
		textMeshObject.transform.Reset();
		textMeshObject.transform.localScale = Vector3.one / 10;
		meshRenderer = textMeshObject.GetOrAddComponent<MeshRenderer>();
		textMesh = textMeshObject.GetOrAddComponent<TextMesh>();
		boxColliders = new List<BoxCollider2D>(textMeshObject.GetComponents<BoxCollider2D>());
		pText = text;
		pFont = font;
		pFontSize = fontSize;
		pFontStyle = fontStyle;
		UpdateTextMesh();
	}

	void Update(){
		if (Application.isPlaying){
			if (pText != text || pFont != font || pFontSize != fontSize || pFontStyle != fontStyle || pColor != color || pColliderSize != colliderSize || pTrigger != colliderIsTrigger){
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
		else Preview();
	}

	void UpdateTextMesh(){
		if (string.IsNullOrEmpty(text) || fontSize == 0){
			foreach (BoxCollider2D boxCollider in boxColliders){
				boxCollider.Remove();
			}
			textMesh.text = text;
			textMesh.fontSize = fontSize;
			textMesh.fontStyle = fontStyle;
			textMesh.font = font;
			if (font != null) meshRenderer.material = font.material;
			textMesh.color = color;
		}
		else if (font != null){
			textMesh.text = text;
			textMesh.fontSize = fontSize;
			textMesh.fontStyle = fontStyle;
			textMesh.font = font;
			meshRenderer.material = font.material;
			textMesh.color = color;

			string[] lines = text.Split('\n');

			for (int i = lines.Length; i < boxColliders.Count; i++){
				boxColliders[i].Remove();
				boxColliders.RemoveAt(i);
			}

			float heightSum = 0;
			for (int i = 0; i < lines.Length; i++){
				if (i >= boxColliders.Count) boxColliders.Add(textMeshObject.AddComponent<BoxCollider2D>());
				BoxCollider2D boxCollider = boxColliders[i];
				boxCollider.isTrigger = colliderIsTrigger;

				CharacterInfo charInfo;
				font.GetCharacterInfo(' ', out charInfo, fontSize, fontStyle);
				float xOffset = (lines[i].Length - lines[i].TrimStart().Length) * charInfo.width / 10;

				Rect textRect;
				if (string.IsNullOrEmpty(lines[i].TrimStart())) textRect = "\n".GetRect(font, fontSize, fontStyle);
				else textRect = lines[i].TrimStart().TrimEnd().GetRect(font, fontSize, fontStyle);

				float width = (textRect.width * colliderSize.x) / 10;
				float height = (textRect.height * colliderSize.y) / 10;
				float x = (width / 2) + (textRect.width / 10 - width) / 2 + xOffset;
				float y = heightSum - (height + (textRect.height / 10 - height)) / 2;
				boxCollider.size = new Vector2(width, height);
				boxCollider.center = new Vector2(x, y);
				heightSum -= textRect.height / 10;
			}
		}
	}

	void Preview(){
		textMeshObject = gameObject.FindOrAddChild("TextMesh");
		textMeshObject.transform.Reset();
		textMeshObject.transform.localScale = Vector3.one / 10;
		meshRenderer = textMeshObject.GetOrAddComponent<MeshRenderer>();
		textMesh = textMeshObject.GetOrAddComponent<TextMesh>();
		boxColliders = new List<BoxCollider2D>(textMeshObject.GetComponents<BoxCollider2D>());

		pText = textMesh.text;
		pFontSize = textMesh.fontSize;
		pFontStyle = textMesh.fontStyle;
		pFont = textMesh.font;
		pColor = textMesh.color;
		if (pText != text || pFont != font || pFontSize != fontSize || pFontStyle != fontStyle || pColor != color || pColliderSize != colliderSize || pTrigger != colliderIsTrigger){
			pColliderSize = colliderSize;
			pTrigger = colliderIsTrigger;
			UpdateTextMesh();
		}
	}
}
