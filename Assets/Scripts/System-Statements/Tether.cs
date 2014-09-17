using UnityEngine;
using System.Collections;

public static class Tether {
	
	private static Vector3 tetherTarget = Vector3.right;
	private static Vector3 previousTetherTarget = Vector3.right;
	private static float tetherLength = 3f;
	private static float slerpAmount = 0.2f;
	
	public static Vector3 Origin{
		get{
			Vector3 centre = Semicolon.instance.transform.position;
			
			return Vector3.Lerp (centre, Target, 0.2f);
		}
	}
	public static Vector3 Target{
		get{
			return tetherTarget;
		}
	}
	public static Vector3 Bar {
		get{
			return Target + Vector3.up * 0.5f;
		}
	}
	
	static Tether () {
		
	}
	
	public static void Step () {
		Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 target = Vector3.Slerp(previousTetherTarget, mouse, slerpAmount);
		Transform body = Semicolon.instance.transform;
		
		target = new Vector3(target.x, target.y, body.position.z);
		Vector3 diff = target - body.position;
		tetherTarget = diff.normalized * tetherLength + body.position;
	}
	
	public static void EndStep() {
		previousTetherTarget = tetherTarget;
	}
}
