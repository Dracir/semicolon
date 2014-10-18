using UnityEngine;
using System.Collections;

public class BufferedDataReader {

	private string fullLine;
	public string errorMessagePrefix;
	public string remainingLine{get; private set; }
	 
	
	public BufferedDataReader(string line, string errorMessagePrefix){
		this.fullLine = line;
		this.remainingLine = line;
		this.errorMessagePrefix = errorMessagePrefix;
	}

	public void reset(){
		this.remainingLine = fullLine;
	}
	
	public bool readBoolean(){
		string value = this.readWord();
		string valueLowered = value.ToLower();
		if(valueLowered.StartsWith("true")){
			return true;
		}else if(valueLowered.StartsWith("false")){
			return false;
		}else{
			logError("Unknown Boolean value \"" + value + "\" must be TRUE or FALSE.");
			return false;
		}
	}
	
	public string readString(){
		readDataUntil('"');
		string text = readDataUntil('"');
		remainingLine.TrimStart(new char[]{' '});
		return text;
	}
	
	public float readFloat(){
		return float.Parse(readWord());
	}
	
	public int readInt(){
		int result = 0;
		string word = readWord();
		if(!int.TryParse(word, out result)){
			logError("Unknown Integer value \"" + word + "\" must be a number between " + int.MinValue + " and " + int.MaxValue);
		}
		return result;
	}
	
	public string readWord(){
		string next = readDataUntil(' ');
		if(next == null){
			if(remainingLine.Length != 0){
				next = remainingLine;
				remainingLine = "";
			}else{
				return "";
			}

		}
		return next;
	}
	
	public bool hasNextWord(){
		int index = remainingLine.IndexOf(' ');
		return index != -1;
	}

	public string readDataUntil(char stopChar){
		Debug.Log(remainingLine);
		int index = remainingLine.IndexOf(stopChar);
		if(index == -1){
			return  null;
		}else{
			string data = remainingLine.Substring(0,index);
			remainingLine = remainingLine.Substring(index+1);
			return data;
		}
	}
	
	private void logError(string errorMesssage){
		Debug.LogError(this.errorMessagePrefix + errorMesssage);
	}
}
