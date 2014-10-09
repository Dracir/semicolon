using UnityEngine;

public class Player : MonoBehaviour {
	
	public float speed = 10;
	
	void FixedUpdate(){
		rigidbody2D.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
	}
}
