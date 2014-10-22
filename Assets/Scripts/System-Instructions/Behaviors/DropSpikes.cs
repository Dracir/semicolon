using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropSpikes : Observer {

	public string spawningOrderAlgoName = "RandomAtLeastOnceInvoking";
	public int nbSpikesToDrop;
	public float timeBetweenCallMin;
	public float timeBetweenCallMax;
	
	public List<SpikeManager> linkedSpikes;
	
	void Start () {
		linkedSpikes = References.SpikeManagers;
	}
	
	
	public override void notify(){
		if(linkedSpikes == null) return;
		
		BagInvoker bagInvoker = new BagInvoker();
		foreach (var spikes in linkedSpikes) {
			bagInvoker.objectsToInvoke.Add(spikes.GetComponent<MonoBehaviour>());
		}
		bagInvoker.Invoking(spawningOrderAlgoName, "SpawnSpikeIn",nbSpikesToDrop,timeBetweenCallMin, timeBetweenCallMax);
	}
}
