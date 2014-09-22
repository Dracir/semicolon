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
		line = line.Replace("\n","").Replace("\r","");
		while( (indexOfNextSpace = line.IndexOf(' ',x)) != -1){
			int lenght = indexOfNextSpace - x;
			if(lenght > 0){
				string nextStatement = line.Substring(x,lenght);
				if(nextStatement.Length != 0 && !nextStatement.Equals(" ")){
					createInstruction(nextStatement,x,levelY);
					x += lenght;
				}

			}else{
				x += 1;
			}
			
		}
		levelY-=1.66f;
	}


	private void createInstruction(string line, float x, float y){
		line = line.Replace('_',' ');

		Instruction instruction = createInstructionObject(line,x,y);

		int indexOfArgument = line.IndexOf ('$');
		int indexOfWhereImAt = 0;
		int indexOfChild = 0;
		while (indexOfArgument != -1) {
			string argumentKey 	= line.Substring(indexOfArgument,3);
			if(parameters.ContainsKey(argumentKey)){
				string paramData 	= parameters[argumentKey];
				Parameter parameter = instruction.gameObject.GetChild(indexOfChild++).GetComponent<Parameter>();
				setParameterData(parameter, paramData);
			}else{
				Debug.LogError("Unknown parameter key\"" + argumentKey + "\"");
			}
			indexOfWhereImAt+= indexOfArgument + 1;
			indexOfArgument = line.IndexOf ('$',indexOfArgument+1);
		}
		instruction.reset ();
	}

	private Instruction createInstructionObject(string line, float x, float y){
		int indexOfArgument = line.IndexOf ('$');
		while (indexOfArgument != -1) {
			string argumentKey 	= line.Substring(indexOfArgument,3);
			line = line.Replace(argumentKey,"$v");
			indexOfArgument = line.IndexOf ('$',indexOfArgument+1);
		}

		GameObject go = GameObjectFactory.createGameObject (line,this.statements);
		go.layer =  LayerMask.NameToLayer("Ignore Raycast");
		go.transform.Translate (x, y, 0);

		Instruction instruction = go.AddComponent<Instruction> ();
		instruction.setText (line);

		TextCollider2D tc = go.GetComponent<TextCollider2D> ();
		tc.font = GameConstantes.instance.statementFont;

		return instruction;
	}

	private void setParameterData(Parameter parameter, string paramData){
		string[] param = paramData.Split(' ');
		string type = param[0].ToLower();
		string value = param[1].ToLower();
		if(type.Equals("boolean")){
				parameter.dataType = DataType.BOOLEAN;
			if(value.StartsWith("true")){
				parameter.value = new SCBoolean(true);
			}else if(value.StartsWith("false")){
				parameter.value = new SCBoolean(false);
			}else{
				Debug.LogError("MAPLOADER - ERROR : Unknown parameter value for " + paramData);
			}
		}else if(type.Equals("integer")){
			parameter.dataType = DataType.INTEGER;
			parameter.value = new SCInteger(int.Parse(value));
		}else{
			Debug.LogError("MAPLOADER - ERROR : Unknown parameter type for " + paramData);
		}
		parameter.refresh();
	}

}
