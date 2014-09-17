using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour {

	public Transform toFollow;
	
	private Transform box;
	Transform trans;
	
	private Rect border;
	private Rect effectiveBorder;
	public Rect Border {
		get{ return border; }
		set{ border = value; }
	}
	
	private float lerpAmount = 0.1f;
	
	
	public static CameraFollow cam;
	
	// Use this for initialization
	
	void Awake () {
		trans = transform;
		if (cam == null){
			cam = this;
		} else {
			Destroy(this);
		}
	}
	
	void Start () {
		
//		effectiveBorder = new Rect(border);
//		effectiveBorder = new RectOffset((int) camera.orthographicSize * Screen.width/Screen.height,(int)  camera.orthographicSize * Screen.width/Screen.height,
//		                                 (int) camera.orthographicSize,(int)  camera.orthographicSize).Remove(effectiveBorder);

		
	}
	
	// Update is called once per frame
	void Update () {
	
			Vector3 target = new Vector3(toFollow.position.x , toFollow.position.y , trans.position.z);
			trans.position = Vector3.Lerp(trans.position, target, lerpAmount);
			
		/*if (effectiveBorder.width > 1 && !effectiveBorder.Contains((Vector2)trans.position)){
			trans.position = new Vector3(Mathf.Clamp(trans.position.x, effectiveBorder.xMin, effectiveBorder.xMax),
									Mathf.Clamp(trans.position.y, effectiveBorder.yMin, effectiveBorder.yMax), trans.position.z);
		}*/
		
	}
	
}
