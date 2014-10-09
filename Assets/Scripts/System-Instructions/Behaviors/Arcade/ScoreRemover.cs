using UnityEngine;
using System.Collections;

public class ScoreRemover : Observer {

	public IntegerParameter integerParameter;
	
	public override void notify(){
		if(LevelScore.instance != null){
			LevelScore.instance.Score -= integerParameter.Valeur;
		}else{
			Debug.LogWarning("Pas d'instance de GameScore ?!!? :O");
		}
	}
	
}
