using UnityEngine;
using System.Collections;

public class ChangeColorObject : Observer {

	public BooleanStatement booleanStatement;

	public override void update(){
		TextMesh tm = gameObject.GetComponentInChildren<TextMesh>();

		if (booleanStatement.BooleanValue.Equals (BooleanValues.TRUE)) {
			tm.color = GameConstantes.instance.booleanValueColor;
		} else {
			tm.color = GameConstantes.instance.statementColor;	
		}
	}

}
