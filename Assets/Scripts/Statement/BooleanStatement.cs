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
			notifyObservers();
		}
	}


	protected override void createTextChild(string text){
		int indexOfValueTag = statementText.IndexOf("%v");
		
		string textBefore = statementText.Substring(0,indexOfValueTag);
		string textAfter  = booleanValue.ToString();
		
		GameObject beforeArgumentChild = createText (Vector2.zero,textBefore);
		
		Vector2 translate = new Vector2(indexOfValueTag, 0);
		beforeArgumentChild = createText (translate, textAfter, GameConstantes.instance.booleanValueColor);

		/*int indexOfValueTag = statementText.IndexOf("%v");
		
		string textBefore = statementText.Substring(0,indexOfValueTag);
		string textAfter  = statementText.Substring(indexOfValueTag+2, statementText.Length - indexOfValueTag -2);
		
		GameObject beforeArgumentChild = createText (Vector2.zero,textBefore);
		
		Vector2 translate = new Vector2(indexOfValueTag, 0);
		beforeArgumentChild = createText (translate, "");
		
		Vector2 translate2 = new Vector2(indexOfValueTag, 0);
		beforeArgumentChild = createText (translate2, textAfter);*/
	}

}
