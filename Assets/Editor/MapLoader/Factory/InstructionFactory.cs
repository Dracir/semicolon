using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class InstructionFactory  {

	public static void createInstruction(string line, float x, float y, GameObject parent, Dictionary<string,string> parameters){
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
		instruction.reset ();
	}

	private static Instruction createInstructionObject(GameObject parent, string line, float x, float y){
		int indexOfArgument = line.IndexOf ('$');
		while (indexOfArgument != -1) {
			string argumentKey 	= line.Substring(indexOfArgument,3);
			line = line.Replace(argumentKey,"$v");
			indexOfArgument = line.IndexOf ('$',indexOfArgument+1);
		}

		GameObject go = GameObjectFactory.createGameObject (line,parent);
		go.layer =  LayerMask.NameToLayer("Ignore Raycast");
		go.transform.Translate (x, y, 0);

		Instruction instruction = go.AddComponent<Instruction> ();
		instruction.setText (line);

		TextCollider2D tc = go.GetComponent<TextCollider2D> ();
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
			instruction.setParameterTo(childIndex, DataType.INTEGER);
			IntegerParameter integer = instruction.GetChild(childIndex).GetComponent<IntegerParameter>();
			integer.Valeur = int.Parse(value);
			if(param.Length >= 3){
				if(param[2].StartsWith("setPlayerJumpHeight")){
					GravityChanger gc = parent.AddComponent<GravityChanger>();
					gc.gravityValue = integer;
					integer.observers.Add(gc);
				}
			}
		}else{
			Debug.LogError("MAPLOADER - ERROR : Unknown parameter type for " + paramData);
		}
	}
}
