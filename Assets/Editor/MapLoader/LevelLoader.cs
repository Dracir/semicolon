using UnityEngine;
using System.Collections;

public class LevelLoader {
	
	private GameObject statements;

	public void load(string mapText, GameObject world){	
		this.statements = GameObjectFactory.createGameObject ("Statements", world);

		string[] lines = mapText.Split (new char[]{'\n'});
		float y = lines.Length;
		foreach (var line in lines) {
			int indexOfNextSpace;
			int x = 0;
			while( (indexOfNextSpace = line.IndexOf(' ',x)) != -1){
				int lenght = indexOfNextSpace - x;
				if(lenght > 0){
					string nextStatement = line.Substring(x,lenght);
					createStatement(nextStatement,x,y);
					x += lenght;
				}else{
					x += 1;
				}

			}

			/*

			//string nextStatement = "";
			float startingX = 0 ;
			for (int x = 0; x < line.Length; x++) {
				char nextChar = line[x];

				if(nextChar == ' '){
					if(nextStatement.Length > 0){
						createStatement(nextStatement,startingX,y);
					}else{
						//Nothing
					}
				}

			}
			char nextChar;

			while( (nextChar = line[x]) != ' '){
				nextStatement += nextChar;
				x++;
			}

			string[] statementsInText = line.Split(new char[]{' '});
			foreach (var statement in statementsInText) {
				createStatement(statement,x,y);
			}*/

			y--;
		}

	}

	private void createStatement(string line, float x, float y){
		GameObject obj = GameObjectFactory.createGameObject (line, statements);
		Statement statement = obj.AddComponent<Statement> ();
		statement.setText(line);
		obj.transform.Translate (x, y, 0);
	}


}
