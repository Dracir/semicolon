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
			y--;
		}

	}

	private void createStatement(string line, float x, float y){
		line = line.Replace('_',' ');
		GameObject obj;
		int indexOfArgument = line.IndexOf ('%');
		if (indexOfArgument == -1) {
			obj = createCommentStatement (line);
		} else {
			line = line.Substring(0,indexOfArgument) + "%v"; 
			obj = createBooleanStatement (line);		
		}
		
		obj.transform.Translate (x, y, 0);
	}

	private GameObject createCommentStatement(string line){
		GameObject obj = GameObjectFactory.createGameObject (line, statements);
		Statement statement = obj.AddComponent<Statement> ();
		statement.setText(line);
		return obj;
	}

	private GameObject createBooleanStatement(string line){
		GameObject obj = GameObjectFactory.createGameObject (line, statements);
		BooleanStatement statement = obj.AddComponent<BooleanStatement> ();
		statement.BooleanValue = BooleanValues.TRUE;
		statement.setText(line);
		return obj;
	}


}
