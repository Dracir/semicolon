using UnityEngine;
using System.Collections.Generic;

public class BagInvoker {

	
	public List<MonoBehaviour> objectsToInvoke;
	
	public BagInvoker(){
		this.objectsToInvoke = new List<MonoBehaviour>();
	}
	
	public void Invoking(string algoName, string methodName,  int nbCalls, float minTime, float maxTime){
		if(algoName.StartsWith("RandomAtLeastOnceInvoking")){
			RandomAtLeastOnceInvoking(methodName,nbCalls,minTime,maxTime);
		}else if(algoName.StartsWith("RoundRobinInvoking")){
			RoundRobinInvoking(methodName,nbCalls,minTime,maxTime);
		}else{
			Debug.LogError("The fuck je connais pas cette algo :" + algoName);
		}
	}
	
	public void RandomAtLeastOnceInvoking(string methodName, int nbCalls, float minTime, float maxTime){
		List<MonoBehaviour> objectsRemainning = new List<MonoBehaviour>();
		int spanned = 0;
		float totalTime = 0;
		while(spanned < nbCalls){
			if(objectsRemainning.Count == 0){
				objectsRemainning.AddRange(objectsToInvoke);
			}
			
			MonoBehaviour mb = objectsRemainning.PopRandom();
			float time = Random.Range(minTime, maxTime);
			totalTime += time;
			mb.SendMessage(methodName, totalTime);
			spanned++;
		}
	}
	
	public void RoundRobinInvoking(string methodName, int nbCalls, float minTime, float maxTime){
		List<MonoBehaviour> objectsRemainning = new List<MonoBehaviour>();
		int spanned = 0;
		float totalTime = 0;
		while(spanned < nbCalls){
			if(objectsRemainning.Count == 0){
				objectsRemainning.AddRange(objectsToInvoke);
			}
			
			MonoBehaviour mb = objectsRemainning.Pop(0);
			float time = Random.Range(minTime, maxTime);
			totalTime += time;
			mb.SendMessage(methodName, totalTime);
			spanned++;
		}
	}
}
