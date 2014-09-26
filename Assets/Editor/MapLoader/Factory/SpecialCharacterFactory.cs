using UnityEngine;
using System.Collections;

public static class SpecialCharacterFactory  {

	public static GameObject createSpecialCharacter(GameObject parent, string character, string param, int x, float y){
		GameObject go = GameObjectFactory.createGameObject (character,parent);
		go.layer =  LayerMask.NameToLayer("Ignore Raycast");
		go.transform.Translate (x, y, 0);

		TextMesh tm = go.AddComponent<TextMesh>();
		
		TextCollider2D tc = go.AddComponent<TextCollider2D> ();
		tc.textMesh = tm;
		tc.Text = character;
		tc.Font = GameConstantes.instance.currentTheme.statementFont;
		
		addCharacterSpecifiqueFeature(go, character, param, parent);
		
		return go;
	}

	static void addCharacterSpecifiqueFeature(GameObject go, string character, string paramData, GameObject parent){
		string[] param = paramData.TrimEnd(new char[]{'\n','\r'}).Split(' ');
		string arg = param[1];
		int bob = int.Parse(param[2]);
		if(arg.ToLower().Equals("isaspike")){
			var spikeMenager = go.AddComponent<SpikeManager>();
			spikeMenager.spawnInterval = bob;
		}
	}
}