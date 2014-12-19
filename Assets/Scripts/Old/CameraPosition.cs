using UnityEngine;
using System.Collections;

public class CameraPosition : MonoBehaviour {
	private bool bigElementIsMoved = false;
	private bool rotationDirection = true;
	private float angle = 0;
	private float test = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (bigElementIsMoved) {
			if (bigElementIsMoved) {
				Rotate (rotationDirection);
				test = test + 1;
				if (test >= angle) {
					bigElementIsMoved = false;
					test = 0;
				}
			}
		}
		if (Input.GetKey ("right")) {
			Rotate ();
		} else if (Input.GetKey ("left")) {
			Rotate (false);
		}	
	}

	public void setBigElementIsMoved (bool im) {
		bigElementIsMoved = im;
	}

	public void setRotationDirection (bool ro) {
		rotationDirection = ro;
	}

	public void setRotationAngle (float deg) {
		angle = deg;
	}

	void Rotate (bool sens = true) {
		Vector3 centre = new Vector3(250,1.2f,250); // origine
		Vector3 axe = Vector3.up; // Axe vertical de rotation
		float angle = 0;;
		if (sens)
			angle = 1.0f; // rotation 2 deg
		else
			angle = -1.0f;		
		transform.RotateAround(centre, axe, angle);
	}
}
