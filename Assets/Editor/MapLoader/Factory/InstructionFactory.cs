using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class InstructionFactory  {

	public static LevelScore levelScore;
	public static LevelTime levelTime;
	
	public static Instruction createInstruction(string line, float x, float y, GameObject parent, List<string> parameterList){
		Dictionary<string,string> parameters = new Dictionary<string, string>();
		foreach (var paramItem in parameterList) {
			int indexOfFirstSpace 	= paramItem.IndexOf (' ');
			string key 				= paramItem.Substring (0, indexOfFirstSpace);
			string paramValue 		= paramItem.Substring (indexOfFirstSpace + 1);
			parameters.Add (key, paramValue);
		}
		return InstructionFactory.createInstruction(line,x,y,parent,parameters);
	}
	
	public static Instruction createInstruction(string line, float x, float y, GameObject parent, Dictionary<string,string> parameters){
		line = line.Replace('_',' ');

		Instruction instruction = createInstructionObject(parent, line,x,y);

		int indexOfArgument = line.IndexOf ('$');
		int indexOfWhereImAt = 0;
		int indexOfChild = 0;
		while (indexOfArgument != -1) {
			string argumentKey 	= line.Substring(indexOfArgument,3);
			if(parameters.ContainsKey(argumentKey)){
				string paramData 	= parameters[argumentKey];
				setParameterData(instruction, indexOfChild++, paramData, parent);
			}else{
				Debug.LogError("Unknown parameter key\"" + argumentKey + "\"");
			}
			indexOfWhereImAt+= indexOfArgument + 1;
			indexOfArgument = line.IndexOf ('$',indexOfArgument+1);
		}
		if(instruction.hasCompileSpot){
			addCompileSpotMethod(instruction,parameters);
		}
		instruction.reset ();
		
		return instruction;
	}


	private static Instruction createInstructionObject(GameObject parent, string line, float x, float y){
		int indexOfArgument = line.IndexOf ('$');
		while (indexOfArgument != -1) {
			string argumentKey 	= line.Substring(indexOfArgument,3);
			line = line.Replace(argumentKey,"$v");
			indexOfArgument = line.IndexOf ('$',indexOfArgument+1);
		}

		GameObject go = GameObjectFactory.createGameObject (line,parent);
		go.layer =  LayerMask.NameToLayer("Default");
		go.transform.Translate (x, y, 0);

		TextCollider2D tc = go.AddComponent<TextCollider2D> ();
		Instruction instruction = go.AddComponent<Instruction> ();
		instruction.setText (line);

		
		tc.Font = GameConstantes.instance.currentTheme.instructionFont;

		return instruction;
	}
	
	private static void setParameterData(Instruction instruction, int childIndex, string paramData, GameObject parent){
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
			IntegerParameter integer = addIntegerParam(instruction, childIndex, value);
			if(param.Length >= 3){
				addIntegerMethod(integer,param[2], instruction.gameObject);
			}
			if(param.Length >= 4){
				integer.autoCompile = param[3].StartsWith("true");
			}
		}else{
			Debug.LogError("MAPLOADER - ERROR : Unknown parameter type for " + paramData);
		}
	}

	static IntegerParameter addIntegerParam(Instruction instruction, int childIndex, string value){
		instruction.setParameterTo(childIndex, DataType.INTEGER);
		IntegerParameter integer = instruction.GetChild(childIndex).GetComponent<IntegerParameter>();
		integer.Valeur = int.Parse(value);
		return integer;
	}

	static void addIntegerMethod(IntegerParameter integer, string methode, GameObject parent){
		if(methode.StartsWith("setPlayerJumpHeight")){
			GravityChanger gc = parent.AddComponent<GravityChanger>();
			gc.gravityValue = integer;
			integer.observers.Add(gc);
			
		}else if(methode.StartsWith("addScore")){
			ScoreAdder sa = integer.gameObject.AddComponent<ScoreAdder>();
			sa.integerParameter = integer;
			integer.observers.Add(sa);
			
		}else if(methode.StartsWith("removeScore")){
			ScoreRemover sr = integer.gameObject.AddComponent<ScoreRemover>();
			sr.integerParameter = integer;
			integer.observers.Add(sr);
			
		}else if(methode.StartsWith("addTime")){
			TimeAdder ta = integer.gameObject.AddComponent<TimeAdder>();
			ta.integerParameter = integer;
			integer.observers.Add(ta);
			
		}else if(methode.StartsWith("removeTime")){
			TimeRemover tr = integer.gameObject.AddComponent<TimeRemover>();
			tr.integerParameter = integer;
			integer.observers.Add(tr);
		} else{
			Debug.LogError("MAPLOADER - ERROR : Unknown Function type for " + methode);
		}
	}

	static void addCompileSpotMethod(Instruction instruction, Dictionary<string, string> parameters){
		string key = instruction.instructionText.Substring(instruction.instructionText.IndexOf("¶"));
		if(parameters.ContainsKey(key)){
			string[] splited = parameters[key].Split(new char[]{' '});
			string method = splited[0];
			addCompileSpotMethod(instruction,method,parameters[key]);
		} else{
			Debug.LogError("MAPLOADER - ERROR : Unknown Key " + key);
		}
	}
	
	static void addCompileSpotMethod(Instruction instruction, string method, string arguments){
		if(method.Equals("debuglog")){
			string debugText = arguments.Substring(arguments.IndexOf(" "));
			debugText = debugText.Replace("\"", " ").Trim();
			DebugLog dl = instruction.AddComponent<DebugLog>();
			dl.textToLog = debugText;
			instruction.observers.Add(dl);
		}
	}
}
