using UnityEngine;
using System.Collections;

public class TimeAdder : Observer {

	public IntegerParameter integerParameter;
	
	public override void notify(){
		if(LevelTime.instance != null){
			LevelTime.instance.TimeLeft += integerParameter.Valeur;
		}else{
			//Debug.LogWarning("Pas d'instance de GameTime ?!!? :O");
		}		
	}
}
