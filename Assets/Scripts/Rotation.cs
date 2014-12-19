using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {
	private bool bigElementIsMoved = false;
	private bool rotationDirection = true;
	private bool rotationAxis = true;
	private float angle = 0; // rotation degree
	private float test = 0; // test if rotation is fully completed

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (bigElementIsMoved) {
			Rotate ();
			test = test + 1;
			if (test >= angle) {
				bigElementIsMoved = false;
				test = 0;
			}
		}

		// keyboard rotations
		if (Input.GetKey ("right")) {
			rotationAxis = true;
			rotationDirection = true;
			Rotate ();
		} else if (Input.GetKey ("left")) {
			rotationAxis = true;
			rotationDirection = false;
			Rotate ();
		} else if (Input.GetKey ("up")) {
			rotationAxis = false;
			rotationDirection = true;
			Rotate ();
		} else if (Input.GetKey ("down")) {
			rotationAxis = false;
			rotationDirection = false;
			Rotate ();
		}		
	}

	void Rotate () {
		Vector3 centre; // origine
		Vector3 axe;
		float angle = 0;
		if (rotationAxis) { // horizontal rotation
			if (rotationDirection) {
				transform.Rotate (new Vector3 (0, 1, 0));
				GameObject.Find ("HandController").transform.Rotate (new Vector3 (0, 1, 0));
			}
			else {
				transform.Rotate (new Vector3 (0, -1, 0));
				GameObject.Find ("HandController").transform.Rotate (new Vector3 (0, 1, 0));
			}
		} else { // vertical rotation

			if (rotationDirection) // vers le bas
				transform.Rotate (new Vector3 (1, 0, 0));
			else
				transform.Rotate (new Vector3 (-1, 0, 0)); // vers le haut
		}
	}

	// ACCESSORS MUTATORS --------------------------------------------
	public void setBigElementIsMoved (bool im) {
		bigElementIsMoved = im;
	}
	
	public void setRotationDirection (bool ro) {
		rotationDirection = ro;
	}

	public void setRotationAxis (bool ra) {
		rotationAxis = ra;
	}
	
	public void setRotationAngle (float deg) {
		angle = deg;
	}
}
