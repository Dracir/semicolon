using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropSpikes : Observer {

	public string spawningOrderAlgoName = "RandomAtLeastOnceInvoking";
	public IntegerParameter nbSpikesToDropParameter;
	public int nbSpikesToDrop;
	public float timeBetweenCallMin;
	public float timeBetweenCallMax;
	
	public List<SpikeManager> linkedSpikes;
	
	void Start () {
		linkedSpikes = LevelReferences.instance.spikeManagers;
	}
	
	
	public override void notify(){
		if(linkedSpikes == null) return;
		
		BagInvoker bagInvoker = new BagInvoker();
		foreach (var spikes in linkedSpikes) {
			bagInvoker.objectsToInvoke.Add(spikes.GetComponent<MonoBehaviour>());
		}
		int nbToDrop = (nbSpikesToDropParameter == null) ? nbSpikesToDrop : nbSpikesToDropParameter.Valeur;
		bagInvoker.Invoking(spawningOrderAlgoName, "SpawnSpikeIn", nbToDrop,timeBetweenCallMin, timeBetweenCallMax);
	}
}
