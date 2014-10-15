using UnityEngine;
using System.Collections;

public class DrawTether : MonoBehaviour {
	
	
	public Transform avatar;
	public Transform node;
	public Material mat;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Tether.Step();
	}
	
	void OnPostRender () {
		GL.PushMatrix();
		mat.SetPass(0);
		//GL.LoadOrtho();
		GL.Color(Color.white);
		
		GL.Begin(GL.LINES);
		GL.Vertex(Tether.Origin);
		GL.Vertex(Tether.Target);
		//GL.Vertex(Tether.Bar);
		GL.End();
		GL.PopMatrix();
		
		Tether.EndStep();
	}
	
	void OnDrawGizmos(){
//		if (Application.isPlaying){
//			Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//			Vector3 target = Vector3.Slerp(previousTetherTarget, mouse, slerpAmount);
//			target = new Vector3(target.x, target.y, t.position.z);
//			Vector3 diff = target - t.position;
//			Vector3 displayTarget = diff.normalized * tetherLength + t.position;
//			
//			Gizmos.DrawLine(t.position, displayTarget);
//			previousTetherTarget = displayTarget;
//		}
	}
}
