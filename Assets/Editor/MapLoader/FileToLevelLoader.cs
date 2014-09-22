using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ReadingMod{ PARAM, LEVEL}

public class FileToLevelLoader {

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
					case ReadingMod.LEVEL : readLevelLine(line + " ");
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
				if(nextStatement.Length != 0){
					createInstruction(nextStatement,x,levelY);
					x += lenght;
				}

			}else{
				x += 1;
			}
			
		}
		levelY--;
	}


	private void createInstruction(string line, float x, float y){
		line = line.Replace('_',' ');

		Instruction instruction = createInstructionObject(line,x,y);

		int indexOfArgument = line.IndexOf ('%');
		int indexOfWhereImAt = 0;
		while (indexOfArgument != -1) {
			string argumentKey 	= line.Substring(indexOfArgument,3);
			if(parameters.ContainsKey(argumentKey)){
				string param 		= parameters[argumentKey];
			}else{
				Debug.LogError("Unknown parameter key\"" + argumentKey + "\"");
			}
			indexOfWhereImAt+= indexOfArgument + 1;
			indexOfArgument = line.IndexOf ('%',indexOfArgument+1);
		}
	}

	private Instruction createInstructionObject(string line, float x, float y){
		int indexOfArgument = line.IndexOf ('%');
		while (indexOfArgument != -1) {
			string argumentKey 	= line.Substring(indexOfArgument,3);
			line.Replace(argumentKey,"%v");
			indexOfArgument = line.IndexOf ('%',indexOfArgument+1);
		}

		GameObject go = GameObjectFactory.createGameObject (line,this.statements);
		Instruction instruction = go.AddComponent<Instruction> ();
		instruction.setText (line);

		TextCollider2D tc = go.GetComponent<TextCollider2D> ();
		tc.font = GameConstantes.instance.statementFont;

		return instruction;
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

		statement.setText(line);
		return obj;
	}


}
