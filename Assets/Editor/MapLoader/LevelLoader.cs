using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ReadingMod{ PARAM, LEVEL}

public class LevelLoader {

	private ReadingMod readingMod;
	private GameObject statements;

	private float levelY;

	private Dictionary<string,string> parameters;

	public void load(string mapText, GameObject world){	
		this.parameters = new Dictionary<string, string>();
		this.statements = GameObjectFactory.createGameObject ("Statements", world);

		string[] lines = mapText.Split (new char[]{'\n'});
		foreach (var line in lines) {
			if(line.StartsWith("<")){
				changeReadingMod(line);
			}else{
				switch(readingMod){
					case ReadingMod.PARAM: readParam(line); 
						break;
					case ReadingMod.LEVEL : readLevelLine(line);
						break;
				}
			}

		}

	}

	private void changeReadingMod(string line){
		if (line.Contains("<Params>")) {
			readingMod = ReadingMod.PARAM;
		} else if (line.Contains("<Level>")) {
			readingMod = ReadingMod.LEVEL;
		}
	}

	private void readParam(string line){
		int indexOfFirstSpace = line.IndexOf (' ');
		string key = line.Substring (0, indexOfFirstSpace);
		string param = line.Substring (indexOfFirstSpace + 1);
		this.parameters.Add (key, param);
	}

	private void readLevelLine(string line){
		int x = 0;
		int indexOfNextSpace;
		while( (indexOfNextSpace = line.IndexOf(' ',x)) != -1){
			int lenght = indexOfNextSpace - x;
			if(lenght > 0){
				string nextStatement = line.Substring(x,lenght);
				createStatement(nextStatement,x,levelY);
				x += lenght;
			}else{
				x += 1;
			}
			
		}
		levelY--;
	}
	
	private void createStatement(string line, float x, float y){
		line = line.Replace('_',' ');
		GameObject obj = null;
		int indexOfArgument = line.IndexOf ('%');
		if (indexOfArgument == -1) {
			obj = createCommentStatement (line);
		} else {
			string key = line.Substring(indexOfArgument).TrimEnd(';');
			string[] param = parameters[key].Split(' ');
			string type = param[0].ToLower();
			if(type.Equals("boolean")){
				line = line.Substring(0,indexOfArgument) + "%v"; 
				obj = createBooleanStatement (line,param);
			}else{
				Debug.LogError("MAPLOADER - ERROR : Unknown parameter type");
			}

		}
		
		obj.transform.Translate (x, y, 0);
	}



	private GameObject createCommentStatement(string line){
		GameObject obj = GameObjectFactory.createGameObject (line, statements);
		Statement statement = obj.AddComponent<Statement> ();
		statement.setText(line);
		return obj;
	}

	private GameObject createBooleanStatement(string line,string[] param ){
		GameObject obj = GameObjectFactory.createGameObject (line, statements);
		BooleanStatement statement = obj.AddComponent<BooleanStatement> ();
		if (param [1].ToLower ().Trim ().Equals ("true")) {
			statement.booleanValue = BooleanValues.TRUE;
		} else {
			statement.booleanValue = BooleanValues.FALSE;
		}
		
		Debug.Log ("create " + line);
		statement.setText(line);
		return obj;
	}


}
