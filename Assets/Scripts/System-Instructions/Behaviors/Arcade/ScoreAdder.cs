using UnityEngine;
using System.Collections;

public class ScoreAdder : Observer {

	
	public IntegerParameter integerParameter;
	
	public override void notify(){
		if(LevelScore.instance != null){
			LevelScore.instance.score += integerParameter.Valeur;
		}else{
			Debug.LogWarning("Pas d'instance de GameScore ?!!? :O");
		}
		
	}
	
}
