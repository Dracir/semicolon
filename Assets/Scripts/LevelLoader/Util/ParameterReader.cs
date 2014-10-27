using UnityEngine;
using System.Collections;

public class ParameterReader {

	private BufferedDataReader reader;
	private int parameterIndex;
	private int fileLine;
	private string parameterKey;
	
	public ParameterReader(string parameterKey, string parameterlineString, int fileLine){
		this.reader = new BufferedDataReader(parameterlineString, "");
		this.fileLine = fileLine;
		this.parameterKey = parameterKey;
	}
	
	public void reset(){
		reader.reset();
		parameterIndex = 0;
	}
	
	public string getRemainingLine(){
		return reader.remainingLine;
	}
	
	private void resetErrorMessage(){
		this.reader.errorMessagePrefix = "Error (key:" + parameterKey + ") line:" + fileLine + ", param Index:" + parameterIndex + " : ";
		parameterIndex++;
	}
	
	public bool readBoolean(){
		resetErrorMessage();
		return reader.readBoolean();
	}
	
	public string readString(){
		resetErrorMessage();
		return reader.readString();
	}

	public int readIndexPosition(){
		resetErrorMessage();
		return reader.readIndexPosition();
	}
	public float readFloat(){
		resetErrorMessage();
		return reader.readFloat();
	}
	
	public int readInt(){
		resetErrorMessage();
		return reader.readInt();
	}
	
	public string readWord(){
		resetErrorMessage();
		return reader.readWord();
	}
	
	public bool hasNextWord(){
		resetErrorMessage();
		return reader.hasNextWord();
	}

	public bool nextWordContains(string str){
		resetErrorMessage();
		return reader.nextWordContains(str);
	}
	public string readDataUntil(char stopChar){
		resetErrorMessage();
		return reader.readDataUntil(stopChar);
	}
}
