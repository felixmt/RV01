using UnityEngine;
using System.Collections;

public class CameraPosition : MonoBehaviour {
	private bool bigElementIsMoved = false;
	private Vector3 destPos;
	private bool rotationDirection = true;
	private float angle = 0;
	private float test = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (bigElementIsMoved) {
			Vector3 screenPos = camera.WorldToScreenPoint(destPos);
			if (bigElementIsMoved) {
				Rotate (rotationDirection);
				test = test + 2;
				if (test >= angle) {
					bigElementIsMoved = false;
					test = 0;
				}
			}
			//print("target x is " + screenPos.x + " pixels from the left and screen width is: " + Screen.width + " target Z is : " + screenPos.z + "screen height is " + Screen.height);
			/*if ((screenPos.x > Screen.width || screenPos.z > Screen.height || screenPos.x < 0 || screenPos.z < 0)) {
				Rotate (rotationDirection);
				screenPos = camera.WorldToScreenPoint(destPos);
			} /*else if(screenPos.x < (Screen.width / 2) - 10  || screenPos.x > (Screen.width / 2) + 10) {
				Rotate (rotationDirection);
				screenPos = camera.WorldToScreenPoint(destPos);
			}
			else {
				bigElementIsMoved = false;
				//destPos = new Vector3 (0, 0, 0);
			}*/
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

	public void setDestPos (Vector3 dp) {
		destPos = dp;
	}

	public void setRotationAngle (float deg) {
		angle = deg;
	}

	void Rotate (bool sens = true) {
		Vector3 centre = new Vector3(250,1.2f,250); // origine
		Vector3 axe = Vector3.up; // Axe vertical de rotation
		float angle = 0;;
		if (sens)
			angle = 2.0f; // rotation 2 deg
		else
			angle = -2.0f;		
		transform.RotateAround(centre, axe, angle);
	}
}
