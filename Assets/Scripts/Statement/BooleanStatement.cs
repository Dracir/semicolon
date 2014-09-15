using UnityEngine;
using System.Collections;

public class BooleanStatement : Statement {

	public BooleanValues booleanValue;

	//public delegate void MyDelegate(int num);
	//public MyDelegate myDelegate;

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
