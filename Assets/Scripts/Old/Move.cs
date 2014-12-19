using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	private Vector3 destPos;
	private Vector3 destRot;
	private Vector3 velocity = Vector3.zero;
	private Quaternion targetRotation;

	// Use this for initialization
	void Start () {
		destPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		// move objects when moved in the mockup
		transform.position =  Vector3.SmoothDamp (transform.position, destPos,  ref velocity, 0.3F);
		//transform.rotation = Vector3.SmoothDamp (transform.rotation

		// Smoothly rotates towards target
		//var targetRotation = Quaternion.LookRotation(destPos - transform.position, Vector3.up);
		if (destRot != new Vector3 (0, 0, 0)) {
			targetRotation = Quaternion.Euler (destRot);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f); 
		}

	}

	public void setDestPos (Vector3 dp) {
		destPos = dp;
	}

	public void setDestRot (Vector3 r) {
		destRot = r;
	}
}
