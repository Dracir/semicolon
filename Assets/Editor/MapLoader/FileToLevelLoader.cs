using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ReadingMod{ PARAM, LEVEL, SPECIAL_CHARACTER}

public class FileToLevelLoader {

	private ReadingMod readingMod;
	private GameObject world;
	private GameObject statements;

	private float levelY;

	private Dictionary<string,string> parameters;
	
	private Dictionary<string,string> specialCharacter;
	private Dictionary<string,GameObject> specialCharacterGameObject;

	public void load(string mapText, GameObject world){	
		this.parameters 				= new Dictionary<string, string>();
		this.statements 				= GameObjectFactory.createGameObject ("Statements", world);
		this.specialCharacter 			= new Dictionary<string, string>();
		this.specialCharacterGameObject = new Dictionary<string, GameObject>();
		this.world 						= world;

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
					case ReadingMod.SPECIAL_CHARACTER: readSpecialCharacter(line);
						break;
				}
			}

		}

	}

	
	void readSpecialCharacter(string line){
		int indexOfFirstSpace = line.IndexOf (' ');
		string key = line.Substring (0, indexOfFirstSpace);
		string param = line.Substring (indexOfFirstSpace + 1);
		this.specialCharacter.Add (key, param);
	}
	
	
	private void changeReadingMod(string line){
		if (line.Contains("<Params>")) {
			readingMod = ReadingMod.PARAM;
		} else if (line.Contains("<Level>")) {
			readingMod = ReadingMod.LEVEL;
		} else if (line.Contains("<SpecialCharacter>")) {
			readingMod = ReadingMod.SPECIAL_CHARACTER;
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
				setParameterData(instruction, indexOfChild++, paramData);
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
		GameObject parent = this.statements;
		if(this.specialCharacter.ContainsKey(line)){
			string[] sp = this.specialCharacter[line].TrimEnd(new char[]{'\n','\r'}).Split(' ');
			parent = addOrGetParent(sp[0]);
			
		}

		GameObject go = GameObjectFactory.createGameObject (line,parent);
		go.layer =  LayerMask.NameToLayer("Ignore Raycast");
		go.transform.Translate (x, y, 0);

		Instruction instruction = go.AddComponent<Instruction> ();
		instruction.setText (line);

		TextCollider2D tc = go.GetComponent<TextCollider2D> ();
		tc.Font = GameConstantes.instance.statementFont;

		return instruction;
	}

	GameObject addOrGetParent(string parentName){
		if(this.specialCharacterGameObject.ContainsKey(parentName)){
			return this.specialCharacterGameObject[parentName];
		}else{
			GameObject parent = GameObjectFactory.createGameObject(parentName, this.world);
			this.specialCharacterGameObject.Add(parentName,parent);
			return parent;
		}
	}
	private void setParameterData(Instruction instruction, int childIndex, string paramData){
		string[] param = paramData.TrimEnd(new char[]{'\n','\r'}).Split(' ');
		string type = param[0].ToLower();
		string value = param[1].ToLower();
		
		if(type.Equals("boolean")){
			instruction.setParameterTo(childIndex, DataType.BOOLEAN);
			BooleanParameter boolean = instruction.GetChild(childIndex).GetComponent<BooleanParameter>();
			if(value.StartsWith("true")){
				boolean.Valeur = true;
			}else if(value.StartsWith("false")){
				boolean.Valeur = false;
			}else{
				Debug.LogError("MAPLOADER - ERROR : Unknown parameter value for " + paramData);
			}
		}else if(type.Equals("integer")){
			instruction.setParameterTo(childIndex, DataType.INTEGER);
			IntegerParameter integer = instruction.GetChild(childIndex).GetComponent<IntegerParameter>();
			integer.Valeur = int.Parse(value);
			if(param.Length >= 3){
				if(param[2].StartsWith("setPlayerJumpHeight")){
					GravityChanger gc = world.AddComponent<GravityChanger>();
					gc.gravityValue = integer;
					integer.observers.Add(gc);
				}
			}
		}else{
			Debug.LogError("MAPLOADER - ERROR : Unknown parameter type for " + paramData);
		}
	}

}
