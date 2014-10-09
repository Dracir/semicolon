using UnityEngine;
using System.Collections;

public class TextColliderFactory {

	public static TextCollider2D createTextCollider2D(string text, Transform parent, Vector3 position){
		GameObject go = GameObjectFactory.createGameObject(text,parent);
		go.transform.Translate(position);
		TextCollider2D tc = go.AddComponent<TextCollider2D>();
		tc.Text = text;
			
		return tc;
	}
}
