using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataTypeDictionnary {

	public static Dictionary<DataTypeEnum, DataType> dataTypes = generateDataTypes();

	private static Dictionary<DataTypeEnum, DataType> generateDataTypes(){
		Dictionary<DataTypeEnum, DataType> dictionnary = new Dictionary<DataTypeEnum, DataType> ();

		addToDictionnary (dictionnary, DataTypeEnum.Boolean	, "Boolean"	, null);
		addToDictionnary (dictionnary, DataTypeEnum.Number	, "Number"	, null);

		return dictionnary;
	}

	private static void addToDictionnary(Dictionary<DataTypeEnum, DataType> dictionnary, DataTypeEnum dataType, string text, DataType parent){
		DataType newDataType = new DataType (dataType, text, parent);
		dictionnary.Add (DataTypeEnum.Boolean, newDataType);
	}
}
