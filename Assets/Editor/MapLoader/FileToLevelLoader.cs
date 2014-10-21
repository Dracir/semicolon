using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ReadingMod{ PARAM, LEVEL, SPECIAL_CHARACTER, CONFIG}

public class FileToLevelLoader {

	private ReadingMod readingMod;
	private GameObject world;
	private GameObject instructions;
	private GameObject levelCharacters;

	private float levelY;
	private int fileLine;

	
	private Dictionary<string,ParameterReader> parameterReaders;
	
	private Dictionary<string,string> specialCharacter;
	private Dictionary<string,GameObject> specialCharacterGameObject;

	public void load(string mapText, GameObject world){	
		this.parameterReaders 			= new Dictionary<string, ParameterReader>();
		this.instructions 				= GameObjectFactory.createGameObject ("Instruction", world);
		this.levelCharacters			= GameObjectFactory.createGameObject ("Level Character", world);
		this.specialCharacter 			= new Dictionary<string, string>();
		this.specialCharacterGameObject = new Dictionary<string, GameObject>();
		this.world 						= world;
		this.fileLine					= 0;

		string[] lines = mapText.Split (new char[]{'\n'});
		foreach (var line in lines) {
			fileLine++;
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
					case ReadingMod.CONFIG : readConfig(line);
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
		} else if (line.Contains("<Config>")) {
			readingMod = ReadingMod.CONFIG;
		} else {
			Debug.LogError("FileToLevelLoader - ERROR : Unknown readingMod " + line);
		}
	}

	private void readParam(string line){
		BufferedDataReader parser 		= new BufferedDataReader(line, "");
		string 			key			= parser.readWord();
		ParameterReader lineParam 	= new ParameterReader(key, parser.remainingLine, this.fileLine);
		this.parameterReaders.Add(key, lineParam);
	}
	
	private void readConfig(string line){
		BufferedDataReader parser = new BufferedDataReader(line, "");
		string configName = parser.readWord();
		if(configName.Equals("Time")){
			LevelTime lt = world.AddComponent<LevelTime>();
			lt.TimeLeft = parser.readInt();
			
		} else if(configName.StartsWith("Score")){
			world.AddComponent<LevelScore>();
		
		} else {
			Debug.LogError("FileToLevelLoader - ERROR : Unknown config " + configName);
		}
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
					create(nextStatement, x,levelY);
					x += lenght;
				}
			}else{
				x += 1;
			}
		}
		levelY-=1.66f;
	}

	void create(string line, int x, float y){
		if(this.specialCharacter.ContainsKey(line)){
			string[] sp = this.specialCharacter[line].TrimEnd(new char[]{'\n','\r'}).Split(' ');
			GameObject parent = addOrGetParent(sp[0]);
			SpecialCharacterFactory.createSpecialCharacter(parent, line, this.specialCharacter[line], x, y);
			
		}else{
			GameObject parent = this.levelCharacters;
			if(line.Contains("$") || line.Contains("¶")){
				parent = this.instructions;
			}
			InstructionFactory.createInstruction(line, x, y, parent, this.parameterReaders);
		}
	}
	

	private GameObject addOrGetParent(string parentName){
		if(this.specialCharacterGameObject.ContainsKey(parentName)){
			return this.specialCharacterGameObject[parentName];
		}else{
			GameObject parent = GameObjectFactory.createGameObject(parentName, this.world);
			this.specialCharacterGameObject.Add(parentName,parent);
			return parent;
		}
	}
}