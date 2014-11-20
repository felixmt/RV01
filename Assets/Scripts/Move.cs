using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	private Vector3 destPos;
	private Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
		destPos = transform.position;	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position =  Vector3.SmoothDamp (transform.position, destPos,  ref velocity, 0.3F);
	}

	public void setDestPos (Vector3 dp) {
		destPos = dp;
	}
}
