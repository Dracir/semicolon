using UnityEngine;
using System.Collections;

public static class SpecialCharacterFactory  {

	public static GameObject createSpecialCharacter(GameObject parent, string character, string param, int x, float y){
		GameObject go = GameObjectFactory.createGameObject (character,parent);
		go.layer =  LayerMask.NameToLayer("Ignore Raycast");
		go.transform.Translate (x, y, 0);

		TextMesh tm = go.AddComponent<TextMesh>();
		
		TextCollider2D tc = go.AddComponent<TextCollider2D> ();
		tc.TextMesh = tm;
		tc.Text = character;
		tc.Font = GameConstantes.instance.currentTheme.instructionFont;
		
		addCharacterSpecifiqueFeature(go, character, param, parent);
		
		return go;
	}

	static void addCharacterSpecifiqueFeature(GameObject go, string character, string paramData, GameObject parent){
		string[] param = paramData.TrimEnd(new char[]{'\n','\r'}).Split(' ');
		string arg = param[1];
		if(arg.ToLower().Equals("isaspike")){
			var spikeManager = go.AddComponent<SpikeManager>();
			int spawnDelay = int.Parse(param[2]);
			spikeManager.spawnMinDelay = spawnDelay;
			spikeManager.spawnMaxDelay = spawnDelay * 4;
			References.SpikeMenagers.Add(spikeManager);
		} else if (arg.ToLower ().Equals ("isaspawner")){
			go.AddComponent<InstructionSpawner>();
		} else if (arg.ToLower ().Equals ("compiles")){
			go.AddComponent<CompileSemiColon>();
			Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
			rb.gravityScale = 0;
			rb.isKinematic = true;
		}
	}
}