using UnityEngine;
using System.Collections;

public class GridMaker : MonoBehaviour {
	
	public bool makeLines = true;
	
	public int horizontalLines = 24;
	public int verticalLines = 24;
	public int hSpacing = 8;
	public int vSpacing = 8;
	public int hLength = 256;
	public int vLength = 128;
	public int originX;
	public int originY;
	public Vector2 startV = Vector2.zero;
	public float alpha = 0.7f;
	public int yellowLineInterval = 4;
	public int blueLineInterval = 40;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDrawGizmos(){
		if (!makeLines) return;
		Color yellow = new Color(1, 1, 0, alpha);
		Color red = new Color(1, 0, 0, alpha);
		Color cyan = new Color(0, 1, 1, alpha);
		for (int i = 0; i < horizontalLines; i ++){
			
			Gizmos.color = i % blueLineInterval == 0? cyan : (i % yellowLineInterval == 0? yellow : red);
			Gizmos.DrawLine(new Vector3(originX, originY + hSpacing * i, 0), new Vector3(originX + hLength, originY + hSpacing * i, 0));
		}
		for (int i = 0; i < verticalLines; i ++){
			Gizmos.color = i % blueLineInterval == 0? cyan : (i % yellowLineInterval == 0? yellow : red);
			Gizmos.DrawLine(new Vector3(originX + vSpacing * i, originY, 0), new Vector3(originX + vSpacing * i, originY + vLength, 0));
		}
	}
}
