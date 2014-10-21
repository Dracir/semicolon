using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class InstructionFactory  {

	public static Instruction createInstruction(string line, float x, float y, GameObject parent, List<string> parameterList){
		Dictionary<string,ParameterReader> parameters = new Dictionary<string, ParameterReader>();
		int index = 0;
		foreach (var paramItem in parameterList) {
			int indexOfFirstSpace 	= paramItem.IndexOf (' ');
			string key 				= paramItem.Substring (0, indexOfFirstSpace);
			string paramValue 		= paramItem.Substring (indexOfFirstSpace + 1);
			parameters.Add (key, new ParameterReader(key, paramValue, index));
			index++;
		}
		return InstructionFactory.createInstruction(line,x,y,parent,parameters);
	}
	
	public static Instruction createInstruction(string line, float x, float y, GameObject parent, Dictionary<string,ParameterReader> parameters){
		line = line.Replace('_',' ');

		Instruction instruction = createInstructionObject(parent, line,x,y);

		int indexOfArgument = line.IndexOf ('$');
		int indexOfWhereImAt = 0;
		int indexOfChild = 0;
		while (indexOfArgument != -1) {
			string argumentKey 	= line.Substring(indexOfArgument,3);
			if(parameters.ContainsKey(argumentKey)){
				ParameterReader paramReader = parameters[argumentKey];
				setParameterData(instruction, indexOfChild++, paramReader, parent);
				paramReader.reset();
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
	
	private static void setParameterData(Instruction instruction, int childIndex, ParameterReader paramData, GameObject parent){
		string type = paramData.readWord().ToLower();
		
		if(type.Equals("boolean")){
			instruction.setParameterTo(childIndex, DataType.BOOLEAN);
			BooleanParameter boolean = instruction.GetChild(childIndex).GetComponent<BooleanParameter>();
			boolean.Valeur = paramData.readBoolean();
		}else if(type.Equals("integer")){
			instruction.setParameterTo(childIndex, DataType.INTEGER);
			IntegerParameter integer = instruction.GetChild(childIndex).GetComponent<IntegerParameter>();
			integer.Valeur = paramData.readInt();
			if(paramData.hasNextWord()){
				addIntegerMethod(integer, paramData.readWord(), instruction.gameObject);
			}
			if(paramData.hasNextWord()){
				integer.autoCompile = paramData.readBoolean();
			}
		}else{
			Debug.LogError("MAPLOADER - ERROR : Unknown parameter type for " + type);
		}
	}

	static void addIntegerMethod(IntegerParameter integer, string method, GameObject parent){
		if(method.StartsWith("setPlayerJumpHeight")){
			GravityChanger gc = parent.AddComponent<GravityChanger>();
			gc.gravityValue = integer;
			integer.observers.Add(gc);
			
		}else if(method.StartsWith("addScore")){
			ScoreAdder sa = integer.gameObject.AddComponent<ScoreAdder>();
			sa.integerParameter = integer;
			integer.observers.Add(sa);
			
		}else if(method.StartsWith("removeScore")){
			ScoreRemover sr = integer.gameObject.AddComponent<ScoreRemover>();
			sr.integerParameter = integer;
			integer.observers.Add(sr);
			
		}else if(method.StartsWith("addTime")){
			TimeAdder ta = integer.gameObject.AddComponent<TimeAdder>();
			ta.integerParameter = integer;
			integer.observers.Add(ta);
			
		}else if(method.StartsWith("removeTime")){
			TimeRemover tr = integer.gameObject.AddComponent<TimeRemover>();
			tr.integerParameter = integer;
			integer.observers.Add(tr);
			
		} else if(method.StartsWith("showScore")){
			integer.gameObject.AddComponent<ScoreShower>();
			
		} else if(method.StartsWith("showTime")){
			integer.gameObject.AddComponent<TimeShower>();
			
		} else {
			Debug.LogError("MAPLOADER - ERROR : Unknown Function type for " + method);
		}
	}

	static void addCompileSpotMethod(Instruction instruction, Dictionary<string, ParameterReader> parameters){
		string key = instruction.instructionText.Substring(instruction.instructionText.IndexOf("¶"));
		if(parameters.ContainsKey(key)){
			ParameterReader reader = parameters[key];
			addCompileSpotMethod(instruction, reader);
			reader.reset();
		} else{
			Debug.LogError("MAPLOADER - ERROR : Unknown Key " + key);
		}
	}
	
	static void addCompileSpotMethod(Instruction instruction, ParameterReader parameterReader){
		string method = parameterReader.readWord();
		string methodLowered = method.ToLower();
		
		if(methodLowered.Equals("debuglog")){
			DebugLog dl = instruction.AddComponent<DebugLog>();
			dl.textToLog = parameterReader.readString();
			instruction.observers.Add(dl);
			
		}else if(methodLowered.Equals("compile")){
			
		}else if(methodLowered.Equals("dropspikes")){
			DropSpikes ds = instruction.AddComponent<DropSpikes>();
			ds.nbSpikesToDrop = parameterReader.readInt();
			ds.timeBetweenCallMin = parameterReader.readFloat();
			ds.timeBetweenCallMax = parameterReader.readFloat();
			ds.spawningOrderAlgoName = parameterReader.readWord();
			instruction.observers.Add(ds);
			
		}else{
			Debug.LogError("MAPLOADER - ERROR : Unknown Function type for compile spot " + method);
		}
	}
	
}
