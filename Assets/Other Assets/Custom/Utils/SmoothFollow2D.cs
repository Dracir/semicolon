using UnityEngine;

[ExecuteInEditMode]
public class SmoothFollow2D : MonoBehaviour {

	public Transform target; 
	public Vector3 offset = new Vector3(0, 0, -10);
	public Vector3 damping = new Vector3(100, 100, 100);
	public bool useMaxDistance;
	public Vector3 maxDistance;
	
	void Update(){
		if (!Application.isPlaying && target){
			damping.x = Mathf.Max(damping.x, 0);
			damping.y = Mathf.Max(damping.y, 0);
			damping.z = Mathf.Max(damping.z, 0);
			transform.position = target.position + offset;
		}
		else if (Application.isPlaying && target){
			Vector3 position = transform.position;
			position.x = Mathf.Lerp(position.x, target.position.x + offset.x, damping.x * Time.deltaTime);
			position.y = Mathf.Lerp(position.y, target.position.y + offset.y, damping.y * Time.deltaTime);
			position.z = Mathf.Lerp(position.z, target.position.z + offset.z, damping.z * Time.deltaTime);
			if (useMaxDistance){
				position.x = Mathf.Clamp(position.x, target.position.x - maxDistance.x, position.x + maxDistance.x);
				position.y = Mathf.Clamp(position.y, target.position.y - maxDistance.y, position.y + maxDistance.y);
				position.z = Mathf.Clamp(position.z, target.position.z - maxDistance.z, position.z + maxDistance.z);
			}
			transform.position = position;
		}
	}
}
			