using UnityEngine;
using System.Collections.Generic;

public class DataType {

	public DataTypeEnum dataType;
	public string text;
	public List<DataType> childs;
	public DataType parent;

	public DataType(DataTypeEnum dataType, string text, DataType parent){
		this.dataType = dataType;
		this.text = text;
		this.parent = parent;
		if (parent != null) {
			this.parent.childs.Add(this);		
		}
	}

}
