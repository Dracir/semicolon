using UnityEngine;
using System.Collections;

[System.Serializable]
public class BooleanStatement : Statement{

	public BooleanValues booleanValue;
	
	public BooleanValues BooleanValue{
		get{
			return booleanValue;
		}
		set{
			booleanValue = value;
			resetText();
		}
	}


	protected override void createTextChild(string text){
		int indexOfValueTag = statementText.IndexOf("%v");
		
		string textBefore = statementText.Substring(0,indexOfValueTag);
		string textAfter  = "";
		if (!booleanValue.Equals (BooleanValues.NULL)) {
			textAfter  = booleanValue.ToString();
		}

		
		GameObject beforeArgumentChild = createText (Vector2.zero,textBefore);
		
		Vector2 translate = new Vector2(indexOfValueTag, 0);
		beforeArgumentChild = createMoveableText (translate, textAfter, GameConstantes.instance.booleanValueColor);

		/*int indexOfValueTag = statementText.IndexOf("%v");
		
		string textBefore = statementText.Substring(0,indexOfValueTag);
		string textAfter  = statementText.Substring(indexOfValueTag+2, statementText.Length - indexOfValueTag -2);
		
		GameObject beforeArgumentChild = createText (Vector2.zero,textBefore);
		
		Vector2 translate = new Vector2(indexOfValueTag, 0);
		beforeArgumentChild = createText (translate, "");
		
		Vector2 translate2 = new Vector2(indexOfValueTag, 0);
		beforeArgumentChild = createText (translate2, textAfter);*/
	}

	public override bool isSameType(Statement other){
		return other is BooleanStatement;
	}

	public override void swapParam(Statement statement){ 
		BooleanStatement other = (BooleanStatement) statement;
		BooleanValues tmp = other.booleanValue;
		other.BooleanValue = this.booleanValue;
		this.BooleanValue = tmp;

	}

	void OnDrawGizmos (){
		foreach (var observer in observers) {
			Gizmos.color = GameConstantes.instance.booleanValueColor;
			Gizmos.DrawLine (this.transform.GetChild(1).transform.position, observer.transform.position);
		}
	}

}
