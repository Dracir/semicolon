using UnityEngine;
using System.Collections.Generic;

public class AudioGainManager : MonoBehaviour {

	public float volume = 0;
	public string sendName = "";
	
	bool initialized;
	bool sendToPD;
	float pVolume = 0;
	
	static Dictionary<string, float[]> dataToSend = new Dictionary<string, float[]>();
	static Dictionary<string, int> sendAmount = new Dictionary<string, int>();
	static Dictionary<string, int> sendCount = new Dictionary<string, int>();

	public void Initialize(string sendName){
		this.sendName = sendName;
		
		sendToPD = !string.IsNullOrEmpty(sendName);
		if (sendToPD) {
			int bufferSize;
			int bufferAmount;
			AudioSettings.GetDSPBufferSize(out bufferSize, out bufferAmount);
			
			if (!dataToSend.ContainsKey(sendName))
				dataToSend[sendName] = new float[bufferSize * 2];
			if (!sendAmount.ContainsKey(sendName))
				sendAmount[sendName] = 0;
			if (!sendCount.ContainsKey(sendName))
				sendCount[sendName] = 0;
			sendAmount[sendName] += 1;
		}
		initialized = true;
	}
	
	void OnDisable(){
		if (sendToPD) {
			sendAmount[sendName] -= 1;
			if (sendAmount[sendName] == 0){
				dataToSend.Remove(sendName);
				sendAmount.Remove(sendName);
				sendCount.Remove(sendName);
			}
		}
		sendToPD = false;
		initialized = false;
	}
	
	void OnAudioFilterRead (float[] data, int channels){
		for (int i = 0; i < data.Length; i++) {
			pVolume = Mathf.Lerp(pVolume, volume, 0.001F);
			data[i] *= pVolume;
			if (sendToPD && initialized) {
				dataToSend[sendName][i] += data[i];
				data[i] = 0;
			}
		}
		
		if (sendToPD && initialized) {
			sendCount[sendName] += 1;
			if (sendCount[sendName] >= sendAmount[sendName]) {
				PDPlayerOld.SendValue(sendName, dataToSend[sendName]);
				dataToSend[sendName] = new float[data.Length];
				sendCount[sendName] = 0;
			}
		}
		
	}
}
